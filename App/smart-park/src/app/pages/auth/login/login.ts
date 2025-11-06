import { HttpClient } from '@angular/common/http';
import { Component, NgModule } from '@angular/core';
import { Auth } from '../../../core/services/auth';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  loginForm!: FormGroup;
  loading = false;
  errorMessage = '';
  showPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: Auth,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Initialize form
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  // getter for form controls (for easy template access)
  get f() {
    return this.loginForm.controls;
  }

  // Toggle password visibility
  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
            console.log(this.loginForm.value);

      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.authService.login(this.loginForm.value).subscribe({
      next: (res) => {
        console.log('Login response:', res);
        
        // Auth service already handles token and role storage via tap operator
        const role = this.authService.getUserRole();
        console.log('User role from storage:', role);
        
        // Redirect based on role (case-insensitive check)
        if (role?.toLowerCase() === 'admin') {
          console.log('Redirecting to admin dashboard');
          this.router.navigate(['/admin/dashboard']);
        } else if (role?.toLowerCase() === 'driver') {
          console.log('Redirecting to driver dashboard');
          this.router.navigate(['/driver/dashboard']);
        } else {
          console.log('Unknown role, redirecting to login');
          this.router.navigate(['/login']); // fallback
        }
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        
        console.error('Login error:', err);
        
        // Extract error message from API response
        const apiMessage = err.error?.Message || err.error?.message;
        
        // User-friendly error messages
        if (err.status === 500 && apiMessage) {
          // Show API error message (e.g., "Invalid Credentails")
          this.errorMessage = apiMessage;
        } else if (err.status === 401 || err.status === 400) {
          this.errorMessage = apiMessage || 'Invalid email or password. Please try again.';
        } else if (err.status === 0) {
          this.errorMessage = 'Unable to connect to server. Please check your internet connection.';
        } else if (err.status === 500) {
          this.errorMessage = apiMessage || 'Server error. Please try again later.';
        } else {
          this.errorMessage = apiMessage || 'Login failed. Please try again.';
        }
      }
    });
  }
}