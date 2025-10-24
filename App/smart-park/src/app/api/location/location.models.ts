export interface LocationCreateRequest {
  name: string;
  address: string;
  totalSlots: number;
  city: string;
  imageFile?: string;
}

export interface SlotDto {
  id: string;
  locationId?: string;
  slotNumber?: string;
  isAvailable?: boolean;
}

export interface LocationDto {
  id: string;
  name?: string;
  address?: string;
  totalSlots?: number;
  city?: string;
  image?: string;
  imageUrl?: string;
  imageExtension?: string;
  slots?: SlotDto[];
}

export interface LocationUpdateRequest {
  name?: string;
  address?: string;
  totalSlots?: number;
  city?: string;
  imageFile?: string;
}
