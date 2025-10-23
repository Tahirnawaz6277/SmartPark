import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, NavigationEnd } from '@angular/router';
import { LocationService } from '../../../api/location/location.service';
import { LocationDto } from '../../../api/location/location.models';
import { Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-locations',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './locations.component.html',
  styleUrls: ['./locations.component.scss']
})
export class LocationsComponent implements OnInit, OnDestroy {
  locations: LocationDto[] = [];
  loading = true;
  searchTerm = '';
  private routerSubscription?: Subscription;
  
  // Modal state
  showModal = false;
  showViewModal = false;
  modalLoading = false;
  locationForm!: FormGroup;
  selectedFile: File | null = null;
  imagePreview: string | null = null;
  isEditMode = false;
  editingLocationId: string | null = null;
  viewingLocation: LocationDto | null = null;

  constructor(
    private locationService: LocationService,
    private cdr: ChangeDetectorRef,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.initForm();
  }

  initForm(): void {
    this.locationForm = this.fb.group({
      name: ['', Validators.required],
      address: ['', Validators.required],
      totalSlots: [1, [Validators.required, Validators.min(1)]],
      city: ['', Validators.required]
    });
  }

  resetForm(): void {
    this.locationForm.reset({ totalSlots: 1 });
    this.selectedFile = null;
    this.imagePreview = null;
    this.isEditMode = false;
    this.editingLocationId = null;
  }

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

  // Open add location modal
  openAddModal(): void {
    this.showModal = true;
    this.resetForm();
  }

  // Open view location modal
  openViewModal(location: LocationDto): void {
    this.viewingLocation = location;
    this.showViewModal = true;
  }

  // Close view modal
  closeViewModal(): void {
    this.showViewModal = false;
    this.viewingLocation = null;
  }

  // Open edit location modal
  openEditModal(location: LocationDto): void {
    this.isEditMode = true;
    this.editingLocationId = location.id;
    this.showModal = true;
    this.modalLoading = true;

    // Fetch full location details
    this.locationService.getLocationById(location.id).subscribe({
      next: (data) => {
        this.locationForm.patchValue({
          name: data.name,
          address: data.address,
          totalSlots: data.totalSlots,
          city: data.city
        });
        
        // Set image preview if exists
        if (data.imageUrl) {
          this.imagePreview = data.imageUrl;
        }
        
        this.modalLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading location details:', err);
        alert('Error loading location details: ' + (err.error?.Message || err.message));
        this.modalLoading = false;
        this.showModal = false;
      }
    });
  }

  // Close modal
  closeModal(): void {
    this.showModal = false;
    this.resetForm();
    this.modalLoading = false;
  }

  // Handle file selection
  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      
      // Create preview
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreview = e.target.result;
        this.cdr.detectChanges();
      };
      reader.readAsDataURL(file);
    }
  }

  // Save location (create or update)
  saveLocation(): void {
    if (this.locationForm.invalid) {
      alert('Please fill all required fields correctly');
      return;
    }

    this.modalLoading = true;
    const formData = new FormData();
    
    // Append form fields
    formData.append('Name', this.locationForm.get('name')?.value);
    formData.append('Address', this.locationForm.get('address')?.value);
    formData.append('TotalSlots', this.locationForm.get('totalSlots')?.value.toString());
    formData.append('City', this.locationForm.get('city')?.value);
    
    // Append image file if selected
    if (this.selectedFile) {
      formData.append('ImageFile', this.selectedFile, this.selectedFile.name);
    }

    if (this.isEditMode && this.editingLocationId) {
      // Update existing location
      this.locationService.updateLocationWithFormData(this.editingLocationId, formData).subscribe({
        next: () => {
          alert('Location updated successfully');
          this.closeModal();
          this.loadLocations();
        },
        error: (err) => {
          console.error('Error updating location:', err);
          alert('Error updating location: ' + (err.error?.Message || err.message));
          this.modalLoading = false;
        }
      });
    } else {
      // Create new location
      this.locationService.createLocationWithFormData(formData).subscribe({
        next: () => {
          alert('Location created successfully');
          this.closeModal();
          this.loadLocations();
        },
        error: (err) => {
          console.error('Error creating location:', err);
          alert('Error creating location: ' + (err.error?.Message || err.message));
          this.modalLoading = false;
        }
      });
    }
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
