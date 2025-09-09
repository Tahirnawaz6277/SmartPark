import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { ApiService, UserRegistrationRequest } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})
export class SignupComponent implements OnInit {
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Check if user is already logged in
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
    }
  }

  onSubmit(form: NgForm): void {
    if (form.valid && form.value.password === form.value.confirmPassword) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      const registrationData: UserRegistrationRequest = {
        fullName: form.value.fullName,
        email: form.value.email,
        password: form.value.password
      };

      this.apiService.register(registrationData).subscribe({
        next: (response) => {
          this.isLoading = false;
          if (response.success && response.data) {
            this.successMessage = 'Registration successful! Redirecting to login...';
            
            // Redirect to login after a short delay
            setTimeout(() => {
              this.router.navigate(['/login']);
            }, 2000);
          } else {
            this.errorMessage = response.message || 'Registration failed. Please try again.';
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.message || 'An error occurred during registration. Please try again.';
        }
      });
    } else {
      if (form.value.password !== form.value.confirmPassword) {
        this.errorMessage = 'Passwords do not match!';
      } else {
        this.errorMessage = 'Please fill in all required fields correctly.';
      }
    }
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}
