using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using Infrastructure.QuartzJobs.AutoVsdProcess;
using Quartz;

namespace Infrastructure.Services
{
    public class AutoVsdProcessingStartService: IAutoVsdProcessingStartService
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public AutoVsdProcessingStartService(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task StartAutoVsdProcessing(CancellationToken cancellationToken)
        {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            var job = JobBuilder.Create<StartProcessingJob>().Build();

            var trigger = TriggerBuilder.Create().StartNow().Build();

            await scheduler.ScheduleJob(job, trigger, cancellationToken);
        }
    }
}