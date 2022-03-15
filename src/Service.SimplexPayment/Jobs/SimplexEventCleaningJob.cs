using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.Service.Tools;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using Service.SimplexPayment.Domain.Models;
using Service.SimplexPayment.Postgres;
using Service.SimplexPayment.Services;

namespace Service.SimplexPayment.Jobs
{
    public class SimplexEventCleaningJob : IStartable, IDisposable
    {
        private readonly ILogger<SimplexEventCleaningJob> _logger;
        private readonly SimplexHttpClient _client;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly MyTaskTimer _timer;
        private readonly IServiceBusPublisher<SimplexIntention> _publisher;

        public SimplexEventCleaningJob(ILogger<SimplexEventCleaningJob> logger, SimplexHttpClient client, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IServiceBusPublisher<SimplexIntention> publisher)
        {
            _logger = logger;
            _client = client;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisher = publisher;

            _timer = new MyTaskTimer(typeof(SimplexEventCleaningJob), TimeSpan.FromMinutes(1), logger, DoProcess);
        }

        private async Task DoProcess()
        {
            try
            {
                var eventsResponse = await _client.GetEvents();
                if (!eventsResponse.Events.Any())
                    return;

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                foreach (var simplexEvent in eventsResponse.Events)
                {
                    var paymentId = simplexEvent.Payment.Id;
                    var intention = await context.Intentions.FirstOrDefaultAsync(t => t.PaymentId == paymentId);
                    if(intention == null)
                        continue;

                    _logger.LogDebug("Got simplex event for existing intention, event: {eventJson}", simplexEvent.ToJson());

                    //pending_simplexcc_payment_to_partner
                    //pending_simplexcc_approval
                    //simplexcc_declined
                    
                    if (simplexEvent.Payment.Status == "simplexcc_approved" ||
                        simplexEvent.Payment.Status == "simplexcc_declined")
                    {
                        intention.Status = simplexEvent.Payment.Status == "simplexcc_declined" ? SimplexStatus.Cancelled : SimplexStatus.PaymentCompleted;

                        await _publisher.PublishAsync(intention);
                        await context.UpsertAsync(new[] {intention});
                        await _client.DeleteEvent(simplexEvent.EventId);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When cleaning Simplex events");
                throw;
            }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}