import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  LocationCreateRequest,
  LocationDto,
  LocationUpdateRequest
} from './location.models';

@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private apiUrl = `${environment.apiUrl}Location`;

  constructor(private http: HttpClient) {}

  createLocation(data: LocationCreateRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/create-location`, data);
  }

  getLocationById(id: string): Observable<LocationDto> {
    return this.http.get<LocationDto>(`${this.apiUrl}/get-location-by/${id}`);
  }

  getAllLocations(): Observable<LocationDto[]> {
    return this.http.get<LocationDto[]>(`${this.apiUrl}/get-all-locations`);
  }

  updateLocation(id: string, data: LocationUpdateRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/update-location/${id}`, data);
  }

  deleteLocation(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/delete-location/${id}`);
  }
}
