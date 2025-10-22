import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const token = localStorage.getItem('auth_token');

  // Clone request and add authorization header if token exists
  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // Handle 401 Unauthorized - Token expired or invalid
      // BUT exclude certain endpoints and check if user was actually logged in
      const isAuthEndpoint = req.url.includes('/user-login') || 
                            req.url.includes('/user-registration') ||
                            req.url.includes('/get-user-profile') ||
                            req.url.includes('/upload-profile-img');
      
      const currentUrl = router.url;
      const isOnLoginPage = currentUrl === '/login' || currentUrl === '/register';
      
      // Only redirect if:
      // 1. Got 401 error
      // 2. NOT an auth/profile endpoint
      // 3. User has a token (was logged in)
      // 4. NOT already on login page
      if (error.status === 401 && !isAuthEndpoint && token && !isOnLoginPage) {
        console.error('Token expired or unauthorized. Redirecting to login...');
        
        // Clear stored auth data
        localStorage.removeItem('auth_token');
        localStorage.removeItem('user_role');
        localStorage.removeItem('user_id');
        localStorage.removeItem('user_name');
        localStorage.removeItem('user_email');
        
        // Show toast message
        alert('Your session has expired. Please log in again.');
        
        // Redirect to login
        router.navigate(['/login']);
      }
      
      return throwError(() => error);
    })
  );
};
