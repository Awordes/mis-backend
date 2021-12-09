using System;
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

        public SchedulerService(IMisDbContext context, IMediator mediator, ILogger<SchedulerService> logger, IMercuryService mercuryService)
        {
            _context = context;
            _mediator = mediator;
            _logger = logger;
            _mercuryService = mercuryService;
        }

        public async Task AutoProcessVsd(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation(
                    string.Format("==============================={0}" +
                                  "==Начало операции автогашения=={1}" +
                                  "===============================",
                        Environment.NewLine, Environment.NewLine));
                
                var users = await _context.Users.AsNoTracking()
                        .Include(x => x.Enterprises)
                    .Where(x => x.AutoVsdProcess && !x.Deleted)
                    .ToListAsync(cancellationToken);

                var datetime = DateTime.Now.AddMinutes(1);

                do
                {
                    Task.WaitAll(users.Select(user => ProcessVsd(user, cancellationToken)).ToArray());
                
                    Console.WriteLine("=====6=====");
                } while (DateTime.Now < datetime);

                _logger.LogInformation(
                    string.Format("==================================={0}" +
                                  "==Завершение операции автогашения=={1}" +
                                  "===================================",
                        Environment.NewLine, Environment.NewLine));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        private async Task ProcessVsd(User user, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                string.Format("==================================={0}" +
                              "==Начало обработки пользователя {2}=={1}" +
                              "===================================",
                    Environment.NewLine, Environment.NewLine, user.UserName));
            foreach (var enterprise in user.Enterprises)
            {
                var vsds = (await _mercuryService.GetVetDocumentList("a10003", user, enterprise, 10, 0, 3, 1)).result;
                
                await _mediator.Send(new ProcessIncomingVsdListAutoCommand
                {
                    Enterprise = enterprise,
                    User = user,
                    Vsds = vsds.Select(vsd => new VsdForProcessModel { VsdId = vsd.Id, ProcessDate = vsd.ProcessDate }).ToList()
                }, cancellationToken);
            }
            
            _logger.LogInformation(
                string.Format("==================================={0}" +
                              "==Завершение обработки пользователя {2}=={1}" +
                              "===================================",
                    Environment.NewLine, Environment.NewLine, user.UserName));
        }
    }
}