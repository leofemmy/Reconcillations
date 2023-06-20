using Serilog;
using System.Data.SqlClient;
using System.Data;
using System;
using Hangfire;

namespace Reconcillations.Services
{
    public static class PusherServices
    {
        public static void InitialiseService()
        {
            // Pusher Reems Starts Here
            const string reemsPushJobId = "482cbab9d555-423abfe6c2c5b541276c";
            RecurringJob.RemoveIfExists(reemsPushJobId);

            RecurringJob.AddOrUpdate<HangFireServices.ServiceScheduler>(reemsPushJobId, x => x.DoReemsPushAsync(), Cron.MinuteInterval(45));

            //RecurringJob.AddOrUpdate<ServiceScheduler>(reemsPushJobId, x => x.DoReemsPushAsync(), Cron.MinuteInterval(2));
        }

        
        
    }
}
