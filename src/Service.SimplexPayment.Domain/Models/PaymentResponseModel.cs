using System.Text.Json.Serialization;

namespace Service.SimplexPayment.Domain.Models;

public class PaymentResponseModel
{
    [JsonPropertyName("is_kyc_update_required")]
    public bool IsKycUpdateRequired { get; set; }
}