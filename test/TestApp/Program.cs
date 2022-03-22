using System;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using SimpleTrading.Common.Helpers;
using HexConverterUtils = MyJetWallet.Sdk.Service.HexConverterUtils;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();
            
            var apiKey = "";
            var encodingKey = "";
            Console.WriteLine(EncodeString(apiKey, encodingKey));
            Console.WriteLine("End");
            Console.ReadLine();
        }
        public static string EncodeString(string str, string keyStr)
        {
            var key = Encoding.UTF8.GetBytes(keyStr);
            var data = Encoding.UTF8.GetBytes(str);

            var result = AesEncodeDecode.Encode(data, key);
            
            return HexConverterUtils.ToHexString(result);
        }
    }
}
