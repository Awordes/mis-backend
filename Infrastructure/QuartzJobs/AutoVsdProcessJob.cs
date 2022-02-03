using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Services;
using Core.Application.Usecases.MercuryIntegration.Commands;
using Core.Application.Usecases.MercuryIntegration.Models;
using Core.Application.Usecases.MercuryIntegration.ViewModels;
using Core.Domain.Auth;
using Infrastructure.Services;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.QuartzJobs
{
    public class AutoVsdProcessJob: IJob
    {
        private readonly IMisDbContext _context;
        private readonly IMediator _mediator;
        private readonly ILogger<AutoVsdProcessJob> _logger;
        private readonly IMercuryService _mercuryService;

        private List<User> _users;
        
        public AutoVsdProcessJob(
            IMisDbContext context,
            IMediator mediator,
            ILogger<AutoVsdProcessJob> logger,
            IMercuryService mercuryService)
        {
            _context = context;
            _mediator = mediator;
            _logger = logger;
            _mercuryService = mercuryService;
        }
        
        public async Task Execute(IJobExecutionContext context)
        {
            var cancellationToken = context.CancellationToken;
            try
            {
                _users = await _context.Users.AsNoTracking()
                    .Include(x => x.Enterprises)
                    .Where(x => x.AutoVsdProcess && !x.Deleted)
                    .ToListAsync(cancellationToken);

                if (_users.Count == 0)
                {
                    _logger.LogInformation("Не найдены пользователи для автогашения.");
                    return;
                }
                
                var processingTimeEnd = DateTime.Now.AddHours(3);

                _logger.LogInformation("Начало процедуры автогашения");

                do
                {
                    _logger.LogInformation("Количество пользователей - {0}", _users.Count);
                    var currentUsers = _users.Select(x => x).ToList();
                    
                    Task.WaitAll(currentUsers.Select(user => ProcessVsd(user, cancellationToken)).ToArray());
                } while (_users.Count > 0 && DateTime.Now < processingTimeEnd);
                
                _logger.LogInformation("Завершение процедуры автогашения");
            }
            catch (MercuryServiceException)
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
                    _users.Remove(user);
                    _logger.LogInformation("Завершение процедуры автогашения для пользователя {0}", user.UserName);
                    return;
                }

                foreach (var enterprise in user.Enterprises)
                {
                    var vsdList = new List<VsdViewModel>();

                    try
                    {
                        vsdList = (await _mercuryService.GetVetDocumentList("a10003", user, enterprise, 10, 0, 3, 1))
                            .result;
                    }
                    catch (MercuryEnterpriseNotFoundException)
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
                        user.UserName, enterprise.Name, vsdList?.Count);

                    await _mediator.Send(new ProcessIncomingVsdListAutoCommand
                    {
                        Enterprise = enterprise,
                        User = user,
                        Vsds = vsdList.Select(vsd => new VsdForProcessModel
                            {VsdId = vsd.Id, ProcessDate = vsd.ProcessDate}).ToList()
                    }, cancellationToken);

                    _logger.LogInformation(
                        "Сеанс автогашения успешно завершён. Пользователь: {0}, Предприятие: {1}, Количество ВСД: {2}",
                        user.UserName, enterprise.Name, vsdList?.Count);
                }

                if (user.Enterprises.Count == 0)
                {
                    _users.Remove(user);
                    _logger.LogInformation("Завершение процедуры автогашения для пользователя {0}", user.UserName);
                }
            }
            catch (MercuryServiceException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogInformation("Произошла ошибка при обработке автогашения. Пользователь: {0}", user.UserName);
                _logger.LogError(e.Message, e);
            }
        }
    }
}
