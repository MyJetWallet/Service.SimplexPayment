using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.Service.Tools;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataWriter;
using MyServiceBus.Abstractions;
using Service.SimplexPayment.Domain.Models;
using Service.SimplexPayment.Domain.Models.NoSql;
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
        private readonly IMyNoSqlServerDataWriter<PendingPaymentNoSqlEntity> _paymentWriter;
        private readonly IMyNoSqlServerDataWriter<SimplexEventsNoSqlEntity> _simplexWriter;
        
        public SimplexEventCleaningJob(ILogger<SimplexEventCleaningJob> logger, SimplexHttpClient client, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IServiceBusPublisher<SimplexIntention> publisher, IMyNoSqlServerDataWriter<PendingPaymentNoSqlEntity> paymentWriter, IMyNoSqlServerDataWriter<SimplexEventsNoSqlEntity> simplexWriter)
        {
            _logger = logger;
            _client = client;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisher = publisher;
            _paymentWriter = paymentWriter;
            _simplexWriter = simplexWriter;

            _timer = new MyTaskTimer(typeof(SimplexEventCleaningJob), TimeSpan.FromMinutes(1), logger, DoProcess);
        }

        private async Task DoProcess()
        {
            try
            {
                var eventsResponse = await _client.GetEvents();
                if (!eventsResponse.Events.Any())
                    return;

                var eventsToSave = new List<SimplexEvent>();
                var eventsToCalculate = new List<SimplexEvent>();
                var clientIdDictionary = new Dictionary<string, string>();
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                foreach (var simplexEvent in eventsResponse.Events)
                {
                    var paymentId = simplexEvent.Payment.Id;
                    var intention = await context.Intentions.FirstOrDefaultAsync(t => t.PaymentId == paymentId);
                    if(intention == null)
                        continue;

                    clientIdDictionary.TryAdd(intention.ClientIdHash, intention.ClientId);
                    eventsToSave.Add(simplexEvent);
                    _logger.LogDebug("Got simplex event for existing intention, event: {eventJson}", simplexEvent.ToJson());

                    //payment_simplexcc_crypto_sent
                    //pending_simplexcc_payment_to_partner
                    //pending_simplexcc_approval
                    //simplexcc_declined 
                    //cancelled
                    
                    if (simplexEvent.Payment.Status == "pending_simplexcc_payment_to_partner" ||
                        simplexEvent.Payment.Status == "payment_simplexcc_crypto_sent" || 
                        simplexEvent.Payment.Status == "cancelled")
                    {
                        if (simplexEvent.Payment.Status == "payment_simplexcc_crypto_sent")
                        {
                            intention.Status = SimplexStatus.PaymentCompleted;
                            intention.BlockchainTxHash = simplexEvent.Payment.BlockchainTxHash;
                        }
                        else
                        {
                            intention.Status = simplexEvent.Payment.Status == "pending_simplexcc_payment_to_partner" ? SimplexStatus.PaymentApproved : SimplexStatus.Cancelled;
                        }

                        await _publisher.PublishAsync(intention);
                        await context.UpsertAsync(new[] {intention});
                    }

                    if (simplexEvent.Payment.Status == "pending_simplexcc_payment_to_partner" ||
                        simplexEvent.Payment.Status == "pending_simplexcc_approval")
                    {
                        eventsToCalculate.Add(simplexEvent);
                    }
                    
                    await _client.DeleteEvent(simplexEvent.EventId);
                }


                if (eventsToSave.Any())
                    await _simplexWriter.InsertOrReplaceAsync(SimplexEventsNoSqlEntity.Create(eventsToSave));

                if(eventsToCalculate.Any())
                    await CalculatePendingBalances(eventsToCalculate, clientIdDictionary);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When cleaning Simplex events");
                throw;
            }
        }

        private async Task CalculatePendingBalances(List<SimplexEvent> simplexEvents, Dictionary<string, string> clientIdDictionary)
        {
            var payments = new Dictionary<string, Dictionary<string, decimal>>();
            var groupsByClient = simplexEvents.GroupBy(t => t.Payment.PartnerEndUserId);
            foreach (var groupByClient in groupsByClient)
            {
                var groupsByAsset = groupByClient.GroupBy(t => t.Payment.CryptoTotalAmount.Currency);
                var dictionary = groupsByAsset.ToDictionary(group => group.Key, group => (decimal) group.Select(t => t.Payment.CryptoTotalAmount.Amount).Sum());
                
                payments.TryAdd(clientIdDictionary[groupByClient.Key], dictionary);
            }

            await _paymentWriter.CleanAndBulkInsertAsync(payments.Select(t =>
                PendingPaymentNoSqlEntity.Create(t.Key, t.Value)));
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