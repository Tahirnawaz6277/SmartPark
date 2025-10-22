import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookingService } from '../../../api/booking/booking.service';
import { BookingDto } from '../../../api/booking/booking.models';
import { Auth } from '../../../core/services/auth';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './my-bookings.component.html',
  styleUrls: ['./my-bookings.component.scss']
})
export class MyBookingsComponent implements OnInit {
  bookings: BookingDto[] = [];
  loading = true;
  searchTerm = '';
  userId: string | null = '';

  constructor(
    private bookingService: BookingService,
    private authService: Auth
  ) {}

  ngOnInit(): void {
    this.userId = this.authService.getUserId();
    this.loadBookings();
  }

  loadBookings(): void {
    this.loading = true;
    this.bookingService.getAllBookings().subscribe({
      next: (data) => {
        // Filter bookings for current user
        this.bookings = data.filter(b => b.userId === this.userId);
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading bookings:', err);
        this.loading = false;
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
