using System.ServiceModel;
using System.Threading.Tasks;
using Service.SimplexPayment.Grpc.Models;

namespace Service.SimplexPayment.Grpc
{
    [ServiceContract]
    public interface ISimplexPaymentService
    {
        [OperationContract]
        Task<ExecuteQuoteResponse> RequestPayment(RequestPaymentRequest requestPaymentRequest);
        
        [OperationContract]
        Task<GetIntentionsResponse> GetIntentions(GetIntentionsRequest request);

        [OperationContract]
        Task<IntentionsInProgressResponse> CheckIntentionsInProgress(IntentionsInProgressRequest request);
    }
}