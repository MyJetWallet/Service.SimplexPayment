using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Service.SimplexPayment.Domain.Models;

public class GetQuoteRequestModel
{
    [JsonPropertyName("end_user_id")]
    public string EndUserId { get; set; }

    [JsonPropertyName("digital_currency")]
    public string DigitalCurrency { get; set; }

    [JsonPropertyName("fiat_currency")]
    public string FiatCurrency { get; set; }

    [JsonPropertyName("requested_currency")]
    public string RequestedCurrency { get; set; }

    [JsonPropertyName("requested_amount")]
    public decimal RequestedAmount { get; set; }

    [JsonPropertyName("wallet_id")]
    public string WalletId { get; set; }

    [JsonPropertyName("client_ip")]
    public string ClientIp { get; set; }

    [JsonPropertyName("payment_methods")]
    public List<string> PaymentMethods { get; set; }
}