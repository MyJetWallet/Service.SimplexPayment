using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.ApiSecurityManager.Notifications;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.SimplexPayment.Jobs;
using Service.SimplexPayment.Services;

namespace Service.SimplexPayment
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly MyNoSqlClientLifeTime _noSqlClientLifeTime;
        private readonly ServiceBusLifeTime _serviceBusLifeTime;
        private readonly SimplexEventCleaningJob _cleaningJob;
        private readonly SimplexHttpClient _simplexHttpClient;
        private readonly INotificatorSubscriber _notificatorSubscriber;

        public ApplicationLifetimeManager(
            IHostApplicationLifetime appLifetime,
            ILogger<ApplicationLifetimeManager> logger,
            MyNoSqlClientLifeTime noSqlClientLifeTime,
            ServiceBusLifeTime serviceBusLifeTime,
            SimplexEventCleaningJob cleaningJob,
            SimplexHttpClient simplexHttpClient,
            INotificatorSubscriber notificatorSubscriber)
            : base(appLifetime)
        {
            _logger = logger;
            _noSqlClientLifeTime = noSqlClientLifeTime;
            _serviceBusLifeTime = serviceBusLifeTime;
            _cleaningJob = cleaningJob;
            _simplexHttpClient = simplexHttpClient;
            _notificatorSubscriber = notificatorSubscriber;
        }

        protected override void OnStarted()
        {
            _notificatorSubscriber.Subscribe((apiKey) =>
            {
                if (apiKey.Id == Program.Settings.ApiKeyId)
                    _simplexHttpClient.SetUpApiKey(apiKey.ApiKeyValue);
            });

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
