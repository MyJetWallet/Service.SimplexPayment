using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.ClientProfile.Grpc;
using Service.ClientProfile.Grpc.Models.Requests;
using Service.PersonalData.Grpc;
using Service.PersonalData.Grpc.Contracts;
using Service.SimplexPayment.Domain.Models;
using Service.SimplexPayment.Grpc.Models;
using Service.SimplexPayment.Postgres;

namespace Service.SimplexPayment.Services
{
    public class SimplexPaymentService
    {
        private readonly ILogger<SimplexPaymentService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IClientProfileService _clientProfile;
        private readonly SimplexHttpClient _client;
        private readonly IPersonalDataServiceGrpc _personalData;
        public SimplexPaymentService(ILogger<SimplexPaymentService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IClientProfileService clientProfile, SimplexHttpClient client, IPersonalDataServiceGrpc personalData)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _clientProfile = clientProfile;
            _client = client;
            _personalData = personalData;
        }

        public async Task<ExecuteQuoteResponse> RequestPayment(RequestPaymentRequest requestPaymentRequest)
        {
            try
            {
                var profile = await _clientProfile.GetOrCreateProfile(new GetClientProfileRequest
                {
                    ClientId = requestPaymentRequest.ClientId
                });

                var pd = await _personalData.GetByIdAsync(new GetByIdRequest()
                {
                    Id = requestPaymentRequest.ClientId
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
                        AppInstallDate = pd.PersonalData.CreatedAt, 
                        Email = pd.PersonalData.Email,
                        Phone = pd.PersonalData.Phone,
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
