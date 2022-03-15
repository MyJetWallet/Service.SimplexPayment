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
            //4EE2EE41835266AF983D8F3CD12723767DC280F627A51E320AA5C22BFC3E3B8B3896CE29377652E639D1EC583EF6F47FD0350A76BA6556808A55C493978786EE3D9777C619F65AC3DB4333A984AD9FDF91166ACEED3D251952D2C889B96B3CAF4AB6092E8FD505ACF719622C7E5E762090FF5C55FEA56509B7E45B0E5547CE822C8E1BFB10DFF944ED44AD76E159AA27C4E5B4FB2C60E42DC9DE6124F862CBF402B573E0053C08F6035C982423DEF1ED
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
