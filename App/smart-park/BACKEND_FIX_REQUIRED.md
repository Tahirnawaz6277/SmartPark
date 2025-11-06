# Backend API Fixes Required

## Issue: Location and Slot Names Showing N/A in Bookings

### Problem
When fetching bookings, the API returns N/A for Location and Slot fields because these navigation properties are not being populated.

### Current API Response
```json
{
  "id": "...",
  "status": "Booked",
  "startTime": "2025-11-06T09:29:00Z",
  "endTime": "2025-11-06T09:45:00Z",
  "userId": "...",
  "userName": "admin",
  "slotId": null,          // ❌ NULL
  "slotNumber": null,      // ❌ NULL
  "locationName": null     // ❌ NULL
}
```

### Required Backend Changes

#### Update Booking Entity / DTO Mapping
The backend needs to populate `locationName` and `slotNumber` when returning bookings.

**For C# Backend (ASP.NET Core):**

1. **Update the GetAllBookings endpoint** to include related entities:

```csharp
public async Task<List<BookingDto>> GetAllBookings()
{
    var bookings = await _context.Bookings
        .Include(b => b.BookingSlots)
            .ThenInclude(bs => bs.Slot)
            .ThenInclude(s => s.Location)
        .Include(b => b.User)
        .ToListAsync();

    return bookings.Select(b => new BookingDto
    {
        Id = b.Id,
        Status = b.Status,
        StartTime = b.StartTime,
        EndTime = b.EndTime,
        UserId = b.UserId,
        UserName = b.User?.Name,
        // Since booking now has MANY slots, you need to decide how to display:
        // Option 1: Show first slot
        SlotId = b.BookingSlots.FirstOrDefault()?.SlotId,
        SlotNumber = b.BookingSlots.FirstOrDefault()?.Slot?.SlotNumber,
        LocationName = b.BookingSlots.FirstOrDefault()?.Slot?.Location?.Name,
        // Option 2: Show comma-separated list of slots
        // SlotNumber = string.Join(", ", b.BookingSlots.Select(bs => bs.Slot.SlotNumber)),
        LastStatusSnapshot = b.LastStatusSnapshot
    }).ToList();
}
```

2. **Update GetMyBookings endpoint** similarly:

```csharp
public async Task<List<BookingDto>> GetMyBookings(string userId)
{
    var bookings = await _context.Bookings
        .Where(b => b.UserId == userId)
        .Include(b => b.BookingSlots)
            .ThenInclude(bs => bs.Slot)
            .ThenInclude(s => s.Location)
        .ToListAsync();

    return bookings.Select(b => new BookingDto
    {
        // Same mapping as above
    }).ToList();
}
```

### Alternative: Update BookingDto Structure

Since a booking now has MULTIPLE slots, consider returning a list of slots:

```csharp
public record BookingDto
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    
    // Multiple slots
    public List<BookingSlotDto> Slots { get; set; }
    
    // Or keep single fields for backward compatibility (showing first slot)
    public string SlotNumber { get; set; }
    public string LocationName { get; set; }
}

public record BookingSlotDto
{
    public Guid SlotId { get; set; }
    public string SlotNumber { get; set; }
    public string LocationName { get; set; }
}
```

### Frontend Update (if backend returns multiple slots)
If backend returns a list of slots, update the frontend components to display all slots:

```html
<!-- In booking table -->
<td>
  <span *ngFor="let slot of booking.slots; let last = last">
    {{ slot.slotNumber }}<span *ngIf="!last">, </span>
  </span>
</td>
```

---

## Summary
The Location and Slot N/A issue is a **backend problem**. The backend API needs to:
1. Include `.Include()` statements for related entities (Slot, Location)
2. Properly map these to the BookingDto
3. Decide how to handle multiple slots per booking in the response
