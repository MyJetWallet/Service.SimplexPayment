using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyNoSqlServer.Abstractions;
using Service.SimplexPayment.Domain.Models;
using Service.SimplexPayment.Domain.Models.NoSql;
using Service.SimplexPayment.Grpc;
using Service.SimplexPayment.Grpc.Models;
using Service.SimplexPayment.Postgres;

namespace Service.SimplexPayment.Services
{
    public class InProgressBuysService : IInProgressBuysService
    {
        private readonly ILogger<InProgressBuysService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IMyNoSqlServerDataWriter<BuysInProgressNoSqlEntity> _writer;

        private static readonly List<SimplexStatus> InProgressStatuses = new()
            {SimplexStatus.PaymentSubmitted, SimplexStatus.PaymentApproved, SimplexStatus.CryptoSent};

        public InProgressBuysService(ILogger<InProgressBuysService> logger, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IMyNoSqlServerDataWriter<BuysInProgressNoSqlEntity> writer)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _writer = writer;
        }

        public async Task<InProgressResponse> GetInProgressBuys(InProgressRequest request)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var intentions = await context.Intentions.Where(t => t.ToAsset == request.Asset && t.ClientId == request.ClientId && InProgressStatuses.Contains(t.Status) ).ToListAsync();
                if (intentions.Any())
                {
                    var total = intentions.Sum(t => t.ToAmount);
                    var count = intentions.Count;

                    await _writer.InsertOrReplaceAsync(
                        BuysInProgressNoSqlEntity.Create(request.ClientId, request.Asset, total, count));
                    
                    return new InProgressResponse
                    {
                        TotalAmount = total,
                        TxCount = count
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When calculating in-progress buys for request {request}", request.ToJson());
            }
            
            return new InProgressResponse()
            {
                TotalAmount = 0,
                TxCount = 0
            };
        }
    }
}