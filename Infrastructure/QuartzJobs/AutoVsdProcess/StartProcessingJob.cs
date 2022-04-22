using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.QuartzJobs.AutoVsdProcess
{
    public class StartProcessingJob: IJob
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<StartProcessingJob> _logger;
        private readonly IAutoVsdProcessDataService _autoVsdProcessDataService;
        private readonly IMisDbContext _context;

        public StartProcessingJob(ISchedulerFactory schedulerFactory,
            ILogger<StartProcessingJob> logger,
            IAutoVsdProcessDataService autoVsdProcessDataService,
            IMisDbContext context)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
            _autoVsdProcessDataService = autoVsdProcessDataService;
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken);

            var job = JobBuilder.Create<ReprocessingJob>().Build();

            var trigger = TriggerBuilder.Create().StartNow().Build();
            
            //todo вынести в опции
            _autoVsdProcessDataService.AutoProcessEnd = DateTime.Now.AddHours(3);
            
            _autoVsdProcessDataService.Users = await _context.Users.AsNoTracking()
                .Include(x => x.Enterprises)
                .Where(x => x.AutoVsdProcess && !x.Deleted)
                .ToListAsync(context.CancellationToken);

            _autoVsdProcessDataService.VsdBlackList = new List<string>();

            await scheduler.ScheduleJob(job, trigger, context.CancellationToken);
            
            _logger.LogInformation("Запуск процедуры автогашения.");
        }
    }
}