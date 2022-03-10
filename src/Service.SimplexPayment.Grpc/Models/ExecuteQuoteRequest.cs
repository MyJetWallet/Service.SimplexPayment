using System.Runtime.Serialization;

namespace Service.SimplexPayment.Grpc.Models
{
    [DataContract]
    public class ExecuteQuoteRequest
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
        [DataMember(Order = 6)]
        public string QuoteId { get; set; }
        [DataMember(Order = 7)]
        public string UserAgent { get; set; }
    }
}