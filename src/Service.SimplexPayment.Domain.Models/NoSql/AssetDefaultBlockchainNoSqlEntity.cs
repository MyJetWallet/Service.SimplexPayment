using System;
using System.Collections.Generic;
using MyNoSqlServer.Abstractions;

namespace Service.SimplexPayment.Domain.Models.NoSql
{
    public class AssetDefaultBlockchainNoSqlEntity: MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-simplex-asset-default-blockchain";

        public static string GeneratePartitionKey() => "Default";
        public static string GenerateRowKey(string assetId) => assetId;

        public string AssetId { get; set; }
        public string AssetNetwork { get; set; }

        public static AssetDefaultBlockchainNoSqlEntity Create(string assetId, string assetNetwork)
        {
            return new AssetDefaultBlockchainNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(assetId),
                AssetId = assetId,
                AssetNetwork = assetNetwork,
            };
        }
    }
}