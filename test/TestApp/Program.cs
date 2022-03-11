using System;
using System.Threading.Tasks;
using MyNoSqlServer.DataWriter;
using ProtoBuf.Grpc.Client;
using Service.SimplexPayment.Client;
using Service.SimplexPayment.Domain.Models;
using Service.SimplexPayment.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();
            // http://192.168.70.80:5123

            var writer = new MyNoSqlServerDataWriter<DepositAddressNoSqlEntity>(() => "http://192.168.70.80:5123", DepositAddressNoSqlEntity.TableName, true);
            var accountId = "97";
            
            await writer.InsertOrReplaceAsync(DepositAddressNoSqlEntity.Create(accountId, "BTC", "tb1q2xmz43p6uvlsslplwuv3d9034x9f2rhg8cz22d", null));
            await writer.InsertOrReplaceAsync(DepositAddressNoSqlEntity.Create(accountId, "XRP", "r3TsxjBWPVV8Sbk4Q2vpt59jKRUViTjZRB", "1147442382"));
            await writer.InsertOrReplaceAsync(DepositAddressNoSqlEntity.Create(accountId, "XLM", "GA5J5BISNT45WOWRHKYQW7LCO3R56TR7YHVMRNBBPRQWF3YPQFMO6BWG", "3941159564"));
            await writer.InsertOrReplaceAsync(DepositAddressNoSqlEntity.Create(accountId, "ETH", "0x6c53728aA6D5dAC8667dd62Af741B102C2050e0A", null));
            await writer.InsertOrReplaceAsync(DepositAddressNoSqlEntity.Create(accountId, "LTC", "tltc1q2xmz43p6uvlsslplwuv3d9034x9f2rhg7sq56y", null));

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
