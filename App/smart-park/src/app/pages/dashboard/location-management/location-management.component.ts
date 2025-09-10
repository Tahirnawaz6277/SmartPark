import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ApiService, LocationDto, CreateLocationRequest, CreateLocationReponse } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-location-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
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
  formData: CreateLocationRequest = {
    Name: '',
    Address: '',
    SmallSlotCount: 0,
    MediumSlotCount: 0,
    LargeSlotCount: 0,
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
        if (response.success && response.data) {
          this.locations = response.data;
        } else {
          this.errorMessage = response.message || 'Failed to load locations.';
          this.autoDismissMessages();
        }
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
        if (response.success && response.data) {
          this.editingLocation = response.data;
          this.formData = {
            Name: response.data.Name ?? '',
            Address: response.data.Address ?? '',
            SmallSlotCount: 0,
            MediumSlotCount: 0,
            LargeSlotCount: 0,
            city: response.data.City ?? '',
            image: response.data.Image ?? ''
          };
          this.showAddForm = true;
        } else {
          this.errorMessage = response.message || 'Failed to load location details.';
          this.autoDismissMessages();
        }
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
      Name: '',
      Address: '',
      SmallSlotCount: 0,
      MediumSlotCount: 0,
      LargeSlotCount: 0,
      city: '',
      image: ''
    };
    this.showAddForm = true;
    this.clearMessages();
  }

  editLocation(location: LocationDto): void {
    this.getLocationById(location.Id);
  }

  saveLocation(form: NgForm): void {
    if (form.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      if (this.editingLocation) {
        // Update existing location
        this.apiService.updateLocation(this.editingLocation.Id, this.formData).subscribe({
          next: (response) => {
            this.isLoading = false;
            if (response.success) {
              this.successMessage = 'Location updated successfully!';
              this.loadLocations();
              this.closeForm();
              this.autoDismissMessages();
            } else {
              this.errorMessage = response.message || 'Failed to update location.';
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
            if (response.success) {
              this.successMessage = 'Location created successfully!';
              this.loadLocations();
              this.closeForm();
              this.autoDismissMessages();
            } else {
              this.errorMessage = response.message || 'Failed to create location.';
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
    if (confirm(`Are you sure you want to delete location "${location.Name}"?`)) {
      this.isLoading = true;
      this.errorMessage = '';
      
      this.apiService.deleteLocation(location.Id).subscribe({
        next: (response) => {
          this.isLoading = false;
          if (response.success) {
            this.successMessage = 'Location deleted successfully!';
            this.loadLocations();
            this.autoDismissMessages();
          } else {
            this.errorMessage = response.message || 'Failed to delete location.';
            this.autoDismissMessages();
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
      Name: '',
      Address: '',
      SmallSlotCount: 0,
      MediumSlotCount: 0,
      LargeSlotCount: 0,
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
