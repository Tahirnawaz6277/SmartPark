namespace SmartPark.Models;

public partial class BookingHistory
{
    public Guid Id { get; set; }

    public int? Duration { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public string? StatusSnapshot { get; set; }

    public bool? IsArchived { get; set; }

    public DateTime? TimeStamp { get; set; }

    public Guid? SlotId { get; set; }

    public Guid? BookingId { get; set; }

    public Guid? UserId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Slot? Slot { get; set; }

    public virtual User? User { get; set; }
}
