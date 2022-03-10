using System.Runtime.Serialization;

namespace Service.SimplexPayment.Grpc.Models
{
    [DataContract]
    public class GetQuoteRequest
    {
        [DataMember(Order = 1)]
        public string ClientId { get; set; }
        [DataMember(Order = 2)]
        public string ClientIp { get; set; }
        [DataMember(Order = 3)]
        public string FromCurrency { get; set; }
        [DataMember(Order = 4)]
        public decimal FromAmount { get; set; }
        [DataMember(Order = 5)]
        public string ToAsset { get; set; }
    }
}