import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd } from '@angular/router';
import { BookingService } from '../../../api/booking/booking.service';
import { BillingService } from '../../../api/billing/billing.service';
import { UserService } from '../../../api/user/user.service';
import { LocationService } from '../../../api/location/location.service';
import { Subscription, filter, forkJoin } from 'rxjs';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit, OnDestroy {
  totalUsers = 0;
  totalBookings = 0;
  totalRevenue = 0;
  totalLocations = 0;
  totalBillings = 0;
  loading = true;
  private routerSubscription?: Subscription;

  constructor(
    private bookingService: BookingService,
    private billingService: BillingService,
    private userService: UserService,
    private locationService: LocationService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
    
    // Reload data when navigating back to dashboard
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        if (event.url.includes('/admin/dashboard') || event.url === '/admin' || event.url === '/admin/') {
          console.log('Navigated to dashboard, reloading data...');
          this.loadDashboardData();
        }
      });
  }

  ngOnDestroy(): void {
    this.routerSubscription?.unsubscribe();
  }

  loadDashboardData(): void {
    this.loading = true;
    this.cdr.detectChanges();
    
    console.log('Loading dashboard data...');

    // Use forkJoin to load all data in parallel
    forkJoin({
      users: this.userService.getAllUsers(),
      bookings: this.bookingService.getAllBookings(),
      billings: this.billingService.getAllBillings(),
      locations: this.locationService.getAllLocations()
    }).subscribe({
      next: (result) => {
        console.log('Dashboard data loaded:', result);
        
        this.totalUsers = result.users?.length || 0;
        this.totalBookings = result.bookings?.length || 0;
        this.totalBillings = result.billings?.length || 0;
        this.totalRevenue = result.billings?.reduce((sum, bill) => sum + (bill.amount || 0), 0) || 0;
        this.totalLocations = result.locations?.length || 0;
        
        this.loading = false;
        this.cdr.detectChanges();
        
        console.log('Dashboard stats:', {
          users: this.totalUsers,
          bookings: this.totalBookings,
          billings: this.totalBillings,
          revenue: this.totalRevenue,
          locations: this.totalLocations
        });
      },
      error: (err) => {
        console.error('Error loading dashboard data:', err);
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // Navigate to specific page
  navigateTo(route: string): void {
    this.router.navigate([`/admin/${route}`]);
  }
}
