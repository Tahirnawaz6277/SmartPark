namespace SmartPark.Dtos.Billing
{
    public record BillingResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public DateTime TimeStamp { get; set; }
        public Guid BookingId { get; set; }
    }
}
