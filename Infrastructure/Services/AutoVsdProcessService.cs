using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using Core.Application.Usecases.MercuryIntegration.Commands;
using Core.Application.Usecases.MercuryIntegration.Models;
using Core.Application.Usecases.MercuryIntegration.ViewModels;
using Core.Domain.Auth;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class AutoVsdProcessService: IAutoVsdProcessService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AutoVsdProcessService> _logger;
        private readonly IMercuryService _mercuryService;
        private readonly IAutoVsdProcessDataService _autoVsdProcessDataService;

        public AutoVsdProcessService(IMediator mediator,
            ILogger<AutoVsdProcessService> logger,
            IMercuryService mercuryService,
            IAutoVsdProcessDataService autoVsdProcessDataService)
        {
            _mediator = mediator;
            _logger = logger;
            _mercuryService = mercuryService;
            _autoVsdProcessDataService = autoVsdProcessDataService;
        }

        public async Task ProcessVsd(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Начало процедуры автогашения");

                List<User> currentUsers;

                lock (_autoVsdProcessDataService.Locker)
                {
                    currentUsers = _autoVsdProcessDataService.Users.ToList();
                }
                
                _logger.LogInformation("Количество пользователей - {0}", currentUsers.Count);

                await Task.WhenAll(currentUsers.Select(user => ProcessVsd(user, cancellationToken)).ToList());
                
                _logger.LogInformation("Завершение процедуры автогашения");
            }
            catch (MercuryRequestRejectedException)
            {
                throw;
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
        

        private async Task ProcessVsd(User user, CancellationToken cancellationToken)
        {
            try
            {
                if (user.Enterprises is null)
                {
                    lock (_autoVsdProcessDataService.Locker)
                    {
                        _autoVsdProcessDataService.Users.Remove(user);
                    }

                    _logger.LogInformation("Завершение процедуры автогашения для пользователя {0}", user.UserName);
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
                            offset = _autoVsdProcessDataService.VsdBlackList.Count;
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

                    _logger.LogInformation(
                        "Начало сеанса автогашения. Пользователь: {0}, Предприятие: {1}, Количество ВСД: {2}",
                        user.UserName, enterprise.Name, vsdList.Count);

                    await _mediator.Send(new ProcessIncomingVsdListAutoCommand
                    {
                        Enterprise = enterprise,
                        User = user,
                        Vsds = vsdList.Select(vsd => new VsdForProcessModel
                            {VsdId = vsd.Id, ProcessDate = vsd.ProcessDate}).ToList()
                    }, cancellationToken);

                    _logger.LogInformation(
                        "Сеанс автогашения успешно завершён. Пользователь: {0}, Предприятие: {1}, Количество ВСД: {2}",
                        user.UserName, enterprise.Name, vsdList.Count);
                }

                if (user.Enterprises.Count == 0)
                {
                    lock (_autoVsdProcessDataService.Locker)
                    {
                        _autoVsdProcessDataService.Users.Remove(user);
                    }

                    _logger.LogInformation("Завершение процедуры автогашения для пользователя {0}", user.UserName);
                }
            }
            catch (MercuryRequestRejectedException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (AggregateException e)
            {
                foreach (var innerException in e.InnerExceptions)
                {
                    if (innerException is MercuryServiceException mercuryServiceException)
                    {
                        lock (_autoVsdProcessDataService.Locker)
                        {
                            _autoVsdProcessDataService.VsdBlackList.Add(mercuryServiceException.VsdId);
                            _logger.LogInformation("Произошла ошибка при обработке автогашения. Пользователь: {0}", user.UserName);
                            _logger.LogError(mercuryServiceException.Message, mercuryServiceException);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("Произошла ошибка при обработке автогашения. Пользователь: {0}", user.UserName);
                _logger.LogError(e.Message, e);
            }
        }
    }
}