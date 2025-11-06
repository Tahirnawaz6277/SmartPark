import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BookingService } from '../../../api/booking/booking.service';
import { BookingDto } from '../../../api/booking/booking.models';
import { LocationService } from '../../../api/location/location.service';
import { LocationDto } from '../../../api/location/location.models';
import { Auth } from '../../../core/services/auth';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './my-bookings.component.html',
  styleUrls: ['./my-bookings.component.scss']
})
export class MyBookingsComponent implements OnInit, OnDestroy {
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

  private destroy$ = new Subject<void>();

  constructor(
    private bookingService: BookingService,
    private locationService: LocationService,
    private authService: Auth,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.userId = this.authService.getUserId();
    this.initializeForm();
    this.loadBookings();
    this.loadLocations();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  initializeForm(): void {
    this.bookingForm = this.fb.group({
      locationId: ['', Validators.required],
      slotIds: [[], Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required]
    });
  }

  loadLocations(): void {
    this.locationService.getAllLocations()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data: LocationDto[]) => {
          this.locations = data;
          this.cdr.detectChanges();
        },
        error: (err: any) => {
          console.error('Error loading locations:', err);
          this.cdr.detectChanges();
        }
      });
  }

  onLocationChange(locationId: string): void {
    this.selectedLocationId = locationId;
    if (locationId) {
      this.locationService.getSlotsByLocationId(locationId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (slots: any[]) => {
            this.availableSlots = slots.filter((slot: any) => slot.isAvailable);
            this.bookingForm.patchValue({ slotIds: [] }); // Reset slot selection
            this.cdr.detectChanges();
          },
          error: (err: any) => {
            console.error('Error loading slots:', err);
            this.cdr.detectChanges();
          }
        });
    } else {
      this.availableSlots = [];
      this.bookingForm.patchValue({ slotIds: [] });
      this.cdr.detectChanges();
    }
  }

  // Toggle slot selection
  toggleSlotSelection(slotId: string): void {
    const currentSlots = this.bookingForm.get('slotIds')?.value || [];
    const index = currentSlots.indexOf(slotId);
    
    if (index === -1) {
      // Add slot
      this.bookingForm.patchValue({ slotIds: [...currentSlots, slotId] });
    } else {
      // Remove slot
      const updatedSlots = currentSlots.filter((id: string) => id !== slotId);
      this.bookingForm.patchValue({ slotIds: updatedSlots });
    }
  }

  // Check if slot is selected
  isSlotSelected(slotId: string): boolean {
    const selectedSlots = this.bookingForm.get('slotIds')?.value || [];
    return selectedSlots.includes(slotId);
  }

  openBookingModal(): void {
    this.showBookingModal = true;
    this.bookingForm.reset();
    this.availableSlots = [];
    this.cdr.detectChanges();
  }

  closeBookingModal(): void {
    this.showBookingModal = false;
    this.bookingForm.reset();
    this.availableSlots = [];
    this.cdr.detectChanges();
  }

  saveBooking(): void {
    if (this.bookingForm.invalid) {
      alert('Please fill all required fields');
      return;
    }

    const selectedSlots = this.bookingForm.get('slotIds')?.value || [];
    if (selectedSlots.length === 0) {
      alert('Please select at least one slot');
      return;
    }

    this.modalLoading = true;
    this.cdr.detectChanges();

    const formValue = this.bookingForm.value;

    // Format datetime-local value to ISO string
    // Treat as UTC to preserve exact time entered by user
    const bookingData = {
      slotIds: formValue.slotIds,
      startTime: formValue.startTime + ':00.000Z',
      endTime: formValue.endTime + ':00.000Z'
    };

    this.bookingService.createBooking(bookingData)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          alert('Booking created successfully!');
          this.modalLoading = false;
          this.closeBookingModal();
          this.loadBookings();
        },
        error: (err: any) => {
          console.error('Error creating booking:', err);
          alert('Error creating booking: ' + (err.error?.Message || err.message));
          this.modalLoading = false;
          this.cdr.detectChanges();
        }
      });
  }

  loadBookings(): void {
    this.loading = true;
    this.cdr.detectChanges();

    // Check if user is authenticated before making API call
    if (!this.authService.isLoggedIn()) {
      console.warn('User not authenticated, cannot load bookings');
      this.loading = false;
      this.cdr.detectChanges();
      return;
    }

    this.bookingService.getMyBookings()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data: BookingDto[]) => {
          // Get current user's bookings directly from API
          this.bookings = data || [];
          this.loading = false;
          this.cdr.detectChanges();
        },
        error: (err: any) => {
          console.error('Error loading bookings:', err);
          // Show error but don't block UI
          this.bookings = [];
          this.loading = false;
          this.cdr.detectChanges();

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
      booking.locationName?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      booking.slotNumber?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      booking.slotNumbers?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      booking.status?.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  cancelBooking(id: string): void {
    if (confirm('Are you sure you want to cancel this booking?')) {
      this.bookingService.cancelBooking(id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            alert('Booking cancelled successfully');
            this.loadBookings();
          },
          error: (err: any) => {
            console.error('Error cancelling booking:', err);
            alert('Error cancelling booking: ' + (err.error?.Message || err.message));
            this.cdr.detectChanges();
          }
        });
    }
  }
}
