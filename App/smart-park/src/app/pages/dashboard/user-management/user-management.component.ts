import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ApiService, UserResponse, UserRegistrationRequest } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.scss'
})
export class UserManagementComponent implements OnInit {
  users: UserResponse[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  showAddForm = false;
  editingUser: UserResponse | null = null;

  // Form data for add/edit
  formData: UserRegistrationRequest = {
    fullName: '',
    email: '',
    password: ''
  };

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.apiService.getAllUsers().subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success && response.data) {
          this.users = response.data;
        } else {
          this.errorMessage = response.message || 'Failed to load users.';
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.message || 'An error occurred while loading users.';
      }
    });
  }

  getUserById(id: string): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.apiService.getUserById(id).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success && response.data) {
          this.editingUser = response.data;
          this.formData = {
            fullName: response.data.fullName,
            email: response.data.email,
            password: ''
          };
          this.showAddForm = true;
        } else {
          this.errorMessage = response.message || 'Failed to load user details.';
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.message || 'An error occurred while loading user details.';
      }
    });
  }

  addUser(): void {
    this.editingUser = null;
    this.formData = {
      fullName: '',
      email: '',
      password: ''
    };
    this.showAddForm = true;
    this.clearMessages();
  }

  editUser(user: UserResponse): void {
    this.getUserById(user.id);
  }

  saveUser(form: NgForm): void {
    if (form.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      if (this.editingUser) {
        // Update existing user
        this.apiService.updateUser(this.editingUser.id, this.formData).subscribe({
          next: (response) => {
            this.isLoading = false;
            if (response.success) {
              this.successMessage = 'User updated successfully!';
              this.loadUsers();
              this.closeForm();
            } else {
              this.errorMessage = response.message || 'Failed to update user.';
            }
          },
          error: (error) => {
            this.isLoading = false;
            this.errorMessage = error.message || 'An error occurred while updating user.';
          }
        });
      } else {
        // Create new user
        this.apiService.register(this.formData).subscribe({
          next: (response) => {
            this.isLoading = false;
            if (response.success) {
              this.successMessage = 'User created successfully!';
              this.loadUsers();
              this.closeForm();
            } else {
              this.errorMessage = response.message || 'Failed to create user.';
            }
          },
          error: (error) => {
            this.isLoading = false;
            this.errorMessage = error.message || 'An error occurred while creating user.';
          }
        });
      }
    } else {
      this.errorMessage = 'Please fill in all required fields correctly.';
    }
  }

  deleteUser(user: UserResponse): void {
    if (confirm(`Are you sure you want to delete user "${user.fullName}"?`)) {
      this.isLoading = true;
      this.errorMessage = '';
      
      this.apiService.deleteUser(user.id).subscribe({
        next: (response) => {
          this.isLoading = false;
          if (response.success) {
            this.successMessage = 'User deleted successfully!';
            this.loadUsers();
          } else {
            this.errorMessage = response.message || 'Failed to delete user.';
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.message || 'An error occurred while deleting user.';
        }
      });
    }
  }

  closeForm(): void {
    this.showAddForm = false;
    this.editingUser = null;
    this.formData = {
      fullName: '',
      email: '',
      password: ''
    };
    this.clearMessages();
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}
