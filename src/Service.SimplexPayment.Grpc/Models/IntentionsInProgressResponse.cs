using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.SimplexPayment.Domain.Models;

namespace Service.SimplexPayment.Grpc.Models;

[DataContract]
public class IntentionsInProgressResponse
{
    [DataMember(Order = 1)]
    public bool ShouldWait { get; set; }
    
    [DataMember(Order = 2)]
    public SimplexIntention MatchedIntention { get; set; }
}