import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, NavigationEnd } from '@angular/router';
import { BookingService } from '../../../api/booking/booking.service';
import { BookingDto } from '../../../api/booking/booking.models';
import { LocationService } from '../../../api/location/location.service';
import { LocationDto, SlotDto } from '../../../api/location/location.models';
import { Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-bookings',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './bookings.component.html',
  styleUrls: ['./bookings.component.scss']
})
export class BookingsComponent implements OnInit, OnDestroy {
  bookings: BookingDto[] = [];
  locations: LocationDto[] = [];
  availableSlots: SlotDto[] = [];
  loading = true;
  searchTerm = '';
  private routerSubscription?: Subscription;
  
  // Modal state
  showModal = false;
  modalLoading = false;
  bookingForm!: FormGroup;
  selectedLocationId: string = '';

  constructor(
    private bookingService: BookingService,
    private locationService: LocationService,
    private cdr: ChangeDetectorRef,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.initForm();
  }

  initForm(): void {
    this.bookingForm = this.fb.group({
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
        if (event.url.includes('/admin/bookings')) {
          console.log('Navigated to bookings, reloading data...');
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
    this.bookingService.getAllBookings().subscribe({
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
      booking.userName?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      booking.status?.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
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

  // Open add booking modal
  openAddModal(): void {
    this.showModal = true;
    this.modalLoading = true;
    this.bookingForm.reset();
    this.selectedLocationId = '';
    this.availableSlots = [];
    
    // Load locations for dropdown
    this.locationService.getAllLocations().subscribe({
      next: (locations) => {
        this.locations = locations;
        this.modalLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
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
      error: (err) => {
        console.error('Error creating booking:', err);
        alert('Error creating booking: ' + (err.error?.Message || err.message));
        this.modalLoading = false;
      }
    });
  }

  deleteBooking(id: string): void {
    if (confirm('Are you sure you want to delete this booking?')) {
      this.bookingService.deleteBooking(id).subscribe({
        next: () => {
          console.log('Booking deleted successfully');
          alert('Booking deleted successfully');
          this.loadBookings();
        },
        error: (err) => {
          console.error('Error deleting booking:', err);
          alert('Error deleting booking: ' + (err.error?.Message || err.message));
        }
      });
    }
  }
}
