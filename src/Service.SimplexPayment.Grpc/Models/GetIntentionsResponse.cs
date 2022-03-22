using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.SimplexPayment.Domain.Models;

namespace Service.SimplexPayment.Grpc.Models;

[DataContract]
public class GetIntentionsResponse
{
    [DataMember(Order = 1)]
    public List<SimplexIntention> Intentions { get; set; }
}