using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.SimplexPayment.Domain.Models;
using Service.SimplexPayment.Grpc.Models;
using SimpleTrading.Common.Helpers;
using HexConverterUtils = MyJetWallet.Sdk.Service.HexConverterUtils;

namespace Service.SimplexPayment.Services
{
    public class SimplexHttpClient
    {
        private const string SandboxBase = "https://sandbox.test-simplexcc.com/";
        private const string ProdBase = "https://backend-wallet-api.simplexcc.com/";
        private const string GetQuotePath = "wallet/merchant/v2/quote";
        private const string RequestPaymentPath = "wallet/merchant/v2/payments/partner/data";
        private const string EventsPath = "wallet/merchant/v2/events";
        
        private readonly HttpClient _client;
        private readonly ILogger<SimplexHttpClient> _logger;

        public SimplexHttpClient(ILogger<SimplexHttpClient> logger)
        {
            _logger = logger;
            
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization",$"ApiKey {DecodeKey(Program.Settings.SimplexApiKey)}");
        }


        public async Task<GetQuoteResponseModel> GetQuote(GetQuoteRequestModel request)
        {
            var quoteLink = Program.Settings.ProductionMode ? $"{ProdBase}{GetQuotePath}" : $"{SandboxBase}{GetQuotePath}";
            return await PostRequest<GetQuoteResponseModel, GetQuoteRequestModel>(quoteLink, request);
        }

        public async Task<PaymentResponseModel> RequestPayment(PaymentRequestModel request)
        {
            var paymentLink = Program.Settings.ProductionMode ? $"{ProdBase}{RequestPaymentPath}" : $"{SandboxBase}{RequestPaymentPath}";
            return await PostRequest<PaymentResponseModel, PaymentRequestModel>(paymentLink, request);
        }

        public async Task<GetEventsResponseModel> GetEvents()
        {
            var eventsLink = Program.Settings.ProductionMode ? $"{ProdBase}{EventsPath}" : $"{SandboxBase}{EventsPath}";
            return await GetRequest<GetEventsResponseModel>(eventsLink);
        }

        public async Task DeleteEvent(string eventId)
        {            
            var eventsLink = Program.Settings.ProductionMode ? $"{ProdBase}{EventsPath}/{eventId}" : $"{SandboxBase}{EventsPath}/{eventId}";
            await DeleteRequest(eventsLink);
        }

        private async Task<T1> PostRequest<T1, T2>(string uri, T2 request)
        {
            var responseMessage = String.Empty;
            try
            {
                using var response = await _client.PostAsJsonAsync<T2>(uri, request);
                responseMessage = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Sending request to {requestUrl} with body {requestJson}, {responseJson}", uri, request.ToJson(), responseMessage);
                
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
        
        
        private async Task<T1> GetRequest<T1>(string uri)
        {
            var responseMessage = String.Empty;
            try
            {
                using var response = await _client.GetAsync(uri);
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
        
        private async Task DeleteRequest(string uri)
        {
            var responseMessage = String.Empty;
            try
            {
                using var response = await _client.DeleteAsync(uri);
                responseMessage = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When executing request to path {path}. Response {response}", uri, responseMessage);
                throw;
            }
        }

        private static string DecodeKey(string apiKey)
        {
            var data = HexConverterUtils.HexStringToByteArray(apiKey);
            return Encoding.UTF8.GetString(AesEncodeDecode.Decode(data, Program.EncodingKey));
        }
    }
}