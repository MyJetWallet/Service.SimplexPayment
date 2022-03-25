using System;
using System.Runtime.Serialization;

namespace Service.SimplexPayment.Domain.Models
{
    [DataContract]
    public class SimplexIntention
    {
        public const string TopicName = "jetwallet-simplex-intention";
        [DataMember(Order = 1)]public string QuoteId { get; set; }
        [DataMember(Order = 2)]public string ClientId { get; set; }
        [DataMember(Order = 3)]public string ClientIdHash { get; set; }
        [DataMember(Order = 4)]public decimal FromAmount { get; set; }
        [DataMember(Order = 5)]public string FromCurrency { get; set; }
        [DataMember(Order = 6)]public decimal ToAmount { get; set; }
        [DataMember(Order = 7)] public string ToAsset { get; set; }
        [DataMember(Order = 8)]public string ClientIp { get; set; }
        [DataMember(Order = 9)]public string PaymentId { get; set; }
        [DataMember(Order = 10)]public string OrderId { get; set; }
        [DataMember(Order = 11)]public DateTime CreationTime { get; set; }
        [DataMember(Order = 12)]public string ErrorText { get; set; }
        [DataMember(Order = 13)]public SimplexStatus Status { get; set; }
        [DataMember(Order = 14)]public string BlockchainTxHash { get; set; }
        [DataMember(Order = 15)]public decimal TotalFiatAmount { get; set; }
        [DataMember(Order = 16)]public decimal BaseFiatAmount { get; set; }
        [DataMember(Order = 17)]public decimal Fee { get; set; }
        [DataMember(Order = 18)]public decimal ReceivedAmount { get; set; }
        [DataMember(Order = 19)]public decimal BlockchainFee { get; set; }
        
        [DataMember(Order = 20)] public string WalletId { get; set; }
    }
}