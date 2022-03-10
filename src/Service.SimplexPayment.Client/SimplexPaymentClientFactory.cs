using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.SimplexPayment.Grpc;

namespace Service.SimplexPayment.Client
{
    [UsedImplicitly]
    public class SimplexPaymentClientFactory: MyGrpcClientFactory
    {
        public SimplexPaymentClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public ISimplexPaymentService GetHelloService() => CreateGrpcService<ISimplexPaymentService>();
    }
}
