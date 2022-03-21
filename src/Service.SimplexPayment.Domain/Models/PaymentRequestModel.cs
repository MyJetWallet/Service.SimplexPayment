using System;
using System.Text.Json.Serialization;

namespace Service.SimplexPayment.Domain.Models;

public class PaymentRequestModel
{
    [JsonPropertyName("account_details")] public AccountDetails AccountDetails { get; set; }

    [JsonPropertyName("transaction_details")] public TransactionDetails TransactionDetails { get; set; }
}

public class SignupLogin
{
    [JsonPropertyName("user_agent")] public string UserAgent { get; set; }
    
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }

    [JsonPropertyName("ip")] public string Ip { get; set; }
}

public class AccountDetails
{
    [JsonPropertyName("app_provider_id")] public string AppProviderId { get; set; }

    [JsonPropertyName("app_version_id")] public string AppVersionId { get; set; }

    [JsonPropertyName("app_end_user_id")] public string AppEndUserId { get; set; }

    [JsonPropertyName("app_install_date")] public DateTime AppInstallDate { get; set; }

    [JsonPropertyName("email")] public string Email { get; set; }

    [JsonPropertyName("phone")] public string Phone { get; set; }

    [JsonPropertyName("signup_login")] public SignupLogin SignupLogin { get; set; }
}

public class DestinationWallet
{
    [JsonPropertyName("currency")] public string Currency { get; set; }

    [JsonPropertyName("address")] public string Address { get; set; }

    [JsonPropertyName("tag")] public string Tag { get; set; }
}

public class PaymentDetails
{
    [JsonPropertyName("quote_id")] public string QuoteId { get; set; }

    [JsonPropertyName("payment_id")] public string PaymentId { get; set; }

    [JsonPropertyName("order_id")] public string OrderId { get; set; }

    [JsonPropertyName("destination_wallet")] public DestinationWallet DestinationWallet { get; set; }

    [JsonPropertyName("original_http_ref_url")] public string OriginalHttpRefUrl { get; set; }
}

public class TransactionDetails
{
    [JsonPropertyName("payment_details")] public PaymentDetails PaymentDetails { get; set; }
}