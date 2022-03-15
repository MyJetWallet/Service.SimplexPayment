using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.ClientProfile.Grpc;
using Service.ClientProfile.Grpc.Models.Requests;
using Service.SimplexPayment.Domain.Models;
using Service.SimplexPayment.Grpc.Models;
using Service.SimplexPayment.Postgres;

namespace Service.SimplexPayment.Services
{
    public class SimplexPaymentService
    {
        private const string SandboxBase = "https://sandbox.test-simplexcc.com/";
        private const string ProdBase = "https://backend-wallet-api.simplexcc.com/";
        private const string GetQuotePath = "wallet/merchant/v2/quote";
        private const string RequestPaymentPath = "wallet/merchant/v2/payments/partner/data";
        private const string EventsPath = "events";

        private readonly ILogger<SimplexPaymentService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IClientProfileService _clientProfile;
        private readonly SimplexHttpClient _client;

        public SimplexPaymentService(ILogger<SimplexPaymentService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IClientProfileService clientProfile, SimplexHttpClient client)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _clientProfile = clientProfile;
            _client = client;
        }

        public async Task<ExecuteQuoteResponse> RequestPayment(RequestPaymentRequest requestPaymentRequest)
        {
            try
            {
                var profile = await _clientProfile.GetOrCreateProfile(new GetClientProfileRequest
                {
                    ClientId = requestPaymentRequest.ClientId
                });
               
                var quoteResponse = await _client.GetQuote(
                    new GetQuoteRequestModel
                    {
                        EndUserId = profile.ExternalClientId,
                        DigitalCurrency = requestPaymentRequest.ToAsset,
                        FiatCurrency = requestPaymentRequest.FromCurrency,
                        RequestedCurrency = requestPaymentRequest.FromCurrency,
                        RequestedAmount = requestPaymentRequest.FromAmount,
                        WalletId = Program.Settings.SimplexWalletId,
                        ClientIp = requestPaymentRequest.ClientIp,
                        PaymentMethods = new() {"credit_card"}
                    });

                var intention = new SimplexIntention
                {
                    QuoteId = quoteResponse.QuoteId,
                    ClientId = requestPaymentRequest.ClientId,
                    ClientIdHash = profile.ExternalClientId,
                    FromAmount = requestPaymentRequest.FromAmount,
                    FromCurrency = requestPaymentRequest.FromCurrency,
                    ToAmount = quoteResponse.DigitalMoney.Amount,
                    ToAsset = quoteResponse.DigitalMoney.Currency,
                    ClientIp = requestPaymentRequest.ClientIp,
                    CreationTime = DateTime.UtcNow,
                    Status = SimplexStatus.QuoteCreated
                };
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.UpsertAsync(new[] {intention});
               
                
                var paymentId = Guid.NewGuid().ToString("D");
                var orderId = Guid.NewGuid().ToString("D");

                intention.PaymentId = paymentId;
                intention.Status = SimplexStatus.QuoteConfirmed;
                await context.UpsertAsync(new[] {intention});
                
                var response =  await _client.RequestPayment(new PaymentRequestModel
                {
                    AccountDetails = new AccountDetails
                    {
                        AppProviderId = Program.Settings.SimplexWalletId,
                        AppVersionId = "1.0.0", //TODO: get somehow
                        AppEndUserId = profile.ExternalClientId,
                        AppInstallDate = DateTime.UtcNow, //TODO: get somehow,
                        Email = String.Empty,
                        Phone = String.Empty,
                        SignupLogin = new SignupLogin
                        {
                            Location = String.Empty,
                            Uaid = String.Empty,
                            AcceptLanguage = String.Empty,
                            HttpAcceptLanguage = String.Empty,
                            UserAgent = requestPaymentRequest.UserAgent,
                            CookieSessionId = String.Empty,
                            Timestamp = DateTime.UtcNow,
                            Ip = requestPaymentRequest.ClientIp,

                        }
                    },
                    TransactionDetails = new TransactionDetails
                    {
                        PaymentDetails = new PaymentDetails
                        {
                            QuoteId = quoteResponse.QuoteId,
                            PaymentId = paymentId,
                            OrderId = orderId,
                            DestinationWallet = new DestinationWallet
                            {
                                Currency = requestPaymentRequest.ToAsset,
                                Address = requestPaymentRequest.DepositAddress,
                                Tag = requestPaymentRequest.DepositTag ?? String.Empty
                            },
                            OriginalHttpRefUrl = "https://simple.app"
                        }
                    }
                });
                
                intention.Status = SimplexStatus.PaymentStarted;
                intention.OrderId = orderId;
                await context.UpsertAsync(new[] {intention});

                return new ExecuteQuoteResponse()
                {
                    IsSuccess = true,
                    PaymentLink = $"{Program.Settings.PaymentLink}{paymentId}"
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When getting simplex payment for clientId {clientId}, request {requestJson}", requestPaymentRequest.ClientId, requestPaymentRequest.ToJson());
                return new ()
                {
                    IsSuccess = false,
                    ErrorCode = e.Message
                };
            }
        }
        

    }
}
