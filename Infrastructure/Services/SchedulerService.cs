using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Services;
using Core.Application.Usecases.MercuryIntegration.Commands;
using Core.Application.Usecases.MercuryIntegration.Models;
using Core.Domain.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class SchedulerService: ISchedulerService
    {
        private readonly IMisDbContext _context;
        private readonly IMediator _mediator;
        private readonly ILogger<SchedulerService> _logger;
        private readonly IMercuryService _mercuryService;

        private List<User> _users;

        public SchedulerService(
            IMisDbContext context,
            IMediator mediator,
            ILogger<SchedulerService> logger,
            IMercuryService mercuryService)
        {
            _context = context;
            _mediator = mediator;
            _logger = logger;
            _mercuryService = mercuryService;
        }

        public async Task AutoProcessVsd(CancellationToken cancellationToken)
        {
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
                
                var processingTimeEnd = DateTime.Now.AddHours(5);

                _logger.LogInformation("Начало процедуры автогашения");

                do
                {
                    _logger.LogInformation("Количество пользователей - {0}", _users.Count);
                    var currentUsers = _users.Select(x => x).ToList();
                    
                    Task.WaitAll(currentUsers.Select(user => ProcessVsd(user, cancellationToken)).ToArray());
                } while (_users.Count > 0 && DateTime.Now < processingTimeEnd);
                
                _logger.LogInformation("Завершение процедуры автогашения");
            }
            catch (Exception e)
            {
                if (e is TaskCanceledException)
                {
                    _logger.LogInformation("Завершение процедуры автогашения");
                    return;
                }
                
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
                
                _logger.LogInformation("Автогашение для пользователя {0}", user.UserName);
            
                foreach (var enterprise in user.Enterprises)
                {
                    var vsdList = (await _mercuryService.GetVetDocumentList("a10003", user, enterprise, 10, 0, 3, 1)).result;
                    
                    if (vsdList is null || vsdList.Count == 0)
                    {
                        user.Enterprises.Remove(enterprise);
                        continue;
                    }
                    
                    _logger.LogInformation("Пользователь: {0}, Количество ВСД {1}", user.UserName, vsdList?.Count);
                
                    await _mediator.Send(new ProcessIncomingVsdListAutoCommand
                    {
                        Enterprise = enterprise,
                        User = user,
                        Vsds = vsdList.Select(vsd => new VsdForProcessModel { VsdId = vsd.Id, ProcessDate = vsd.ProcessDate }).ToList()
                    }, cancellationToken);
                }

                if (user.Enterprises.Count == 0)
                {
                    _users.Remove(user);
                    _logger.LogInformation("Завершение процедуры автогашения для пользователя {0}", user.UserName);
                }
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    _logger.LogError("Operation cancelled");
                    throw;
                }
                
                _logger.LogInformation("Произошла ошибка при обработке автогашения. Пользователь: {0}", user.UserName);
                _logger.LogError(e.Message, e);
            }
        }
    }
}