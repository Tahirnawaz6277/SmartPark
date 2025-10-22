import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, NavigationEnd } from '@angular/router';
import { LocationService } from '../../../api/location/location.service';
import { LocationDto } from '../../../api/location/location.models';
import { Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-locations',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './locations.component.html',
  styleUrls: ['./locations.component.scss']
})
export class LocationsComponent implements OnInit, OnDestroy {
  locations: LocationDto[] = [];
  loading = true;
  searchTerm = '';
  private routerSubscription?: Subscription;

  constructor(
    private locationService: LocationService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadLocations();
    
    // Reload data when navigating back to this component
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        if (event.url.includes('/admin/locations')) {
          console.log('Navigated to locations, reloading data...');
          this.loadLocations();
        }
      });
  }

  ngOnDestroy(): void {
    this.routerSubscription?.unsubscribe();
  }

  loadLocations(): void {
    this.loading = true;
    this.cdr.detectChanges();
    
    console.log('Loading locations...');
    this.locationService.getAllLocations().subscribe({
      next: (data) => {
        console.log('Locations loaded successfully:', data);
        console.log('Number of locations:', data?.length || 0);
        
        this.locations = Array.isArray(data) ? data : [];
        this.loading = false;
        this.cdr.detectChanges();
        
        console.log('Locations assigned to component:', this.locations.length);
      },
      error: (err) => {
        console.error('Error loading locations:', err);
        alert('Error loading locations: ' + (err.error?.Message || err.message));
        this.locations = [];
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  get filteredLocations(): LocationDto[] {
    if (!this.searchTerm) {
      return this.locations;
    }
    return this.locations.filter(location =>
      location.name?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      location.city?.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  deleteLocation(id: string): void {
    if (confirm('Are you sure you want to delete this location?')) {
      this.locationService.deleteLocation(id).subscribe({
        next: () => {
          console.log('Location deleted successfully');
          alert('Location deleted successfully');
          this.loadLocations();
        },
        error: (err) => {
          console.error('Error deleting location:', err);
          alert('Error deleting location: ' + (err.error?.Message || err.message));
        }
      });
    }
  }

  getAvailableSlots(location: LocationDto): number {
    return location.slots?.filter(slot => slot.isAvailable).length || 0;
  }
}
