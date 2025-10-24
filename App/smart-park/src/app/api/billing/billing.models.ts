export interface BillingCreateRequest {
  amount: number;
  bookingId: string;
}

export interface BillingDto {
  id: string;
  amount?: number;
  paymentStatus?: string;
  paymentMethod?: string;
  timeStamp?: string;
  bookingId?: string;
  userName?: string;
  slotNumber?: string;
  locationName?: string;
}

export interface BillingUpdateRequest {
  amount?: number;
  bookingId?: string;
}
