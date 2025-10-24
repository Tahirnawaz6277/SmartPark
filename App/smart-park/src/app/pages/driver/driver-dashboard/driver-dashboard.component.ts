import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BookingService } from '../../../api/booking/booking.service';
import { Auth } from '../../../core/services/auth';
import { BookingDto } from '../../../api/booking/booking.models';

@Component({
  selector: 'app-driver-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './driver-dashboard.component.html',
  styleUrls: ['./driver-dashboard.component.scss']
})
export class DriverDashboardComponent implements OnInit {
  upcomingBookings: BookingDto[] = [];
  totalBookings = 0;
  totalSpent = 0;
  loading = true;
  userId: string | null = '';

  constructor(
    private bookingService: BookingService,
    private authService: Auth
  ) {}

  ngOnInit(): void {
    this.userId = this.authService.getUserId();
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;

    this.bookingService.getMyBookings().subscribe({
      next: (bookings) => {
        // Get all user's bookings
        const validBookings = bookings || [];
        this.totalBookings = validBookings.length;
        
        // Get upcoming bookings
        const now = new Date();
        this.upcomingBookings = validBookings
          .filter(b => b.startTime && new Date(b.startTime) > now)
          .slice(0, 5);
        
        // Note: Total spent is managed by admin, drivers pay cash
        this.totalSpent = 0;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading bookings:', err);
        // Initialize with empty data to prevent UI issues
        this.totalBookings = 0;
        this.upcomingBookings = [];
        this.totalSpent = 0;
        this.loading = false;
        
        if (err.status === 401) {
          console.warn('Unauthorized - please log in again');
        }
      }
    });
  }
}
