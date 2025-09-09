import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

export interface UserLoginRequest {
  username: string;
  password: string;
}

export interface UserRegistrationRequest {
  fullName: string;
  email: string;
  password: string;
}

export interface UserResponse {
  id: string;
  fullName: string;
  email: string;
  username?: string;
}

export interface LocationRequest {
  name: string;
  address: string;
  capacity: number;
  hourlyRate: number;
  description?: string;
}

export interface LocationResponse {
  id: string;
  name: string;
  address: string;
  capacity: number;
  hourlyRate: number;
  description?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'https://localhost:7188/api';
  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    })
  };

  constructor(private http: HttpClient) {}

  // User Authentication
  login(credentials: UserLoginRequest): Observable<ApiResponse<UserResponse>> {
    return this.http.post<ApiResponse<UserResponse>>(
      `${this.baseUrl}/User/user-login`,
      credentials,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  register(userData: UserRegistrationRequest): Observable<ApiResponse<UserResponse>> {
    return this.http.post<ApiResponse<UserResponse>>(
      `${this.baseUrl}/User/user-registration`,
      userData,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  // User CRUD Operations
  getAllUsers(): Observable<ApiResponse<UserResponse[]>> {
    return this.http.get<ApiResponse<UserResponse[]>>(
      `${this.baseUrl}/User/get-all-users`,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  getUserById(id: string): Observable<ApiResponse<UserResponse>> {
    return this.http.get<ApiResponse<UserResponse>>(
      `${this.baseUrl}/User/get-user-by/${id}`,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  updateUser(id: string, userData: Partial<UserRegistrationRequest>): Observable<ApiResponse<UserResponse>> {
    return this.http.put<ApiResponse<UserResponse>>(
      `${this.baseUrl}/User/update-user/${id}`,
      userData,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  deleteUser(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(
      `${this.baseUrl}/User/delete-user/${id}`,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  // Location CRUD Operations
  getAllLocations(): Observable<ApiResponse<LocationResponse[]>> {
    return this.http.get<ApiResponse<LocationResponse[]>>(
      `${this.baseUrl}/Location/get-all-locations`,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  getLocationById(id: string): Observable<ApiResponse<LocationResponse>> {
    return this.http.get<ApiResponse<LocationResponse>>(
      `${this.baseUrl}/Location/get-location-by/${id}`,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  createLocation(locationData: LocationRequest): Observable<ApiResponse<LocationResponse>> {
    return this.http.post<ApiResponse<LocationResponse>>(
      `${this.baseUrl}/Location/create-location`,
      locationData,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  updateLocation(id: string, locationData: Partial<LocationRequest>): Observable<ApiResponse<LocationResponse>> {
    return this.http.put<ApiResponse<LocationResponse>>(
      `${this.baseUrl}/Location/update-location/${id}`,
      locationData,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  deleteLocation(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(
      `${this.baseUrl}/Location/delete-location/${id}`,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred!';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      if (error.status === 0) {
        errorMessage = 'Unable to connect to server. Please check your internet connection.';
      } else if (error.status === 401) {
        errorMessage = 'Invalid credentials. Please check your username and password.';
      } else if (error.status === 403) {
        errorMessage = 'Access denied. You do not have permission to perform this action.';
      } else if (error.status === 404) {
        errorMessage = 'The requested resource was not found.';
      } else if (error.status === 500) {
        errorMessage = 'Internal server error. Please try again later.';
      } else {
        errorMessage = error.error?.message || `Error Code: ${error.status}\nMessage: ${error.message}`;
      }
    }
    
    return throwError(() => new Error(errorMessage));
  }
}
