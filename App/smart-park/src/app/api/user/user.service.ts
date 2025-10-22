import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  UserRegistrationRequest,
  UserLoginRequest,
  UserLoginResponse,
  UserDto,
  UserUpdateRequest
} from './user.models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.apiUrl}User`;

  constructor(private http: HttpClient) {}

  register(data: UserRegistrationRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/user-registration`, data);
  }

  login(credentials: UserLoginRequest): Observable<UserLoginResponse> {
    return this.http.post<UserLoginResponse>(`${this.apiUrl}/user-login`, credentials);
  }

  getUserById(id: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/get-user-by/${id}`);
  }

  getAllUsers(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(`${this.apiUrl}/get-all-users`);
  }

  updateUser(id: string, data: UserUpdateRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/update-user/${id}`, data);
  }

  deleteUser(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/delete-user/${id}`);
  }
}
