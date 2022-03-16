using System.Collections.Generic;
using MyNoSqlServer.Abstractions;

namespace Service.SimplexPayment.Domain.Models.NoSql
{
    public class SimplexEventsNoSqlEntity: MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-simplex-events";

        public static string GeneratePartitionKey() => "Events";
        public static string GenerateRowKey() => "Events";

        public List<SimplexEvent> Events { get; set; }

        public static SimplexEventsNoSqlEntity Create(List<SimplexEvent> events)
        {
            return new SimplexEventsNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(),
                Events = events
            };
        }
    }
}