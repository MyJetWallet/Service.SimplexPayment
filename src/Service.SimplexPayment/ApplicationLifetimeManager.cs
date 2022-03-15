using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.SimplexPayment.Jobs;

namespace Service.SimplexPayment
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly MyNoSqlClientLifeTime _noSqlClientLifeTime;
        private readonly ServiceBusLifeTime _serviceBusLifeTime;
        private readonly SimplexEventCleaningJob _cleaningJob;
        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, ILogger<ApplicationLifetimeManager> logger, MyNoSqlClientLifeTime noSqlClientLifeTime, ServiceBusLifeTime serviceBusLifeTime, SimplexEventCleaningJob cleaningJob)
            : base(appLifetime)
        {
            _logger = logger;
            _noSqlClientLifeTime = noSqlClientLifeTime;
            _serviceBusLifeTime = serviceBusLifeTime;
            _cleaningJob = cleaningJob;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _serviceBusLifeTime.Start();
            _noSqlClientLifeTime.Start();
            _cleaningJob.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _cleaningJob.Dispose();
            _serviceBusLifeTime.Stop();
            _noSqlClientLifeTime.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
