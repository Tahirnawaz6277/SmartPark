import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface UserProfile {
  id: string;
  name: string;
  email: string;
  phoneNumber: string;
  city: string;
  profileImageUrl: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private apiUrl = `${environment.apiUrl}User`;

  constructor(private http: HttpClient) {}

  getUserProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/get-user-profile`);
  }

  uploadProfileImage(userId: string, imageFile: File): Observable<any> {
    const formData = new FormData();
    formData.append('UserId', userId);
    formData.append('ImageFile', imageFile);

    return this.http.post(`${this.apiUrl}/upload-profile-img`, formData);
  }
}
