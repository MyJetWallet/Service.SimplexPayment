using System.ServiceModel;
using System.Threading.Tasks;
using Service.SimplexPayment.Grpc.Models;

namespace Service.SimplexPayment.Grpc
{
    [ServiceContract]
    public interface IInProgressBuysService
    {
        [OperationContract]
        Task<InProgressResponse> GetInProgressBuys(InProgressRequest request);
    }
}