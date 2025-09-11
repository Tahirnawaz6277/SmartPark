import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ApiService, RegistrationResponse, RegistrationRequest ,UserDto, UpdateUserRequest} from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.scss'
})
export class UserManagementComponent implements OnInit {
  users: UserDto[] = [];
  selectedUser: UserDto | null = null;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  showAddForm = false;
  editingUserId: string | null = null;

  // Form data for add/edit
  formData: RegistrationRequest = {
    Name: '',
    Email: '',
    Password: '',
    Address: null,
    PhoneNumber: null,
    City: ''
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
        this.users = response || [];
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.message || 'An error occurred while loading users.';
        this.autoDismissMessages();
      }
    });
  }

  getUserById(id: string): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.apiService.getUserById(id).subscribe({
      next: (response) => {
        this.isLoading = false;
        const user = response as unknown as UserDto;
        this.selectedUser = user as unknown as UserDto;
        this.formData = {
          Name: (user as any).name ?? '',
          Email: (user as any).email ?? '',
          Password: '',
          Address: (user as any).address ?? '',
          PhoneNumber: (user as any).phoneNumber ?? null,
          City: (user as any).city ?? ''
        };
        this.editingUserId = (user as any).id;
        this.showAddForm = true;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.message || 'An error occurred while loading user details.';
        this.autoDismissMessages();
      }
    });
  }

  addUser(): void {
    this.editingUserId = null;
    this.formData = {
      Name: '',
      Email: '',
      Password: '',
      Address: null,
      PhoneNumber: null,
      City: ''
    };
    this.showAddForm = true;
    this.clearMessages();
  }

  editUser(user: RegistrationResponse | UserDto): void {
    const id = (user as any).Id ?? (user as any).id;
    this.getUserById(id);
  }

  saveUser(form: NgForm): void {
    if (form.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      if (this.editingUserId) {
        // Update existing user
        this.apiService.updateUser(this.editingUserId, {
          Name: this.formData.Name,
          Address: this.formData.Address ?? '',
          PhoneNumber: this.formData.PhoneNumber ?? '',
          City: this.formData.City,
          Email: this.formData.Email
        }).subscribe({
          next: (response) => {
            this.isLoading = false;
            if (response.success) {
              this.successMessage = 'User updated successfully!';
              this.loadUsers();
              this.closeForm();
              this.autoDismissMessages();
            } else {
              this.errorMessage = response.message || 'Failed to update user.';
              this.autoDismissMessages();
            }
          },
          error: (error) => {
            this.isLoading = false;
            this.errorMessage = error.message || 'An error occurred while updating user.';
            this.autoDismissMessages();
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
              this.autoDismissMessages();
            } else {
              this.errorMessage = response.message || 'Failed to create user.';
              this.autoDismissMessages();
            }
          },
          error: (error) => {
            this.isLoading = false;
            this.errorMessage = error.message || 'An error occurred while creating user.';
            this.autoDismissMessages();
          }
        });
      }
    } else {
      this.errorMessage = 'Please fill in all required fields correctly.';
      this.autoDismissMessages();
    }
  }

  deleteUser(user: RegistrationResponse | UserDto): void {
    const name = (user as any).Name ?? (user as any).name ?? '';
    if (confirm(`Are you sure you want to delete user "${name}"?`)) {
      this.isLoading = true;
      this.errorMessage = '';
      
      const id = (user as any).Id ?? (user as any).id;
      this.apiService.deleteUser(id).subscribe({
        next: (response) => {
          this.isLoading = false;
          if (response.success) {
            this.successMessage = 'User deleted successfully!';
            this.loadUsers();
            this.autoDismissMessages();
          } else {
            this.errorMessage = response.message || 'Failed to delete user.';
            this.autoDismissMessages();
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.message || 'An error occurred while deleting user.';
          this.autoDismissMessages();
        }
      });
    }
  }

  closeForm(): void {
    this.showAddForm = false;
    this.editingUserId = null;
    this.formData = {
      Name: '',
      Email: '',
      Password: '',
      Address: null,
      PhoneNumber: null,
      City: ''
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
