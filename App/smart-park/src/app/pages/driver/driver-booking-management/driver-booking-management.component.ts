import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, NavigationEnd } from '@angular/router';
import { BookingService } from '../../../api/booking/booking.service';
import { BookingDto } from '../../../api/booking/booking.models';
import { LocationService } from '../../../api/location/location.service';
import { LocationDto, SlotDto } from '../../../api/location/location.models';
import { Auth } from '../../../core/services/auth';
import { Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-driver-booking-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './driver-booking-management.component.html',
  styleUrls: ['./driver-booking-management.component.scss']
})
export class DriverBookingManagementComponent implements OnInit, OnDestroy {
  bookings: BookingDto[] = [];
  locations: LocationDto[] = [];
  availableSlots: SlotDto[] = [];
  loading = true;
  searchTerm = '';
  private routerSubscription?: Subscription;

  // Modal state
  showModal = false;
  showDetailsModal = false;
  modalLoading = false;
  bookingForm!: FormGroup;
  selectedLocationId: string = '';
  selectedBooking: BookingDto | null = null;

  constructor(
    private bookingService: BookingService,
    private locationService: LocationService,
    private authService: Auth,
    private cdr: ChangeDetectorRef,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.initForm();
  }

  initForm(): void {
    this.bookingForm = this.fb.group({
      locationId: ['', Validators.required],
      slotId: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadBookings();

    // Reload data when navigating back to this component
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        if (event.url.includes('/driver/booking-management')) {
          console.log('Navigated to booking management, reloading data...');
          this.loadBookings();
        }
      });
  }

  ngOnDestroy(): void {
    this.routerSubscription?.unsubscribe();
  }

  loadBookings(): void {
    this.loading = true;
    this.cdr.detectChanges(); // Force UI update

    console.log('Loading bookings...');
    this.bookingService.getMyBookings().subscribe({
      next: (data) => {
        console.log('Bookings loaded successfully:', data);
        console.log('Number of bookings:', data?.length || 0);

        // Ensure data is an array
        this.bookings = Array.isArray(data) ? data : [];
        this.loading = false;

        // Force change detection
        this.cdr.detectChanges();

        console.log('Bookings assigned to component:', this.bookings.length);
      },
      error: (err) => {
        console.error('Error loading bookings:', err);
        alert('Error loading bookings: ' + (err.error?.Message || err.message));
        this.bookings = [];
        this.loading = false;
        this.cdr.detectChanges();
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
      booking.status?.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  // Get duration in hours for a booking
  getBookingDuration(booking: BookingDto): number {
    if (booking.startTime && booking.endTime) {
      const startTime = new Date(booking.startTime).getTime();
      const endTime = new Date(booking.endTime).getTime();
      return (endTime - startTime) / (1000 * 60 * 60);
    }
    return 0;
  }

  // Get duration for selected booking in modal
  get selectedBookingDuration(): number {
    if (this.selectedBooking) {
      return this.getBookingDuration(this.selectedBooking);
    }
    return 0;
  }

  cancelBooking(id: string): void {
    if (confirm('Are you sure you want to cancel this booking?')) {
      this.bookingService.cancelBooking(id).subscribe({
        next: () => {
          console.log('Booking cancelled successfully');
          alert('Booking cancelled successfully');
          this.loadBookings();
        },
        error: (err) => {
          console.error('Error cancelling booking:', err);
          alert('Error cancelling booking: ' + (err.error?.Message || err.message));
        }
      });
    }
  }

  // View booking details
  viewBookingDetails(booking: BookingDto): void {
    this.selectedBooking = booking;
    this.showDetailsModal = true;
    this.cdr.detectChanges();
  }

  // Get full booking details by ID
  loadBookingDetails(id: string): void {
    this.bookingService.getBookingById(id).subscribe({
      next: (booking: BookingDto) => {
        this.selectedBooking = booking;
        this.showDetailsModal = true;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error loading booking details:', err);
        alert('Error loading booking details: ' + (err.error?.Message || err.message));
      }
    });
  }

  // Open add booking modal
  openAddModal(): void {
    this.showModal = true;
    this.modalLoading = true;
    this.bookingForm.reset();
    this.selectedLocationId = '';
    this.availableSlots = [];

    // Load locations for dropdown
    this.locationService.getAllLocations().subscribe({
      next: (data: LocationDto[]) => {
        this.locations = data;
        this.modalLoading = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error loading locations:', err);
        alert('Error loading locations: ' + (err.error?.Message || err.message));
        this.modalLoading = false;
        this.showModal = false;
      }
    });
  }

  // Handle location change to load slots
  onLocationChange(event: any): void {
    const locationId = event.target.value;
    if (locationId) {
      const selectedLocation = this.locations.find(loc => loc.id === locationId);
      this.availableSlots = selectedLocation?.slots || [];
      this.bookingForm.patchValue({ slotId: '' });
    } else {
      this.availableSlots = [];
    }
    this.cdr.detectChanges();
  }

  // Close modal
  closeModal(): void {
    this.showModal = false;
    this.bookingForm.reset();
    this.selectedLocationId = '';
    this.availableSlots = [];
    this.modalLoading = false;
  }

  // Close details modal
  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedBooking = null;
  }

  // Save booking
  saveBooking(): void {
    if (this.bookingForm.invalid) {
      alert('Please fill all required fields correctly');
      return;
    }

    this.modalLoading = true;
    const formData = this.bookingForm.getRawValue();

    // Convert datetime-local to ISO string
    const bookingData = {
      slotId: formData.slotId,
      startTime: new Date(formData.startTime).toISOString(),
      endTime: new Date(formData.endTime).toISOString()
    };

    this.bookingService.createBooking(bookingData).subscribe({
      next: () => {
        alert('Booking created successfully');
        this.closeModal();
        this.loadBookings();
      },
      error: (err: any) => {
        console.error('Error creating booking:', err);
        alert('Error creating booking: ' + (err.error?.Message || err.message));
        this.modalLoading = false;
      }
    });
  }
}
