import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BookingService } from '../../../api/booking/booking.service';
import { BookingDto } from '../../../api/booking/booking.models';
import { LocationService } from '../../../api/location/location.service';
import { LocationDto } from '../../../api/location/location.models';
import { Auth } from '../../../core/services/auth';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './my-bookings.component.html',
  styleUrls: ['./my-bookings.component.scss']
})
export class MyBookingsComponent implements OnInit {
  bookings: BookingDto[] = [];
  loading = true;
  searchTerm = '';
  userId: string | null = '';
  
  // Modal state
  showBookingModal = false;
  modalLoading = false;
  bookingForm!: FormGroup;
  locations: LocationDto[] = [];
  availableSlots: any[] = [];
  selectedLocationId: string = '';

  constructor(
    private bookingService: BookingService,
    private locationService: LocationService,
    private authService: Auth,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.userId = this.authService.getUserId();
    this.initializeForm();
    this.loadBookings();
    this.loadLocations();
  }

  initializeForm(): void {
    this.bookingForm = this.fb.group({
      locationId: ['', Validators.required],
      slotId: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required]
    });
  }

  loadLocations(): void {
    this.locationService.getAllLocations().subscribe({
      next: (data) => {
        this.locations = data;
      },
      error: (err) => console.error('Error loading locations:', err)
    });
  }

  onLocationChange(locationId: string): void {
    this.selectedLocationId = locationId;
    if (locationId) {
      this.locationService.getSlotsByLocationId(locationId).subscribe({
        next: (slots) => {
          this.availableSlots = slots.filter((slot: any) => slot.isAvailable);
        },
        error: (err) => console.error('Error loading slots:', err)
      });
    }
  }

  openBookingModal(): void {
    this.showBookingModal = true;
    this.bookingForm.reset();
    this.availableSlots = [];
  }

  closeBookingModal(): void {
    this.showBookingModal = false;
    this.bookingForm.reset();
    this.availableSlots = [];
  }

  saveBooking(): void {
    if (this.bookingForm.invalid) {
      alert('Please fill all required fields');
      return;
    }

    this.modalLoading = true;
    const formValue = this.bookingForm.value;

    const bookingData = {
      slotId: formValue.slotId,
      startTime: new Date(formValue.startTime).toISOString(),
      endTime: new Date(formValue.endTime).toISOString()
    };

    this.bookingService.createBooking(bookingData).subscribe({
      next: () => {
        alert('Booking created successfully!');
        this.modalLoading = false;
        this.closeBookingModal();
        this.loadBookings();
      },
      error: (err) => {
        console.error('Error creating booking:', err);
        alert('Error creating booking: ' + (err.error?.Message || err.message));
        this.modalLoading = false;
      }
    });
  }

  loadBookings(): void {
    this.loading = true;
    this.bookingService.getMyBookings().subscribe({
      next: (data) => {
        // Get current user's bookings directly from API
        this.bookings = data || [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading bookings:', err);
        // Show error but don't block UI
        this.bookings = [];
        this.loading = false;
        // Optionally show a toast or alert
        if (err.status === 401) {
          console.warn('Unauthorized - please log in again');
        }
      }
    });
  }

  get filteredBookings(): BookingDto[] {
    if (!this.searchTerm) {
      return this.bookings;
    }
    return this.bookings.filter(booking =>
      booking.slotNumber?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      booking.status?.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  cancelBooking(id: string): void {
    if (confirm('Are you sure you want to cancel this booking?')) {
      this.bookingService.cancelBooking(id).subscribe({
        next: () => {
          alert('Booking cancelled successfully');
          this.loadBookings();
        },
        error: (err) => console.error('Error cancelling booking:', err)
      });
    }
  }
}
