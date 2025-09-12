import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ApiService, LocationDto, LocationRequest, LocationResponse } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';
import { MasterLayoutComponent } from '../../../shared/master-layout/master-layout.component';

@Component({
  selector: 'app-location-management',
  standalone: true,
  imports: [CommonModule, FormsModule, MasterLayoutComponent],
  templateUrl: './location-management.component.html',
  styleUrl: './location-management.component.scss'
})
export class LocationManagementComponent implements OnInit {
  locations: LocationDto[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  showAddForm = false;
  editingLocation: LocationDto | null = null;

  // Form data for add/edit
  formData: LocationRequest = {
    name: '',
    address: '',
    totalSlots: 0,
    city: '',
    image: ''
  };

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadLocations();
  }

  loadLocations(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.apiService.getAllLocations().subscribe({
      next: (response) => {
        this.isLoading = false;
        this.locations = response || [];
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.message || 'An error occurred while loading locations.';
        this.autoDismissMessages();
      }
    });
  }

  getLocationById(id: string): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.apiService.getLocationById(id).subscribe({
      next: (response) => {
        this.isLoading = false;
        const loc = response as LocationDto;
        this.editingLocation = loc;
        this.formData = {
          name: loc.name ?? '',
          address: loc.address ?? '',
          totalSlots: loc.totalSlots ?? 0,
          city: loc.city ?? '',
          image: loc.image ?? ''
        };
        this.showAddForm = true;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.message || 'An error occurred while loading location details.';
        this.autoDismissMessages();
      }
    });
  }

  addLocation(): void {
    this.editingLocation = null;
    this.formData = {
      name: '',
      address: '',
      totalSlots: 0,
      city: '',
      image: ''
    };
    this.showAddForm = true;
    this.clearMessages();
  }

  editLocation(location: LocationDto): void {
    this.getLocationById((location as any).id);
  }

  saveLocation(form: NgForm): void {
    if (form.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      if (this.editingLocation) {
        // Update existing location
        this.apiService.updateLocation((this.editingLocation as any).id, this.formData).subscribe({
          next: (response) => {
            this.isLoading = false;
            if ((response as any).success !== false) {
              this.successMessage = 'Location updated successfully!';
              this.loadLocations();
              this.closeForm();
              this.autoDismissMessages();
            } else {
              this.errorMessage = (response as any).message || 'Failed to update location.';
              this.autoDismissMessages();
            }
          },
          error: (error) => {
            this.isLoading = false;
            this.errorMessage = error.message || 'An error occurred while updating location.';
            this.autoDismissMessages();
          }
        });
      } else {
        // Create new location
        this.apiService.createLocation(this.formData).subscribe({
          next: (response) => {
            this.isLoading = false;
            if ((response as any).success !== false) {
              this.successMessage = 'Location created successfully!';
              this.loadLocations();
              this.closeForm();
              this.autoDismissMessages();
            } else {
              this.errorMessage = (response as any).message || 'Failed to create location.';
              this.autoDismissMessages();
            }
          },
          error: (error) => {
            this.isLoading = false;
            this.errorMessage = error.message || 'An error occurred while creating location.';
            this.autoDismissMessages();
          }
        });
      }
    } else {
      this.errorMessage = 'Please fill in all required fields correctly.';
      this.autoDismissMessages();
    }
  }

  deleteLocation(location: LocationDto): void {
    if (confirm(`Are you sure you want to delete location "${(location as any).name}"?`)) {
      this.isLoading = true;
      this.errorMessage = '';
      
      this.apiService.deleteLocation((location as any).id).subscribe({
        next: (response) => {
          this.isLoading = false;
          if (response.success) {
            this.successMessage = 'Location deleted successfully!';
            this.loadLocations();
            this.autoDismissMessages();
          } else {
            this.errorMessage = response.message || 'Failed to delete location.';
            this.autoDismissMessages();
            this.loadLocations();
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.message || 'An error occurred while deleting location.';
          this.autoDismissMessages();
        }
      });
    }
  }

  closeForm(): void {
    this.showAddForm = false;
    this.editingLocation = null;
    this.formData = {
      name: '',
      address: '',
      totalSlots: 0,
      city: '',
      image: ''
    };
    this.clearMessages();
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private autoDismissMessages(): void {
    setTimeout(() => {
      this.clearMessages();
    }, 3000);
  }
}
