using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.SimplexPayment.Domain;
using Service.SimplexPayment.Domain.Models;
using Service.SimplexPayment.Grpc;
using Service.SimplexPayment.Grpc.Models;
using Service.SimplexPayment.Postgres;
using Service.SimplexPayment.Settings;

namespace Service.SimplexPayment.Services
{
    public class SimplexPaymentService: ISimplexPaymentService
    {
        private const string SandboxBase = "https://sandbox.test-simplexcc.com/";
        private const string ProdBase = "https://backend-wallet-api.simplexcc.com/";
        private const string GetQuotePath = "wallet/merchant/v2/quote";
        private const string RequestPaymentPath = "wallet/merchant/v2/payments/partner/data";

        
        private readonly ILogger<SimplexPaymentService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly HttpClient _client;
        private readonly IDepositAddressRepository _addressRepository;
        public SimplexPaymentService(ILogger<SimplexPaymentService> logger, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IDepositAddressRepository addressRepository)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _addressRepository = addressRepository;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization",$"ApiKey {Program.Settings.SimplexApiKey}");
        }

        public async Task<ExecuteQuoteResponse> RequestPayment(RequestPaymentRequest requestPaymentRequest)
        {
            try
            {
                var quoteLink = Program.Settings.ProductionMode ? $"{ProdBase}{GetQuotePath}" : $"{SandboxBase}{GetQuotePath}";
                var clientIdHash = GetStringSha256Hash(requestPaymentRequest.ClientId);
                var quoteResponse = await PostRequest<GetQuoteResponseModel, GetQuoteRequestModel>(quoteLink,
                    new GetQuoteRequestModel
                    {
                        EndUserId = clientIdHash,
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
                    ClientIdHash = clientIdHash,
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
               
                
                var paymentLink = Program.Settings.ProductionMode ? $"{ProdBase}{RequestPaymentPath}" : $"{SandboxBase}{RequestPaymentPath}";
                var paymentId = Guid.NewGuid().ToString("D");
                var orderId = Guid.NewGuid().ToString("D");

                intention.PaymentId = paymentId;
                intention.Status = SimplexStatus.QuoteConfirmed;
                await context.UpsertAsync(new[] {intention});
                
                var response = await PostRequest<PaymentResponseModel, PaymentRequestModel>(paymentLink, new PaymentRequestModel
                {
                    AccountDetails = new AccountDetails
                    {
                        AppProviderId = Program.Settings.SimplexWalletId,
                        AppVersionId = "1.0.0", //TODO: get somehow
                        AppEndUserId = clientIdHash,
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
        
        private async Task<T1> PostRequest<T1, T2>(string uri, T2 request)
        {
            var responseMessage = String.Empty;
            try
            {
                using var response = await _client.PostAsJsonAsync<T2>(uri, request);
                responseMessage = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T1>(responseBody);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When executing request to path {path}. Response {response}", uri, responseMessage);
                throw;
            }
        }
        
        private static string GetStringSha256Hash(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using var sha = System.Security.Cryptography.SHA256.Create();
            byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hash = sha.ComputeHash(textData);
            return BitConverter.ToString(hash).Replace("-", String.Empty);
        }
    }
}
