namespace Service.SimplexPayment.Domain.Models
{
    public enum SimplexStatus
    {
        QuoteCreated = 0,
        PaymentSubmitted = 10, //payment_request_submitted
        PaymentApproved = 20, //payment_simplexcc_approved
        Declined = 30, //payment_simplexcc_declined
        Refunded = 40, //payment_simplexcc_refunded
        CryptoSent = 50, //payment_simplexcc_crypto_sent
    }
}