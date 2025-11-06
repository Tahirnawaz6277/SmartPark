export interface BookingCreateRequest {
  startTime: string; // ISO date-time string
  endTime: string;   // ISO date-time string
  slotIds: string[];    // Array of UUIDs for multiple slots
}

export interface BookingDto {
  id: string;
  status?: string;
  startTime?: string;
  endTime?: string;
  userId?: string;
  userName?: string;
  slotId?: string;
  slotNumber?: string;
  slotNumbers?: string;  // Multiple slot numbers (e.g., "S2-21, S1-111, S1-112")
  locationName?: string;
  lastStatusSnapshot?: string;
}

export interface BookingUpdateRequest {
  startTime: string;
  endTime: string;
  slotIds: string[];  // Array of UUIDs for multiple slots
}

export interface BookingHistoryDto {
  id: string;
  statusSnapshot?: string;
  slotId?: string;
  bookingId?: string;
  userId?: string;
  statTime?: string;  // Note: API has typo "statTime"
  endTime?: string;
  timeStamp?: string;
}
