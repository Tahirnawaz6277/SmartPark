import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, NavigationEnd } from '@angular/router';
import { BookingService } from '../../../api/booking/booking.service';
import { BookingDto } from '../../../api/booking/booking.models';
import { Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-bookings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './bookings.component.html',
  styleUrls: ['./bookings.component.scss']
})
export class BookingsComponent implements OnInit, OnDestroy {
  bookings: BookingDto[] = [];
  loading = true;
  searchTerm = '';
  private routerSubscription?: Subscription;

  constructor(
    private bookingService: BookingService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

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
