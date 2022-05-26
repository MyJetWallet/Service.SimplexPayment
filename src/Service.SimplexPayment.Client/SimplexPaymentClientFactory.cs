using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using MyNoSqlServer.DataReader;
using Service.SimplexPayment.Domain.Models.NoSql;
using Service.SimplexPayment.Grpc;

namespace Service.SimplexPayment.Client
{
    [UsedImplicitly]
    public class SimplexPaymentClientFactory: MyGrpcClientFactory
    {
        private readonly MyNoSqlReadRepository<BuysInProgressNoSqlEntity> _reader;

        public SimplexPaymentClientFactory(string grpcServiceUrl, MyNoSqlReadRepository<BuysInProgressNoSqlEntity> reader) : base(grpcServiceUrl)
        {
            _reader = reader;
        }

        public ISimplexPaymentService GetSimplexService() => CreateGrpcService<ISimplexPaymentService>();

        public IInProgressBuysService GetInProgressClient() => _reader != null
                ? new InProgressBuysClient(CreateGrpcService<IInProgressBuysService>(), _reader)
                : CreateGrpcService<IInProgressBuysService>();

    }
}
