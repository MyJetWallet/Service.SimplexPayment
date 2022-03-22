using System;
using System.Runtime.Serialization;

namespace Service.SimplexPayment.Grpc.Models;

[DataContract]
public class GetIntentionsRequest
{
    [DataMember(Order = 1)]
    public string SearchText { get; set; }
    [DataMember(Order = 2)]
    public DateTime LastSeen { get; set; }
    [DataMember(Order = 3)]
    public int Take { get; set; }
}