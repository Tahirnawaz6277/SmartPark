namespace SmartPark.Dtos.Billing
{
    public record BillingDto
    {
        public Guid Id { get; set; }
        public decimal? Amount { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? TimeStamp { get; set; }
        public Guid? BookingId { get; set; }
    }
}
