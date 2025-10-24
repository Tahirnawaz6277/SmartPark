import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BookingService } from '../../../api/booking/booking.service';
import { Auth } from '../../../core/services/auth';
import { BookingDto } from '../../../api/booking/booking.models';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-driver-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './driver-dashboard.component.html',
  styleUrls: ['./driver-dashboard.component.scss']
})
export class DriverDashboardComponent implements OnInit, OnDestroy {
  upcomingBookings: BookingDto[] = [];
  totalBookings = 0;
  totalSpent = 0;
  loading = true;
  userId: string | null = '';
  private destroy$ = new Subject<void>();

  constructor(
    private bookingService: BookingService,
    private authService: Auth,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.userId = this.authService.getUserId();
    this.loadDashboardData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadDashboardData(): void {
    this.loading = true;
    this.cdr.detectChanges(); // Ensure loading state is reflected immediately

    // Check if user is authenticated before making API call
    if (!this.authService.isLoggedIn()) {
      console.warn('User not authenticated, redirecting to login');
      this.loading = false;
      this.cdr.detectChanges();
      return;
    }

    this.bookingService.getMyBookings()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (bookings) => {
          try {
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
            this.cdr.detectChanges(); // Ensure UI updates
          } catch (error) {
            console.error('Error processing booking data:', error);
            this.handleError();
          }
        },
        error: (err) => {
          console.error('Error loading bookings:', err);
          this.handleError();
        }
      });
  }

  private handleError(): void {
    // Initialize with empty data to prevent UI issues
    this.totalBookings = 0;
    this.upcomingBookings = [];
    this.totalSpent = 0;
    this.loading = false;
    this.cdr.detectChanges();

    if (this.authService.getToken()) {
      // Token exists but API failed, might be a server issue
      console.warn('API request failed despite having token');
    } else {
      // No token, user needs to log in
      console.warn('No authentication token found');
    }
  }
}
