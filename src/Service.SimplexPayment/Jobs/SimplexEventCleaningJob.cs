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
        
        public SimplexEventCleaningJob(
            ILogger<SimplexEventCleaningJob> logger, 
            SimplexHttpClient client,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<SimplexIntention> publisher,
            IMyNoSqlServerDataWriter<PendingPaymentNoSqlEntity> paymentWriter,
            IMyNoSqlServerDataWriter<SimplexEventsNoSqlEntity> simplexWriter)
        {
            _logger = logger;
            _client = client;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisher = publisher;
            _paymentWriter = paymentWriter;
            _simplexWriter = simplexWriter;

            _timer = new MyTaskTimer(typeof(SimplexEventCleaningJob), TimeSpan.FromSeconds(Program.Settings.TimerPeriodInSec), logger, DoProcess);
        }

        private async Task DoProcess()
        {
            try
            {
                if (!_client.IsApiKeySetUp)
                {
                    _logger.LogError("Cant get Simplex Events Api Key is not set");
                    return;
                }

                var eventsResponse = await _client.GetEvents();
                if (!eventsResponse.Events.Any())
                    return;

                var eventsToSave = new List<SimplexEvent>();
                var clientIdDictionary = new Dictionary<string, string>();
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                foreach (var simplexEvent in eventsResponse.Events)
                {
                    var paymentId = simplexEvent.Payment.Id;
                    var intention = await context.Intentions.FirstOrDefaultAsync(t => t.PaymentId == paymentId);
                    if(intention == null)
                        continue;

                    _logger.LogDebug("Got simplex event for existing intention, event: {eventJson}", simplexEvent.ToJson());

                    clientIdDictionary.TryAdd(intention.ClientIdHash, intention.ClientId);
                    eventsToSave.Add(simplexEvent);

                    var status = MapEventToStatus(simplexEvent.Name);
                    
                    if(intention.Status > status)
                        continue;
                    
                    intention.Status = status;
                    intention.BlockchainTxHash = simplexEvent.Payment.BlockchainTxHash;
                    
                    await _publisher.PublishAsync(intention);
                    await context.UpsertAsync(new[] {intention});
                    
                    await _client.DeleteEvent(simplexEvent.EventId);

                    if (!Program.Settings.ProductionMode 
                        && simplexEvent.Name == "payment_simplexcc_approved")
                    {
                        intention.Status = SimplexStatus.CryptoSent;
                        intention.BlockchainTxHash = $"simplex|mock|{Guid.NewGuid():N};";
                        await _publisher.PublishAsync(intention);
                        await context.UpsertAsync(new[] {intention});
                    }
                }
                
                if (eventsToSave.Any())
                    await _simplexWriter.InsertOrReplaceAsync(SimplexEventsNoSqlEntity.Create(eventsToSave));
                
                await CalculatePendingBalances(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When cleaning Simplex events");
                throw;
            }
            
            //locals
            SimplexStatus MapEventToStatus(string name)
            {
                return name switch
                {
                    "payment_request_submitted" => SimplexStatus.PaymentSubmitted,
                    "payment_simplexcc_approved" => SimplexStatus.PaymentApproved,
                    "payment_simplexcc_declined" => SimplexStatus.Declined,
                    "payment_simplexcc_refunded" => SimplexStatus.Refunded,
                    "payment_simplexcc_crypto_sent" => SimplexStatus.CryptoSent,
                    _ => SimplexStatus.QuoteCreated
                };
            }
        }

        private async Task CalculatePendingBalances(DatabaseContext context)
        {
            var data = await context.Intentions
                .Where(e => e.Status == SimplexStatus.PaymentSubmitted || e.Status == SimplexStatus.PaymentApproved  || e.Status == SimplexStatus.CryptoSent)
                .GroupBy(e => new {e.ClientId, e.ToAsset})
                .Select(e => new {ClientId = e.Key.ClientId, Asset = e.Key.ToAsset, Amount = e.Sum(i => i.ToAmount)})
                .ToListAsync();

            var items = data
                .GroupBy(e => e.ClientId)
                .Select(e => PendingPaymentNoSqlEntity.Create(
                    e.Key,
                    e.ToDictionary(i => i.Asset, i => i.Amount)))
                .ToList();
            
            await _paymentWriter.CleanAndBulkInsertAsync(items);
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