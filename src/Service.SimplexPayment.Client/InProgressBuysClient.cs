using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;
using Service.SimplexPayment.Domain.Models.NoSql;
using Service.SimplexPayment.Grpc;
using Service.SimplexPayment.Grpc.Models;

namespace Service.SimplexPayment.Client;

public class InProgressBuysClient : IInProgressBuysService
{
    private readonly IInProgressBuysService _grpcService;
    private readonly IMyNoSqlServerDataReader<BuysInProgressNoSqlEntity> _reader;

    public InProgressBuysClient(IInProgressBuysService grpcService, IMyNoSqlServerDataReader<BuysInProgressNoSqlEntity> reader)
    {
        _grpcService = grpcService;
        _reader = reader;
    }

    public async Task<InProgressResponse> GetInProgressBuys(InProgressRequest request)
    {
        var entity = _reader.Get(BuysInProgressNoSqlEntity.GeneratePartitionKey(request.ClientId),
            BuysInProgressNoSqlEntity.GenerateRowKey(request.Asset));
        if (entity != null)
            return new InProgressResponse
            {
                TotalAmount = entity.TotalAmount,
                TxCount = entity.Count
            };

        return await _grpcService.GetInProgressBuys(request);
    }
}