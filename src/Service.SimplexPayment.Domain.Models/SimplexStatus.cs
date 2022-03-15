namespace Service.SimplexPayment.Domain.Models
{
    public enum SimplexStatus
    {
        QuoteCreated,
        QuoteConfirmed,
        PaymentStarted,
        PaymentApproved,
        Cancelled,
        PaymentCompleted,

    }
}