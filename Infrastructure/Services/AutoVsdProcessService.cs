using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using Core.Application.Usecases.MercuryIntegration.Models;
using Core.Application.Usecases.MercuryIntegration.ViewModels;
using Core.Domain.Auth;
using Core.Domain.Operations;
using Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class AutoVsdProcessService: IAutoVsdProcessService
    {
        private readonly ILogger<AutoVsdProcessService> _logger;
        private readonly IMercuryService _mercuryService;
        private readonly IAutoVsdProcessDataService _autoVsdProcessDataService;
        private readonly ILogService _logService;

        public AutoVsdProcessService(ILogger<AutoVsdProcessService> logger,
            IMercuryService mercuryService,
            IAutoVsdProcessDataService autoVsdProcessDataService,
            ILogService logService)
        {
            _logger = logger;
            _mercuryService = mercuryService;
            _autoVsdProcessDataService = autoVsdProcessDataService;
            _logService = logService;
        }

        public async Task StartProcessing(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Начало процедуры автогашения");

                List<User> currentUsers;

                lock (_autoVsdProcessDataService.Locker)
                {
                    currentUsers = _autoVsdProcessDataService.Users.ToList();
                }
                
                _logger.LogInformation($"Количество пользователей - {currentUsers.Count}");

                await Task.WhenAll(currentUsers.Select(user => ProcessUserVsds(user, cancellationToken)).ToList());
                
                _logger.LogInformation("Завершение процедуры автогашения");
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Процедуры автогашения остановлена.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Процедуры автогашения отменена.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
        
        private async Task ProcessUserVsds(User user, CancellationToken cancellationToken)
        {
            try
            {
                if (user.Enterprises is null)
                {
                    lock (_autoVsdProcessDataService.Locker)
                    {
                        _autoVsdProcessDataService.Users.Remove(user);
                    }

                    _logger.LogInformation($"Завершение процедуры автогашения для пользователя {user.UserName}");
                    return;
                }

                foreach (var enterprise in user.Enterprises)
                {
                    List<VsdViewModel> vsdList;

                    try
                    {
                        var offset = 0;
                        lock (_autoVsdProcessDataService.Locker)
                        {
                            _autoVsdProcessDataService.VsdBlackList.TryGetValue(enterprise.Id, out var blacklist);
                            
                            if (blacklist is not null)
                                offset = blacklist.Count;
                        }
                        
                        vsdList = (await _mercuryService.GetVetDocumentList("a10003", user, enterprise, 10, offset, 3, 1))
                            .result;
                    }
                    catch (MercuryEnterpriseNotFoundException)
                    {
                        user.Enterprises.Remove(enterprise);
                        continue;
                    }
                    catch (MercuryOffsetException)
                    {
                        user.Enterprises.Remove(enterprise);
                        continue;
                    }

                    if (vsdList is null || vsdList.Count == 0)
                    {
                        user.Enterprises.Remove(enterprise);
                        continue;
                    }

                    _logger.LogInformation($"Начало сеанса автогашения. " +
                                           $"Пользователь: {user.UserName}, " +
                                           $"Предприятие: {enterprise.Name}, " +
                                           $"Количество ВСД: {vsdList.Count}");

                    await ProcessVsd(
                        user,
                        enterprise,
                        vsdList.Select(vsd => new VsdForProcessModel
                            {
                                VsdId = vsd.Id, ProcessDate = vsd.ProcessDate
                            }
                        ).ToList(),
                        cancellationToken);

                    _logger.LogInformation($"Сеанс автогашения успешно завершён. " +
                                           $"Пользователь: {user.UserName}, " +
                                           $"Предприятие: {enterprise.Name}, " +
                                           $"Количество ВСД: {vsdList.Count}");
                }

                if (user.Enterprises.Count == 0)
                {
                    lock (_autoVsdProcessDataService.Locker)
                    {
                        _autoVsdProcessDataService.Users.Remove(user);
                    }

                    _logger.LogInformation($"Завершение процедуры автогашения для пользователя {user.UserName}");
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError($"Произошла ошибка при обработке автогашения. Пользователь: {user.UserName}", e);
            }
        }

        private async Task ProcessVsdMiddleware(
            string localTransactionId, User user, Enterprise enterprise, string uuid, DateTime? processDate,
            Guid operationId)
        {
            try
            {
                await _mercuryService.ProcessIncomingConsignment(localTransactionId, user, enterprise, uuid,
                    processDate, operationId);
            }
            catch (MercuryServiceException e)
            {
                lock (_autoVsdProcessDataService.Locker)
                {
                    _autoVsdProcessDataService.VsdBlackList.TryGetValue(e.EnterpriseId,
                        out var blacklist);
                            
                    if (blacklist is null)
                        _autoVsdProcessDataService.VsdBlackList.Add(e.EnterpriseId, new List<string> { e.VsdId });
                    else
                        blacklist.Add(e.VsdId);
                    
                    _logger.LogError(e, $"Произошла ошибка при обработке автогашения. Пользователь: {user.UserName}");
                }
                
            }
            catch (Exception e)
            {
                _logger.LogError($"Произошла ошибка при обработке автогашения. Пользователь: {user.UserName}", e);
            }
        }

        private async Task ProcessVsd(
            User user, Enterprise enterprise, IEnumerable<VsdForProcessModel> vsds, CancellationToken cancellationToken)
        {
            var operationId = Guid.Empty;
            
            try
            {
                operationId = await _logService.StartOperation(user.Id, OperationType.VsdProcess, cancellationToken);
                    
                var tasks = new List<Task>();

                foreach (var vsd in vsds)
                {
                    tasks.Add(ProcessVsdMiddleware(operationId.ToString(), user, enterprise, vsd.VsdId, vsd.ProcessDate, operationId));

                    if (tasks.Count < 4) continue;

                    await Task.WhenAll(tasks);

                    tasks = new List<Task>();
                }

                await Task.WhenAll(tasks);
            }
            finally
            {
                if (operationId != Guid.Empty)
                    await _logService.FinishOperation(operationId, cancellationToken);
            }
        }
    }
}