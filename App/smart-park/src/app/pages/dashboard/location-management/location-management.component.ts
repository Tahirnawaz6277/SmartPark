import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ApiService, LocationResponse, LocationRequest } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-location-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './location-management.component.html',
  styleUrl: './location-management.component.scss'
})
export class LocationManagementComponent implements OnInit {
  locations: LocationResponse[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  showAddForm = false;
  editingLocation: LocationResponse | null = null;

  // Form data for add/edit
  formData: LocationRequest = {
    name: '',
    address: '',
    capacity: 0,
    hourlyRate: 0,
    description: ''
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
        if (response.success && response.data) {
          this.locations = response.data;
        } else {
          this.errorMessage = response.message || 'Failed to load locations.';
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.message || 'An error occurred while loading locations.';
      }
    });
  }

  getLocationById(id: string): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.apiService.getLocationById(id).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success && response.data) {
          this.editingLocation = response.data;
          this.formData = {
            name: response.data.name,
            address: response.data.address,
            capacity: response.data.capacity,
            hourlyRate: response.data.hourlyRate,
            description: response.data.description || ''
          };
          this.showAddForm = true;
        } else {
          this.errorMessage = response.message || 'Failed to load location details.';
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.message || 'An error occurred while loading location details.';
      }
    });
  }

  addLocation(): void {
    this.editingLocation = null;
    this.formData = {
      name: '',
      address: '',
      capacity: 0,
      hourlyRate: 0,
      description: ''
    };
    this.showAddForm = true;
    this.clearMessages();
  }

  editLocation(location: LocationResponse): void {
    this.getLocationById(location.id);
  }

  saveLocation(form: NgForm): void {
    if (form.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      if (this.editingLocation) {
        // Update existing location
        this.apiService.updateLocation(this.editingLocation.id, this.formData).subscribe({
          next: (response) => {
            this.isLoading = false;
            if (response.success) {
              this.successMessage = 'Location updated successfully!';
              this.loadLocations();
              this.closeForm();
            } else {
              this.errorMessage = response.message || 'Failed to update location.';
            }
          },
          error: (error) => {
            this.isLoading = false;
            this.errorMessage = error.message || 'An error occurred while updating location.';
          }
        });
      } else {
        // Create new location
        this.apiService.createLocation(this.formData).subscribe({
          next: (response) => {
            this.isLoading = false;
            if (response.success) {
              this.successMessage = 'Location created successfully!';
              this.loadLocations();
              this.closeForm();
            } else {
              this.errorMessage = response.message || 'Failed to create location.';
            }
          },
          error: (error) => {
            this.isLoading = false;
            this.errorMessage = error.message || 'An error occurred while creating location.';
          }
        });
      }
    } else {
      this.errorMessage = 'Please fill in all required fields correctly.';
    }
  }

  deleteLocation(location: LocationResponse): void {
    if (confirm(`Are you sure you want to delete location "${location.name}"?`)) {
      this.isLoading = true;
      this.errorMessage = '';
      
      this.apiService.deleteLocation(location.id).subscribe({
        next: (response) => {
          this.isLoading = false;
          if (response.success) {
            this.successMessage = 'Location deleted successfully!';
            this.loadLocations();
          } else {
            this.errorMessage = response.message || 'Failed to delete location.';
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.message || 'An error occurred while deleting location.';
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
      capacity: 0,
      hourlyRate: 0,
      description: ''
    };
    this.clearMessages();
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}
