using Hangfire.Dashboard;

namespace Infrastructure.Hangfire
{
    public class HangfireAuthorizationFilter: IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}