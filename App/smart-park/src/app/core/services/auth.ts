import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { LoginRequest, LoginResponse, RegistrationRequest } from '../models/auth.model';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class Auth {

    constructor(private http: HttpClient, private router: Router) {}

    register(data: RegistrationRequest): Observable<any> {
      return this.http.post(`${environment.apiUrl}User/user-registration`, data);
    }

    login(credentials: LoginRequest): Observable<LoginResponse> {
      return this.http.post<LoginResponse>(`${environment.apiUrl}User/user-login`, credentials)
        .pipe(
          tap(response => {
            console.log('Auth Service - Full login response:', response);
            
            // Handle nested response structure (data.accessToken, data.role)
            const token = response.data?.accessToken || response.token;
            const email = response.data?.email || response.email;
            const name = response.data?.name || response.name;
            
            if (token) {
              this.setToken(token);
              console.log('Auth Service - Token saved');
              
              // Decode JWT token to extract role and other claims
              try {
                const decodedToken: any = jwtDecode(token);
                console.log('Auth Service - Decoded token:', decodedToken);
                
                // Extract role from JWT claims (Microsoft identity claims format)
                const role = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] 
                          || decodedToken['role']
                          || response.data?.role 
                          || response.role;
                
                // Extract user ID from JWT claims
                const userId = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
                            || decodedToken['sub']
                            || response.data?.id
                            || response.id;
                
                // Extract email from JWT claims
                const userEmail = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']
                               || decodedToken['email']
                               || email;
                
                // Extract name from JWT claims
                const userName = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
                              || decodedToken['name']
                              || name;
                
                if (role) {
                  this.setUserRole(role);
                  console.log('Auth Service - Role extracted from token:', role);
                } else {
                  console.warn('Auth Service - No role found in token!');
                }
                
                if (userId) {
                  localStorage.setItem('user_id', userId);
                }
                if (userName) {
                  localStorage.setItem('user_name', userName);
                }
                if (userEmail) {
                  localStorage.setItem('user_email', userEmail);
                }
              } catch (error) {
                console.error('Auth Service - Error decoding token:', error);
              }
            } else {
              console.error('Auth Service - No token in response!');
            }
          })
        );
    }

    setToken(token: string): void {
      localStorage.setItem('auth_token', token);
    }

    getToken(): string | null {
      return localStorage.getItem('auth_token');
    }

    setUserRole(role: string): void {
      localStorage.setItem('user_role', role);
    }

    getUserRole(): string | null {
      return localStorage.getItem('user_role');
    }

    getUserId(): string | null {
      return localStorage.getItem('user_id');
    }

    getUserName(): string | null {
      return localStorage.getItem('user_name');
    }

    getUserEmail(): string | null {
      return localStorage.getItem('user_email');
    }

    isLoggedIn(): boolean {
      return !!this.getToken();
    }

    logout(): void {
      localStorage.removeItem('auth_token');
      localStorage.removeItem('user_role');
      localStorage.removeItem('user_id');
      localStorage.removeItem('user_name');
      localStorage.removeItem('user_email');
      this.router.navigate(['/login']);
    }

}
