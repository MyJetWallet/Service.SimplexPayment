using System;
using System.Collections.Generic;
using MyNoSqlServer.Abstractions;

namespace Service.SimplexPayment.Domain.Models.NoSql
{
    public class BuysInProgressNoSqlEntity: MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-simplex-buysinprogress";

        public static string GeneratePartitionKey(string clientId) => clientId;
        public static string GenerateRowKey(string assetId) => assetId;

        public decimal TotalAmount;
        public int Count;
        
        public static BuysInProgressNoSqlEntity Create(string clientId, string assetId, decimal amount, int count)
        {
            return new BuysInProgressNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(clientId),
                RowKey = GenerateRowKey(assetId),
                TotalAmount = amount, 
                Count = count,
                Expires = DateTime.UtcNow.AddDays(1)
            };
        }
    }
}