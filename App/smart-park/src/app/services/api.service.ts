import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { AuthService } from './auth.service';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

export interface UserLoginRequestDto {
  Email: string;
  Password: string;
}

export interface UserRequestDto {
  Name: string;
  Email: string;
  Password: string;
  Address?: string | null;
  PhoneNumber?: string | null;
  City: string;
}

export interface UserResponseDto {
  Id: string; // Guid as string
  Name?: string | null;
  Email: string | "";
  PhoneNumber?: string | null;
}

export interface UserLoginResponse {
  Name?: string | null;
  Email?: string | null;
  AccessToken?: string | null;
}

export interface LocationRequest {
  name: string;
  address: string;
  totalSlots: number;
  city: string;
  image: string;
  userId: string; // Guid
}

export interface LocationResponse {
  id: string;
  name: string;
  address: string;
  totalSlots: number;
  city: string;
  image: string;
  userId: string;
}

export interface SlotRequest {
  locationId: string;
  slotType: string;
  isAvailable: boolean;
}

export interface SlotResponse {
  id: string;
  locationId: string;
  slotType: string;
  isAvailable: boolean;
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

  constructor(private http: HttpClient, private auth: AuthService) {}

  private authHeaders(): HttpHeaders {
    const token = this.auth.getToken();
    let headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    });
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }
    return headers;
  }

  // User Authentication
  login(credentials: UserLoginRequestDto): Observable<ApiResponse<UserLoginResponse>> {
    return this.http.post<ApiResponse<UserLoginResponse>>(
      `${this.baseUrl}/User/user-login`,
      credentials,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  register(userData: UserRequestDto): Observable<ApiResponse<UserResponseDto>> {
    return this.http.post<ApiResponse<UserResponseDto>>(
      `${this.baseUrl}/User/user-registration`,
      userData,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  // User CRUD Operations
  getAllUsers(): Observable<ApiResponse<UserResponseDto[]>> {
    return this.http.get<ApiResponse<UserResponseDto[]>>(
      `${this.baseUrl}/User/get-all-users`,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  getUserById(id: string): Observable<ApiResponse<UserResponseDto>> {
    return this.http.get<ApiResponse<UserResponseDto>>(
      `${this.baseUrl}/User/get-user-by/${id}`,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  updateUser(id: string, userData: Partial<UserRequestDto>): Observable<ApiResponse<UserResponseDto>> {
    return this.http.put<ApiResponse<UserResponseDto>>(
      `${this.baseUrl}/User/update-user/${id}`,
      userData,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  deleteUser(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(
      `${this.baseUrl}/User/delete-user/${id}`,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  // Location CRUD Operations
  getAllLocations(): Observable<ApiResponse<LocationResponse[]>> {
    return this.http.get<ApiResponse<LocationResponse[]>>(
      `${this.baseUrl}/Location/get-all-locations`,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  getLocationById(id: string): Observable<ApiResponse<LocationResponse>> {
    return this.http.get<ApiResponse<LocationResponse>>(
      `${this.baseUrl}/Location/get-location-by/${id}`,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  createLocation(locationData: LocationRequest): Observable<ApiResponse<LocationResponse>> {
    return this.http.post<ApiResponse<LocationResponse>>(
      `${this.baseUrl}/Location/create-location`,
      locationData,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  updateLocation(id: string, locationData: Partial<LocationRequest>): Observable<ApiResponse<LocationResponse>> {
    return this.http.put<ApiResponse<LocationResponse>>(
      `${this.baseUrl}/Location/update-location/${id}`,
      locationData,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  deleteLocation(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(
      `${this.baseUrl}/Location/delete-location/${id}`,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  // Slot APIs
  addSlot(payload: SlotRequest): Observable<ApiResponse<SlotResponse>> {
    return this.http.post<ApiResponse<SlotResponse>>(
      `${this.baseUrl}/Slot/add-slot`,
      payload,
      { headers: this.authHeaders() }
    ).pipe(catchError(this.handleError));
  }

  updateSlot(id: string, payload: SlotRequest): Observable<ApiResponse<SlotResponse>> {
    return this.http.put<ApiResponse<SlotResponse>>(
      `${this.baseUrl}/Slot/update-slot/${id}`,
      payload,
      { headers: this.authHeaders() }
    ).pipe(catchError(this.handleError));
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
        errorMessage = 'Invalid credentials. Please check your email and password.';
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
