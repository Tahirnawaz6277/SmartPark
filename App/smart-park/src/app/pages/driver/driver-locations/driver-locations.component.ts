import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LocationService } from '../../../api/location/location.service';
import { LocationDto, SlotDto } from '../../../api/location/location.models';
import { Auth } from '../../../core/services/auth';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-driver-locations',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './driver-locations.component.html',
  styleUrls: ['./driver-locations.component.scss']
})
export class DriverLocationsComponent implements OnInit, OnDestroy {
  locations: LocationDto[] = [];
  filteredLocations: LocationDto[] = [];
  loading = true;
  searchTerm = '';

  // Slots modal state
  showSlotsModal = false;
  slotsModalLoading = false;
  selectedLocation: LocationDto | null = null;
  locationSlots: SlotDto[] = [];

  private destroy$ = new Subject<void>();

  constructor(
    private locationService: LocationService,
    private authService: Auth,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadLocations();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadLocations(): void {
    this.loading = true;
    this.cdr.detectChanges();

    // Check if user is authenticated before making API call
    if (!this.authService.isLoggedIn()) {
      console.warn('User not authenticated, cannot load locations');
      this.loading = false;
      this.cdr.detectChanges();
      return;
    }

    this.locationService.getAllLocations()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.locations = data;
          this.filteredLocations = data;
          this.loading = false;
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Error loading locations:', err);
          this.locations = [];
          this.filteredLocations = [];
          this.loading = false;
          this.cdr.detectChanges();

          if (err.status === 401) {
            console.warn('Unauthorized - please log in again');
          }
        }
      });
  }

  filterLocations(): void {
    if (!this.searchTerm) {
      this.filteredLocations = this.locations;
    } else {
      this.filteredLocations = this.locations.filter(location =>
        location.name?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        location.city?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        location.address?.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    }
    this.cdr.detectChanges();
  }

  viewSlots(location: LocationDto): void {
    this.selectedLocation = location;
    this.showSlotsModal = true;
    this.slotsModalLoading = true;
    this.locationSlots = [];
    this.cdr.detectChanges();

    // Load slots for the selected location
    this.locationService.getSlotsByLocationId(location.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (slots: SlotDto[]) => {
          this.locationSlots = slots || [];
          this.slotsModalLoading = false;
          this.cdr.detectChanges();
        },
        error: (err: any) => {
          console.error('Error loading slots:', err);
          alert('Error loading slots: ' + (err.error?.Message || err.message));
          this.slotsModalLoading = false;
          this.cdr.detectChanges();
        }
      });
  }

  closeSlotsModal(): void {
    this.showSlotsModal = false;
    this.selectedLocation = null;
    this.locationSlots = [];
    this.slotsModalLoading = false;
  }

  get availableSlotsCount(): number {
    return this.locationSlots.filter(slot => slot.isAvailable).length;
  }

  get totalSlotsCount(): number {
    return this.locationSlots.length;
  }

  // Navigate to booking page with slot pre-selected
  bookSlot(slot: SlotDto): void {
    if (!slot.isAvailable) {
      alert('This slot is currently occupied. Please select an available slot.');
      return;
    }

    if (!this.selectedLocation) {
      return;
    }

    // Navigate to my-bookings with query parameters
    this.router.navigate(['/driver/my-bookings'], {
      queryParams: {
        locationId: this.selectedLocation.id,
        slotId: slot.id,
        locationName: this.selectedLocation.name
      }
    });
  }
}
