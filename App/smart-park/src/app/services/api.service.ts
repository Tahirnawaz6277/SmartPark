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


// Users
export interface UserDto {
  id: string; // Guid
  name: string;
  email?: string | null;
  address: string | null;
  phoneNumber?: string | null;
  city: string;
  roleId?: string | null; // Guid
  roleName?: string | null;
}

export interface UpdateUserRequest {
  Name: string;
  Address: string;
  PhoneNumber: string;
  City: string;
  Email: string ;
}

// Location request/response contracts (updated)
export interface LocationRequest {
  name: string;
  address: string;
  totalSlots: number;
  city: string;
  image: string;
}

export interface LocationResponse {
  Id: string;
  Name: string;
  Address: string;
  City?: string | null;
  TotalSlots?: number | null;
}

// this is Get All Locations and get Location by Id apis response
export interface SlotResponseDto {
  id: string;
  locationId: string;
  slotNumber: string;
  isAvailable: boolean;
}

export interface LocationDto {
  id: string;
  name: string;
  address: string;
  totalSlots: number;
  city: string;
  image: string;
  slots: SlotResponseDto[];
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

export interface ApiResponse<T> { success: boolean; data: T; message: string; }

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

  private unwrapResponse<T>() {
    return map((res: any) => (res && typeof res === 'object' && 'data' in res ? (res.data as T) : (res as T)));
  }

  // // User Authentication
  // login(credentials: LoginRequest): Observable<ApiResponse<LoginResponse>> {
  //   return this.http.post<ApiResponse<LoginResponse>>(
  //     `${this.baseUrl}/User/user-login`,
  //     credentials,
  //     this.httpOptions
  //   ).pipe(
  //     catchError(this.handleError)
  //   );
  // }
  login(credentials: LoginRequest) {
    return this.http.post<any>(
      `${this.baseUrl}/User/user-login`,
      credentials,
      this.httpOptions
    ).pipe(
      map(res => {
        // Case 1: new success format { success, message, data }
        if ('success' in res && 'message' in res) {
          return {
            success: res.success,
            message: res.message,
            data: res.data
          };
        }
  
        // Case 2: old error format { StatusCode, Message }
        if ('StatusCode' in res && 'Message' in res) {
          return {
            success: res.StatusCode === 200,
            message: res.Message,
            data: res.Data ?? null
          };
        }
  
        // fallback
        return {
          success: false,
          message: 'Unexpected response format',
          data: null
        };
      }),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => ({
          success: false,
          message: error.error?.Message || error.error?.message || 'Server error',
          data: null
        }));
      })
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
  getAllUsers(): Observable<UserDto[]> {
    return this.http.get<any>(
      `${this.baseUrl}/User/get-all-users`,
      { headers: this.authHeaders() }
    ).pipe(this.unwrapResponse<UserDto[]>(), catchError(this.handleError));
  }

  getUserById(id: string): Observable<UserDto> {
    return this.http.get<any>(
      `${this.baseUrl}/User/get-user-by/${id}`,
      { headers: this.authHeaders() }
    ).pipe(this.unwrapResponse<UserDto>(), catchError(this.handleError));
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
  getAllLocations(): Observable<LocationDto[]> {
    return this.http.get<any>(
      `${this.baseUrl}/Location/get-all-locations`,
      { headers: this.authHeaders() }
    ).pipe(this.unwrapResponse<LocationDto[]>(), catchError(this.handleError));
  }

  getLocationById(id: string): Observable<LocationDto> {
    return this.http.get<any>(
      `${this.baseUrl}/Location/get-location-by/${id}`,
      { headers: this.authHeaders() }
    ).pipe(this.unwrapResponse<LocationDto>(), catchError(this.handleError));
  }

  createLocation(locationData: LocationRequest): Observable<LocationResponse> {
    return this.http.post<any>(
      `${this.baseUrl}/Location/create-location`,
      locationData,
      { headers: this.authHeaders() }
    ).pipe(this.unwrapResponse<LocationResponse>(), catchError(this.handleError));
  }

  updateLocation(id: string, locationData: Partial<LocationRequest>): Observable<LocationResponse | LocationDto> {
    return this.http.put<any>(
      `${this.baseUrl}/Location/update-location/${id}`,
      locationData,
      { headers: this.authHeaders() }
    ).pipe(this.unwrapResponse<LocationResponse | LocationDto>(), catchError(this.handleError));
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
