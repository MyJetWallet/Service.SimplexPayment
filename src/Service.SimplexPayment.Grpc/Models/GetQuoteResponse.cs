using System;
using System.Runtime.Serialization;
using Service.SimplexPayment.Domain.Models;

namespace Service.SimplexPayment.Grpc.Models
{
    [DataContract]
    public class GetQuoteResponse 
    {
        [DataMember(Order = 1)]
        public string QuoteId { get; set; }
        [DataMember(Order = 2)]
        public DateTime ValidUntil { get; set; }
        [DataMember(Order = 3)]
        public string FromCurrency { get; set; }
        [DataMember(Order = 4)]
        public decimal FromAmount { get; set; }
        [DataMember(Order = 5)]
        public string ToAsset { get; set; }
        [DataMember(Order = 6)]
        public decimal ToAmount { get; set; }
        [DataMember(Order = 7)]
        public bool IsSuccess { get; set; }
        [DataMember(Order = 8)]
        public string ErrorCode { get; set; }
    }
}