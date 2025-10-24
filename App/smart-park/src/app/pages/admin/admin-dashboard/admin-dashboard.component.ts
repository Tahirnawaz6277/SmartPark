import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd } from '@angular/router';
import { BookingService } from '../../../api/booking/booking.service';
import { BillingService } from '../../../api/billing/billing.service';
import { UserService } from '../../../api/user/user.service';
import { LocationService } from '../../../api/location/location.service';
import { LocationDto, SlotDto } from '../../../api/location/location.models';
import { Subscription, filter, forkJoin, firstValueFrom } from 'rxjs';

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

  // Location statistics
  locationsWithSlots: Array<{
    location: LocationDto;
    totalSlots: number;
    availableSlots: number;
    occupiedSlots: number;
  }> = [];

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

    // First load basic data
    forkJoin({
      users: this.userService.getAllUsers(),
      bookings: this.bookingService.getAllBookings(),
      billings: this.billingService.getAllBillings(),
      locations: this.locationService.getAllLocations()
    }).subscribe({
      next: async (result) => {
        console.log('Basic dashboard data loaded:', result);

        this.totalUsers = result.users?.length || 0;
        this.totalBookings = result.bookings?.length || 0;
        this.totalBillings = result.billings?.length || 0;
        this.totalRevenue = result.billings?.reduce((sum, bill) => sum + (bill.amount || 0), 0) || 0;
        this.totalLocations = result.locations?.length || 0;

        // Load slot statistics for each location
        await this.loadLocationSlotStatistics(result.locations || []);

        this.loading = false;
        this.cdr.detectChanges();

        console.log('Dashboard stats:', {
          users: this.totalUsers,
          bookings: this.totalBookings,
          billings: this.totalBillings,
          revenue: this.totalRevenue,
          locations: this.totalLocations,
          locationStats: this.locationsWithSlots
        });
      },
      error: (err) => {
        console.error('Error loading dashboard data:', err);
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  private async loadLocationSlotStatistics(locations: LocationDto[]): Promise<void> {
    this.locationsWithSlots = [];

    for (const location of locations) {
      try {
        const slots = await firstValueFrom(this.locationService.getSlotsByLocationId(location.id));

        if (slots && Array.isArray(slots)) {
          const totalSlots = slots.length;
          const availableSlots = slots.filter((slot: SlotDto) => slot.isAvailable).length;
          const occupiedSlots = totalSlots - availableSlots;

          this.locationsWithSlots.push({
            location,
            totalSlots,
            availableSlots,
            occupiedSlots
          });
        } else {
          // Location has no slots or API returned null
          this.locationsWithSlots.push({
            location,
            totalSlots: location.totalSlots || 0,
            availableSlots: 0,
            occupiedSlots: location.totalSlots || 0
          });
        }
      } catch (error) {
        console.error(`Error loading slots for location ${location.id}:`, error);
        // Add location with default values even if slots loading fails
        this.locationsWithSlots.push({
          location,
          totalSlots: location.totalSlots || 0,
          availableSlots: 0,
          occupiedSlots: location.totalSlots || 0
        });
      }
    }

    this.cdr.detectChanges();
  }

  // Navigate to specific page
  navigateTo(route: string): void {
    this.router.navigate([`/admin/${route}`]);
  }
}
