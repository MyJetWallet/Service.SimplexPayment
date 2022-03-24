using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.SimplexPayment.Domain.Models;

namespace Service.SimplexPayment.Grpc.Models;

[DataContract]
public class IntentionsInProgressRequest
{
    [DataMember(Order = 1)]
    public string ClientId { get; set; }
    
    [DataMember(Order = 2)]
    public string AssetId { get; set; }
    
    [DataMember(Order = 3)]
    public string TxId { get; set; }
    
    [DataMember(Order = 4)]
    public decimal ReceivedVolume { get; set; }
}