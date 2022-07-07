using System;
using System.Threading.Tasks;
using Service.SimplexPayment.Grpc;
using Service.SimplexPayment.Grpc.Models;

namespace Service.SimplexPayment.Services
{
    public class SimplexPaymentServiceGrpc: ISimplexPaymentService
    {
        private readonly SimplexPaymentService _simplexPaymentService;

        public SimplexPaymentServiceGrpc(SimplexPaymentService simplexPaymentService)
        {
            _simplexPaymentService = simplexPaymentService;
        }

        public async Task<ExecuteQuoteResponse> RequestPayment(RequestPaymentRequest requestPaymentRequest) 
            => await _simplexPaymentService.RequestPayment(requestPaymentRequest);

        public async Task<GetIntentionsResponse> GetIntentions(GetIntentionsRequest request)
        {
            try
            {
                var list = await _simplexPaymentService.GetIntentions(request);
                return new GetIntentionsResponse
                {
                    Intentions = list
                };
            }
            catch (Exception ex)
            {
                return new GetIntentionsResponse
                {
                    IsError = true,
                    ErrorMessage = ex.Message
                };
            }

        }

        public async Task<IntentionsInProgressResponse> CheckIntentionsInProgress(IntentionsInProgressRequest request) => await _simplexPaymentService.CheckIntentionsInProgress(request);
    }
}
