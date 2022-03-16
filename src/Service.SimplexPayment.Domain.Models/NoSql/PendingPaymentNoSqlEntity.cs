using System.Collections.Generic;
using MyNoSqlServer.Abstractions;

namespace Service.SimplexPayment.Domain.Models.NoSql
{
    public class PendingPaymentNoSqlEntity: MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-simplex-pendingpayments";

        public static string GeneratePartitionKey() => "PendingPayments";
        public static string GenerateRowKey(string clientId) => clientId;

        public Dictionary<string, decimal> PendingBalances;

        public static PendingPaymentNoSqlEntity Create(string clientId, Dictionary<string, decimal> pendingBalances)
        {
            return new PendingPaymentNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(clientId),
                PendingBalances = pendingBalances
            };
        }
    }
}