import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  BookingCreateRequest,
  BookingDto,
  BookingUpdateRequest,
  BookingHistoryDto
} from './booking.models';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private apiUrl = `${environment.apiUrl}Booking`;

  constructor(private http: HttpClient) {}

  createBooking(data: BookingCreateRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/create-booking`, data);
  }

  getBookingById(id: string): Observable<BookingDto> {
    return this.http.get<BookingDto>(`${this.apiUrl}/get-booking-by/${id}`);
  }

  getAllBookings(): Observable<BookingDto[]> {
    return this.http.get<BookingDto[]>(`${this.apiUrl}/get-all-bookings`);
  }

  updateBooking(id: string, data: BookingUpdateRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/update-booking/${id}`, data);
  }

  cancelBooking(id: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/cancel-booking/${id}`, {});
  }

  deleteBooking(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/delete-booking/${id}`);
  }

  getBookingHistories(bookingId: string): Observable<BookingHistoryDto[]> {
    return this.http.get<BookingHistoryDto[]>(`${this.apiUrl}/get-booking-histories?bookingId=${bookingId}`);
  }

  getBookingHistoryById(id: string): Observable<BookingHistoryDto> {
    return this.http.get<BookingHistoryDto>(`${this.apiUrl}/get-booking-history-by/${id}`);
  }

  getUnpaidBookings(): Observable<BookingDto[]> {
    return this.http.get<BookingDto[]>(`${this.apiUrl}/get-unpaid-bookings`);
  }
}
