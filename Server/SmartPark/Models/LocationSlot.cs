using System;
using System.Collections.Generic;

namespace SmartPark.Models;

public partial class LocationSlot
{
    public Guid LocationId { get; set; }

    public Guid SlotId { get; set; }

    public int SlotCount { get; set; }

    public virtual ParkingLocation Location { get; set; } = null!;

    public virtual Slot Slot { get; set; } = null!;
}
