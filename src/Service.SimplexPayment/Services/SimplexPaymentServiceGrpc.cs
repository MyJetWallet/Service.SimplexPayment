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
            var list = await _simplexPaymentService.GetIntentions(request.Take, request.LastSeen, request.SearchText);
            return new GetIntentionsResponse
            {
                Intentions = list
            };
        }
    }
}
