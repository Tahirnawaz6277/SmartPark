import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Auth } from '../../../core/services/auth';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './signup.html',
  styleUrl: './signup.scss'
})
export class Signup implements OnInit {
  signupForm!: FormGroup;
  loading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: Auth,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.signupForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      phoneNumber: ['', [Validators.pattern(/^[0-9]{10,15}$/)]],
      city: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  // Custom validator to check if passwords match
  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  get f() {
    return this.signupForm.controls;
  }

  onSubmit(): void {
    if (this.signupForm.invalid) {
      Object.keys(this.signupForm.controls).forEach(key => {
        this.signupForm.controls[key].markAsTouched();
      });
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    // Remove confirmPassword before sending to API
    const { confirmPassword, ...registrationData } = this.signupForm.value;

    this.authService.register(registrationData).subscribe({
      next: (res) => {
        this.loading = false;
        this.successMessage = 'Registration successful! Redirecting to login...';
        
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        this.loading = false;
        
        // User-friendly error messages
        if (err.status === 409 || err.status === 400) {
          this.errorMessage = err.error?.message || 'Email already exists or invalid data. Please try again.';
        } else if (err.status === 0) {
          this.errorMessage = 'Unable to connect to server. Please check your internet connection.';
        } else if (err.status === 500) {
          this.errorMessage = 'Server error. Please try again later.';
        } else {
          this.errorMessage = err.error?.message || 'Registration failed. Please try again.';
        }
        console.error('Registration error:', err);
      }
    });
  }
}
