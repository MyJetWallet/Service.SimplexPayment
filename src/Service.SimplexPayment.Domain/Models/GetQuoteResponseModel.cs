using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Service.SimplexPayment.Domain.Models;

public class GetQuoteResponseModel
{
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("quote_id")]
    public string QuoteId { get; set; }

    [JsonPropertyName("wallet_id")]
    public string WalletId { get; set; }

    [JsonPropertyName("digital_money")]
    public DigitalMoney DigitalMoney { get; set; }

    [JsonPropertyName("fiat_money")]
    public FiatMoney FiatMoney { get; set; }

    [JsonPropertyName("valid_until")]
    public DateTime ValidUntil { get; set; }

    [JsonPropertyName("supported_digital_currencies")]
    public List<string> SupportedDigitalCurrencies { get; set; }
}
public class DigitalMoney
{
    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}

public class FiatMoney
{
    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("base_amount")]
    public decimal BaseAmount { get; set; }

    [JsonPropertyName("total_amount")]
    public decimal TotalAmount { get; set; }
}
