using MyNoSqlServer.Abstractions;

namespace Service.SimplexPayment.Domain.Models
{
    public class DepositAddressNoSqlEntity: MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-simplex-depositaddresses";

        public static string GeneratePartitionKey(string accountId) => accountId;
        public static string GenerateRowKey(string symbol) => symbol;

        public string Address { get; set; }
        public string Tag { get; set; }

        public static DepositAddressNoSqlEntity Create(string accountId, string symbol, string address, string tag)
        {
            return new DepositAddressNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(accountId),
                RowKey = GenerateRowKey(symbol),
                Address = address,
                Tag = tag
            };
        }
    }
}