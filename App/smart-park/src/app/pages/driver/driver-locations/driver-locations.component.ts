import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LocationService } from '../../../api/location/location.service';
import { LocationDto } from '../../../api/location/location.models';

@Component({
  selector: 'app-driver-locations',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './driver-locations.component.html',
  styleUrls: ['./driver-locations.component.scss']
})
export class DriverLocationsComponent implements OnInit {
  locations: LocationDto[] = [];
  filteredLocations: LocationDto[] = [];
  loading = true;
  searchTerm = '';

  constructor(private locationService: LocationService) {}

  ngOnInit(): void {
    this.loadLocations();
  }

  loadLocations(): void {
    this.loading = true;
    this.locationService.getAllLocations().subscribe({
      next: (data) => {
        this.locations = data;
        this.filteredLocations = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading locations:', err);
        this.loading = false;
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
  }

  viewSlots(locationId: string): void {
    // Navigate to slots view or show slots modal
    console.log('View slots for location:', locationId);
  }
}
