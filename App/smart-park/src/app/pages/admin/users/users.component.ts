import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, NavigationEnd } from '@angular/router';
import { UserService } from '../../../api/user/user.service';
import { UserDto } from '../../../api/user/user.models';
import { Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit, OnDestroy {
  users: UserDto[] = [];
  loading = true;
  searchTerm = '';
  private routerSubscription?: Subscription;
  
  // Modal state
  showModal = false;
  modalMode: 'view' | 'add' | 'edit' = 'view';
  selectedUser: UserDto | null = null;
  userForm!: FormGroup;
  modalLoading = false;

  constructor(
    private userService: UserService,
    private cdr: ChangeDetectorRef,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.initForm();
  }

  initForm(): void {
    this.userForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: [''],
      phoneNumber: [''],
      city: ['', Validators.required],
      roleName: [''],
      roleId: ['']
    });
  }

  ngOnInit(): void {
    this.loadUsers();
    
    // Reload data when navigating back to this component
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        if (event.url.includes('/admin/users')) {
          console.log('Navigated to users, reloading data...');
          this.loadUsers();
        }
      });
  }

  ngOnDestroy(): void {
    this.routerSubscription?.unsubscribe();
  }

  loadUsers(): void {
    this.loading = true;
    this.cdr.detectChanges();
    
    console.log('Loading users...');
    this.userService.getAllUsers().subscribe({
      next: (data) => {
        console.log('Users loaded successfully:', data);
        console.log('Number of users:', data?.length || 0);
        
        this.users = Array.isArray(data) ? data : [];
        this.loading = false;
        this.cdr.detectChanges();
        
        console.log('Users assigned to component:', this.users.length);
      },
      error: (err) => {
        console.error('Error loading users:', err);
        alert('Error loading users: ' + (err.error?.Message || err.message));
        this.users = [];
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  get filteredUsers(): UserDto[] {
    if (!this.searchTerm) {
      return this.users;
    }
    return this.users.filter(user =>
      user.name?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      user.email?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      user.city?.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  // View user details
  viewUser(id: string): void {
    this.modalLoading = true;
    this.modalMode = 'view';
    this.showModal = true;
    
    this.userService.getUserById(id).subscribe({
      next: (user) => {
        this.selectedUser = user;
        this.userForm.patchValue(user);
        this.userForm.disable();
        this.modalLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading user:', err);
        alert('Error loading user: ' + (err.error?.Message || err.message));
        this.showModal = false;
        this.modalLoading = false;
      }
    });
  }

  // Open add user modal
  openAddModal(): void {
    this.modalMode = 'add';
    this.selectedUser = null;
    this.userForm.reset();
    this.userForm.enable();
    this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.showModal = true;
  }

  // Open edit user modal
  editUser(id: string): void {
    this.modalLoading = true;
    this.modalMode = 'edit';
    this.showModal = true;
    
    this.userService.getUserById(id).subscribe({
      next: (user) => {
        console.log('User data loaded for edit:', user);
        this.selectedUser = user;
        this.userForm.patchValue({
          name: user.name,
          email: user.email,
          phoneNumber: user.phoneNumber,
          city: user.city,
          roleName: user.roleName,
          roleId: user.roleId
        });
        this.userForm.enable();
        // Remove password field from form for edit mode
        this.userForm.removeControl('password');
        this.modalLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading user:', err);
        alert('Error loading user: ' + (err.error?.Message || err.message));
        this.showModal = false;
        this.modalLoading = false;
      }
    });
  }

  // Save user (add or update)
  saveUser(): void {
    if (this.userForm.invalid) {
      alert('Please fill all required fields correctly');
      return;
    }

    this.modalLoading = true;
    const formData = this.userForm.getRawValue();

    if (this.modalMode === 'add') {
      // Create new user
      this.userService.register(formData).subscribe({
        next: () => {
          alert('User created successfully');
          this.closeModal();
          this.loadUsers();
        },
        error: (err) => {
          console.error('Error creating user:', err);
          alert('Error creating user: ' + (err.error?.Message || err.message));
          this.modalLoading = false;
        }
      });
    } else if (this.modalMode === 'edit' && this.selectedUser) {
      // Update existing user (without password and address)
      const updateData = {
        name: formData.name,
        email: formData.email,
        phoneNumber: formData.phoneNumber,
        city: formData.city
      };
      
      this.userService.updateUser(this.selectedUser.id, updateData).subscribe({
        next: () => {
          alert('User updated successfully');
          this.closeModal();
          this.loadUsers();
        },
        error: (err) => {
          console.error('Error updating user:', err);
          alert('Error updating user: ' + (err.error?.Message || err.message));
          this.modalLoading = false;
        }
      });
    }
  }

  // Close modal
  closeModal(): void {
    this.showModal = false;
    this.selectedUser = null;
    this.userForm.reset();
    this.modalLoading = false;
    
    // Re-add password control if it was removed
    if (!this.userForm.contains('password')) {
      this.userForm.addControl('password', this.fb.control(''));
    }
  }

  // Delete user
  deleteUser(id: string): void {
    if (confirm('Are you sure you want to delete this user?')) {
      this.userService.deleteUser(id).subscribe({
        next: () => {
          console.log('User deleted successfully');
          alert('User deleted successfully');
          this.loadUsers();
        },
        error: (err) => {
          console.error('Error deleting user:', err);
          alert('Error deleting user: ' + (err.error?.Message || err.message));
        }
      });
    }
  }
}
