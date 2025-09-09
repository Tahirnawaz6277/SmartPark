import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { UserResponseDto, UserLoginResponse } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<UserResponseDto | UserLoginResponse | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private isLoggedInSubject = new BehaviorSubject<boolean>(false);
  public isLoggedIn$ = this.isLoggedInSubject.asObservable();

  constructor() {
    // Check if user is already logged in (from localStorage)
    const savedUser = localStorage.getItem('currentUser');
    if (savedUser) {
      try {
        const user = JSON.parse(savedUser);
        this.currentUserSubject.next(user);
        this.isLoggedInSubject.next(true);
      } catch (error) {
        // Invalid data in localStorage, clear it
        localStorage.removeItem('currentUser');
      }
    }

    // Restore token separately if present
    const token = localStorage.getItem('accessToken');
    if (token && !this.isLoggedInSubject.value) {
      this.isLoggedInSubject.next(true);
    }
  }

  setCurrentUser(user: UserResponseDto | UserLoginResponse): void {
    this.currentUserSubject.next(user);
    this.isLoggedInSubject.next(true);
    localStorage.setItem('currentUser', JSON.stringify(user));

    // Persist access token if available
    const maybeToken = (user as UserLoginResponse)?.AccessToken;
    if (maybeToken) {
      localStorage.setItem('accessToken', maybeToken);
    }
  }

  getCurrentUser(): UserResponseDto | UserLoginResponse | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return this.isLoggedInSubject.value;
  }

  logout(): void {
    this.currentUserSubject.next(null);
    this.isLoggedInSubject.next(false);
    localStorage.removeItem('currentUser');
    localStorage.removeItem('accessToken');
  }

  getToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  getRole(): string | null {
    const token = this.getToken();
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1] || ''));
      return payload['role'] || payload['Role'] || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
    } catch {
      return null;
    }
  }

  isInRole(role: string): boolean {
    const currentRole = this.getRole();
    return (currentRole || '').toLowerCase() === role.toLowerCase();
  }
}
