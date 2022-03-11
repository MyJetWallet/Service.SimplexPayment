using System;

namespace Service.SimplexPayment.Domain.Models
{
    public class SimplexIntention
    {
        public string QuoteId { get; set; }
        public string ClientId { get; set; }
        public string ClientIdHash { get; set; }
        public decimal FromAmount { get; set; }
        public string FromCurrency { get; set; }
        public decimal ToAmount { get; set; }
        public string ToAsset { get; set; }
        public string ClientIp { get; set; }
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public DateTime CreationTime { get; set; }
        public string ErrorText { get; set; }
        public SimplexStatus Status { get; set; }
    }
}