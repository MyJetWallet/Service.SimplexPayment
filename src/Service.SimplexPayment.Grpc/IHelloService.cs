using System.ServiceModel;
using System.Threading.Tasks;
using Service.SimplexPayment.Grpc.Models;

namespace Service.SimplexPayment.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}