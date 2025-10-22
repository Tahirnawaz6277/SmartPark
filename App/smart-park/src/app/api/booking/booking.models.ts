export interface BookingCreateRequest {
  startTime: string; // ISO date-time string
  endTime: string;   // ISO date-time string
  slotId: string;    // UUID
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
  lastStatusSnapshot?: string;
}

export interface BookingUpdateRequest {
  startTime: string;
  endTime: string;
  slotId: string;
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
