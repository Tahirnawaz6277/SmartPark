import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  BillingCreateRequest,
  BillingDto,
  BillingUpdateRequest
} from './billing.models';

@Injectable({
  providedIn: 'root'
})
export class BillingService {
  private apiUrl = `${environment.apiUrl}Billing`;

  constructor(private http: HttpClient) {}

  createBilling(data: BillingCreateRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/create-billing`, data);
  }

  getBillingById(id: string): Observable<BillingDto> {
    return this.http.get<BillingDto>(`${this.apiUrl}/get-billing-by/${id}`);
  }

  getAllBillings(): Observable<BillingDto[]> {
    return this.http.get<BillingDto[]>(`${this.apiUrl}/get-all-billings`);
  }

  updateBilling(id: string, data: BillingUpdateRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/update-billing/${id}`, data);
  }

  deleteBilling(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/delete-billing/${id}`);
  }
}
