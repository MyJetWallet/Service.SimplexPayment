using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Service.SimplexPayment.Domain.Models;

public class GetEventsResponseModel
{
    [JsonPropertyName("events")]
    public List<SimplexEvent> Events { get; set; }
}
public class FiatTotalAmount
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }
}

public class CryptoTotalAmount
{
    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }
}

public class Payment
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("fiat_total_amount")]
    public FiatTotalAmount FiatTotalAmount { get; set; }

    [JsonPropertyName("crypto_total_amount")]
    public CryptoTotalAmount CryptoTotalAmount { get; set; }

    [JsonPropertyName("partner_end_user_id")]
    public string PartnerEndUserId { get; set; }
    
    [JsonPropertyName("blockchain_txn_hash")]
    public string BlockchainTxHash { get; set; }
}

public class SimplexEvent
{
    [JsonPropertyName("event_id")]
    public string EventId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("payment")]
    public Payment Payment { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}