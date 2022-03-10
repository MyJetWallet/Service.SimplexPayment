using System;
using System.Runtime.Serialization;
using Service.SimplexPayment.Domain.Models;

namespace Service.SimplexPayment.Grpc.Models
{
    [DataContract]
    public class ExecuteQuoteResponse 
    {
        [DataMember(Order = 1)]
        public string PaymentLink { get; set; }
        [DataMember(Order = 2)]
        public bool IsSuccess { get; set; }
        [DataMember(Order = 3)]
        public string ErrorCode { get; set; }
    }
}