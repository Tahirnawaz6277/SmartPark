import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { AuthService } from './auth.service';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';


//
export interface LoginRequest {
  Email: string;
  Password: string;
}

export interface RegistrationRequest {
  Name: string;
  Email: string;
  Password: string;
  Address?: string | null;
  PhoneNumber?: string | null;
  City: string;
}

export interface RegistrationResponse {
  Id: string; // Guid as string
  Name?: string | null;
  Email: string | "";
  PhoneNumber?: string | null;
}

export interface LoginResponse {
  name?: string | null;
  email?: string | null;
  accessToken?: string | null;
}


export interface UserDto {
  Id: string; // Guid
  Name: string;
  Email?: string | null;
  Address: string | null;
  PhoneNumber?: string | null;
  City: string;
  RoleId: string; // Guid
  RoleName: string; 
}

export interface UpdateUserRequest {
  Name: string;
  Address: string;
  PhoneNumber: string;
  City: string;
  Email: string ;
}

// this is Create Location api request
export interface CreateLocationRequest {
  Name: string;
  Address: string;
  SmallSlotCount: number; 
  LargeSlotCount: number;
  MediumSlotCount: number;
  city: string;
  image: string;
}
// this is Create Location api response
export interface CreateLocationReponse {
  LocationId: string;
  Name?: string | null;
  Address?: string | null;
  TotalSlots?: number | null;
  City?: string | null;
}

// this is Get All Locations and get Location by Id apis response
export interface LocationDto {
  Id: string;
  Name?: string | null;
  Address?: string | null;
  TotalSlots?: number | null;
  City?: string | null;
  Image?: string | null;
  UserId?: string | null;
  TimeStamp?: Date | null;
  Slots: SlotSummaryDto[];
}

export interface SlotSummaryDto {
  SlotType: string;
  SlotCount: number;
  IsAvailable?: boolean | null;
}


// export interface SlotRequest {
//   locationId: string;
//   slotType: string;
//   isAvailable: boolean;
// }

// export interface SlotResponse {
//   id: string;
//   locationId: string;
//   slotType: string;
//   isAvailable: boolean;
// }

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
  login(credentials: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(
      `${this.baseUrl}/User/user-login`,
      credentials,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  register(userData: RegistrationRequest): Observable<ApiResponse<RegistrationResponse>> {
    return this.http.post<ApiResponse<RegistrationResponse>>(
      `${this.baseUrl}/User/user-registration`,
      userData,
      this.httpOptions
    ).pipe(
      catchError(this.handleError)
    );
  }

  // User CRUD Operations
  getAllUsers(): Observable<ApiResponse<UserDto[]>> {
    return this.http.get<ApiResponse<UserDto[]>>(
      `${this.baseUrl}/User/get-all-users`,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  getUserById(id: string): Observable<ApiResponse<UserDto>> {
    return this.http.get<ApiResponse<UserDto>>(
      `${this.baseUrl}/User/get-user-by/${id}`,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  updateUser(id: string, userData: Partial<UpdateUserRequest>): Observable<ApiResponse<RegistrationResponse>> {
    return this.http.put<ApiResponse<RegistrationResponse>>(
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
  getAllLocations(): Observable<ApiResponse<LocationDto[]>> {
    return this.http.get<ApiResponse<LocationDto[]>>(
      `${this.baseUrl}/Location/get-all-locations`,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  getLocationById(id: string): Observable<ApiResponse<LocationDto>> {
    return this.http.get<ApiResponse<LocationDto>>(
      `${this.baseUrl}/Location/get-location-by/${id}`,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  createLocation(locationData: CreateLocationRequest): Observable<ApiResponse<CreateLocationReponse>> {
    return this.http.post<ApiResponse<CreateLocationReponse>>(
      `${this.baseUrl}/Location/create-location`,
      locationData,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  updateLocation(id: string, locationData: Partial<CreateLocationRequest>): Observable<ApiResponse<LocationDto>> {
    return this.http.put<ApiResponse<LocationDto>>(
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
