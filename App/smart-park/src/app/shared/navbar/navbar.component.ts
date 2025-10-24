import { Component, OnInit, ChangeDetectorRef, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Auth } from '../../core/services/auth';
import { ProfileService, UserProfile } from '../../core/services/profile.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  userName: string | null = '';
  userProfile: UserProfile | null = null;
  showProfileModal = false;
  profileLoading = false;
  uploadLoading = false;
  selectedFile: File | null = null;
  previewUrl: string | null = null;
  isDropdownOpen = false;

  @Output() sidebarToggle = new EventEmitter<void>();

  constructor(
    public authService: Auth,
    private profileService: ProfileService,
    private cdr: ChangeDetectorRef
  ) {
    this.userName = this.authService.getUserName();
  }

  ngOnInit(): void {
    // Only load profile image if user is logged in
    if (this.authService.isLoggedIn()) {
      this.loadProfileImage();
    }
  }

  loadProfileImage(): void {
    // Only fetch if user is authenticated
    if (!this.authService.isLoggedIn()) {
      return;
    }
    
    this.profileService.getUserProfile().subscribe({
      next: (profile) => {
        this.userProfile = profile;
      },
      error: (err) => {
        console.error('Error loading profile image:', err);
        // Don't show error to user, just use default icon
      }
    });
  }

  openProfileModal(): void {
    this.showProfileModal = true;
    this.profileLoading = true;
    this.selectedFile = null;
    this.previewUrl = null;
    this.cdr.detectChanges();
    
    console.log('Opening profile modal, loading profile...');
    
    this.profileService.getUserProfile().subscribe({
      next: (profile) => {
        console.log('Profile loaded successfully:', profile);
        this.userProfile = profile;
        this.profileLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading profile:', err);
        alert('Error loading profile: ' + (err.error?.Message || err.message));
        this.profileLoading = false;
        this.showProfileModal = false;
        this.cdr.detectChanges();
      }
    });
  }

  closeProfileModal(): void {
    this.showProfileModal = false;
    this.selectedFile = null;
    this.previewUrl = null;
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      
      // Create preview
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.previewUrl = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  uploadProfileImage(): void {
    if (!this.selectedFile || !this.userProfile) {
      alert('Please select an image first');
      return;
    }

    this.uploadLoading = true;
    this.cdr.detectChanges();
    
    this.profileService.uploadProfileImage(this.userProfile.id, this.selectedFile).subscribe({
      next: (response) => {
        console.log('Image uploaded successfully:', response);
        alert('Profile image updated successfully!');
        
        // Reload profile to get new image URL
        this.profileService.getUserProfile().subscribe({
          next: (profile) => {
            this.userProfile = profile;
            this.uploadLoading = false;
            this.selectedFile = null;
            this.previewUrl = null;
            this.cdr.detectChanges();
          }
        });
      },
      error: (err) => {
        console.error('Error uploading image:', err);
        alert('Error uploading image: ' + (err.error?.Message || err.message));
        this.uploadLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  toggleDropdown(): void {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  logout(): void {
    this.authService.logout();
  }

  toggleSidebar(): void {
    this.sidebarToggle.emit();
  }
}
