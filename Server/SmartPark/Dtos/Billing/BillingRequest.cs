using System.ComponentModel.DataAnnotations;

namespace SmartPark.Dtos.Billing
{
    public record BillingRequest
    {
        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }

        //[Required(ErrorMessage = "PaymentStatus is required")]
        //public string PaymentStatus { get; set; } = null!; // e.g., "Paid", "Pending"

        //[Required(ErrorMessage = "PaymentMethod is required")]
        //[RegularExpression("^(Cash|Card|Online)$", ErrorMessage = "Invalid payment method.")]
        //public string PaymentMethod { get; set; } = "Cash"; // default to Cash

        [Required(ErrorMessage = "BookingId is required")]
        public Guid BookingId { get; set; }
    }
}
