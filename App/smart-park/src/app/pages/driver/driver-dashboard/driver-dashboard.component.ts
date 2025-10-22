import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BookingService } from '../../../api/booking/booking.service';
import { BillingService } from '../../../api/billing/billing.service';
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
    private billingService: BillingService,
    private authService: Auth
  ) {}

  ngOnInit(): void {
    this.userId = this.authService.getUserId();
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;

    this.bookingService.getAllBookings().subscribe({
      next: (bookings) => {
        // Filter bookings for current user
        const userBookings = bookings.filter(b => b.userId === this.userId);
        this.totalBookings = userBookings.length;
        
        // Get upcoming bookings
        const now = new Date();
        this.upcomingBookings = userBookings
          .filter(b => b.startTime && new Date(b.startTime) > now)
          .slice(0, 5);
      },
      error: (err) => console.error('Error loading bookings:', err)
    });

    this.billingService.getAllBillings().subscribe({
      next: (billings) => {
        // Calculate total spent (would need to filter by user's bookings)
        this.totalSpent = billings.reduce((sum, bill) => sum + (bill.amount || 0), 0);
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading billings:', err);
        this.loading = false;
      }
    });
  }
}
