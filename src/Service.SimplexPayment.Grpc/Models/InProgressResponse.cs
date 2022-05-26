using System;
using System.Runtime.Serialization;
using Service.SimplexPayment.Domain.Models;

namespace Service.SimplexPayment.Grpc.Models
{
    [DataContract]
    public class InProgressResponse 
    {
        [DataMember(Order = 1)]
        public decimal TotalAmount { get; set; }
        [DataMember(Order = 2)]
        public int TxCount { get; set; }
    }
}