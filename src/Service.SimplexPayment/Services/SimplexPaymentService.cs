using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        public SimplexPaymentService(ILogger<SimplexPaymentService> logger, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization",$"ApiKey {Program.Settings.SimplexApiKey}");
        }

        public async Task<GetQuoteResponse> GetQuote(GetQuoteRequest request)
        {
            try
            {
                var link = Program.Settings.ProductionMode ? $"{ProdBase}{GetQuotePath}" : $"{SandboxBase}{GetQuotePath}";
                var clientIdHash = GetStringSha256Hash(request.ClientId);
                var response = await PostRequest<GetQuoteResponseModel, GetQuoteRequestModel>(link,
                    new GetQuoteRequestModel
                    {
                        EndUserId = clientIdHash,
                        DigitalCurrency = request.ToAsset,
                        FiatCurrency = request.FromCurrency,
                        RequestedCurrency = request.FromCurrency,
                        RequestedAmount = request.FromAmount,
                        WalletId = Program.Settings.SimplexWalletId,
                        ClientIp = request.ClientIp,
                        PaymentMethods = new() {"credit_card"}
                    });

                var intention = new SimplexIntention
                {
                    QuoteId = response.QuoteId,
                    ClientId = request.ClientId,
                    ClientIdHash = clientIdHash,
                    FromAmount = request.FromAmount,
                    FromCurrency = request.FromCurrency,
                    ToAmount = response.DigitalMoney.Amount,
                    ToAsset = response.DigitalMoney.Currency,
                    ClientIp = request.ClientIp,
                    CreationTime = DateTime.UtcNow,
                    Status = SimplexStatus.QuoteCreated
                };
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.UpsertAsync(new[] {intention});

                return new GetQuoteResponse
                {
                    QuoteId = response.QuoteId,
                    ValidUntil = response.ValidUntil,
                    FromCurrency = response.FiatMoney.Currency,
                    FromAmount = response.FiatMoney.TotalAmount,
                    ToAsset = response.DigitalMoney.Currency,
                    ToAmount = response.DigitalMoney.Amount,
                    IsSuccess = true,
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When getting simplex quote for clientId {clientId}", request.ClientId);
                return new GetQuoteResponse()
                {
                    IsSuccess = false,
                    ErrorCode = e.Message
                };
            }
        }

        public async Task<ExecuteQuoteResponse> ExecuteQuote(ExecuteQuoteRequest request)
        {
            try
            {
                var link = Program.Settings.ProductionMode ? $"{ProdBase}{RequestPaymentPath}" : $"{SandboxBase}{RequestPaymentPath}";
                var clientIdHash = GetStringSha256Hash(request.ClientId);
                var paymentId = Guid.NewGuid().ToString("D");
                
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var intention = await context.Intentions.FirstOrDefaultAsync(t => t.QuoteId == request.QuoteId);
                if (intention == null)
                    return new ExecuteQuoteResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "Intention not found"
                    };

                intention.PaymentId = paymentId;
                intention.Status = SimplexStatus.QuoteConfirmed;
                await context.UpsertAsync(new[] {intention});
                
                var response = await PostRequest<PaymentResponseModel, PaymentRequestModel>(link, new PaymentRequestModel
                {
                    AccountDetails = new AccountDetails
                    {
                        AppProviderId = Program.Settings.SimplexWalletId,
                        AppVersionId = null,
                        AppEndUserId = clientIdHash,
                        AppInstallDate = default,
                        SignupLogin = new SignupLogin
                        {
                            UserAgent = request.UserAgent,
                            Timestamp = DateTime.UtcNow,
                            Ip = request.ClientIp
                        }
                    },
                    TransactionDetails = new TransactionDetails
                    {
                        PaymentDetails = new PaymentDetails
                        {
                            QuoteId = request.QuoteId,
                            PaymentId = paymentId,
                            OrderId = Guid.NewGuid().ToString("D"),
                            DestinationWallet = new DestinationWallet
                            {
                                Currency = null,
                                Address = null,
                                Tag = null
                            },
                            OriginalHttpRefUrl = "https://simple.app"
                        }
                    }
                });
                
                intention.Status = SimplexStatus.PaymentStarted;
                await context.UpsertAsync(new[] {intention});

                return new ExecuteQuoteResponse()
                {
                    IsSuccess = true,
                    PaymentLink = $"{Program.Settings.PaymentLink}{paymentId}"
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When getting simplex quote for clientId {clientId}", request.ClientId);
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
