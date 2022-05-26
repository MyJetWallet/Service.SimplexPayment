using System.Runtime.Serialization;

namespace Service.SimplexPayment.Grpc.Models
{
    [DataContract]
    public class InProgressRequest
    {
        [DataMember(Order = 1)]
        public string ClientId { get; set; }
        [DataMember(Order = 2)]
        public string Asset { get; set; }
    }
}