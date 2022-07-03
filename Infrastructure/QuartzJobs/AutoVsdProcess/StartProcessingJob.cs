using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Services;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Infrastructure.QuartzJobs.AutoVsdProcess
{
    public class StartProcessingJob: IJob
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<StartProcessingJob> _logger;
        private readonly IAutoVsdProcessDataService _autoVsdProcessDataService;
        private readonly IMisDbContext _context;
        private readonly AutoVsdProcessingOptions _autoVsdProcessingOptions;

        public StartProcessingJob(ISchedulerFactory schedulerFactory,
            ILogger<StartProcessingJob> logger,
            IAutoVsdProcessDataService autoVsdProcessDataService,
            IMisDbContext context,
            IOptionsMonitor<AutoVsdProcessingOptions> autoVsdProcessingOptions)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
            _autoVsdProcessDataService = autoVsdProcessDataService;
            _context = context;
            _autoVsdProcessingOptions = autoVsdProcessingOptions.CurrentValue;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken);

            var job = JobBuilder.Create<ReprocessingJob>().Build();

            var trigger = TriggerBuilder.Create().StartNow().Build();
            
            _autoVsdProcessDataService.AutoProcessEnd = DateTime.Now.Add(new TimeSpan(
                _autoVsdProcessingOptions.AutoVsdProcessingTimeSpan.Hours,
                _autoVsdProcessingOptions.AutoVsdProcessingTimeSpan.Minutes,
                _autoVsdProcessingOptions.AutoVsdProcessingTimeSpan.Seconds));
            
            _autoVsdProcessDataService.Users = await _context.Users.AsNoTracking()
                .Include(x => x.Enterprises)
                .Where(x => x.AutoVsdProcess && !x.Deleted)
                .ToListAsync(context.CancellationToken);

            _autoVsdProcessDataService.VsdBlackList = new Dictionary<Guid, ICollection<string>>();

            await scheduler.ScheduleJob(job, trigger, context.CancellationToken);
            
            _logger.LogInformation("Запуск процедуры автогашения.");
        }
    }
}