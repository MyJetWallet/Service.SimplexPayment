using System;
using System.Runtime.Serialization;
using Service.SimplexPayment.Domain.Models;

namespace Service.SimplexPayment.Grpc.Models;

[DataContract]
public class GetIntentionsRequest
{
    [DataMember(Order = 1)] public string SearchText { get; set; }
    [DataMember(Order = 2)] public DateTime LastSeen { get; set; }
    [DataMember(Order = 3)] public int Take { get; set; }
    [DataMember(Order = 4)] public SimplexStatus? Status { get; set; }
    [DataMember(Order = 5)] public string ClientId { get; set; }
    [DataMember(Order = 6)] public DateTime? CreationDateFrom { get; set; }
    [DataMember(Order = 7)] public DateTime? CreationDateTo { get; set; }
    [DataMember(Order = 8)] public string FromCurrency { get; set; }
    [DataMember(Order = 9)] public string ToAsset { get; set; }
}