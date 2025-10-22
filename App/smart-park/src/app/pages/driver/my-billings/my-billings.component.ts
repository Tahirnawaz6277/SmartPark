import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BillingService } from '../../../api/billing/billing.service';
import { BookingService } from '../../../api/booking/booking.service';
import { BillingDto } from '../../../api/billing/billing.models';
import { Auth } from '../../../core/services/auth';

@Component({
  selector: 'app-my-billings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './my-billings.component.html',
  styleUrls: ['./my-billings.component.scss']
})
export class MyBillingsComponent implements OnInit {
  billings: BillingDto[] = [];
  loading = true;
  searchTerm = '';
  userId: string | null = '';

  constructor(
    private billingService: BillingService,
    private bookingService: BookingService,
    private authService: Auth
  ) {}

  ngOnInit(): void {
    this.userId = this.authService.getUserId();
    this.loadBillings();
  }

  loadBillings(): void {
    this.loading = true;
    
    // First get user's bookings
    this.bookingService.getAllBookings().subscribe({
      next: (bookings) => {
        const userBookingIds = bookings
          .filter(b => b.userId === this.userId)
          .map(b => b.id);

        // Then get all billings and filter by user's bookings
        this.billingService.getAllBillings().subscribe({
          next: (billings) => {
            this.billings = billings.filter(b => 
              b.bookingId && userBookingIds.includes(b.bookingId)
            );
            this.loading = false;
          },
          error: (err) => {
            console.error('Error loading billings:', err);
            this.loading = false;
          }
        });
      },
      error: (err) => {
        console.error('Error loading bookings:', err);
        this.loading = false;
      }
    });
  }

  get filteredBillings(): BillingDto[] {
    if (!this.searchTerm) {
      return this.billings;
    }
    return this.billings.filter(billing =>
      billing.paymentStatus?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      billing.paymentMethod?.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  getTotalAmount(): number {
    return this.billings.reduce((sum, bill) => sum + (bill.amount || 0), 0);
  }
}
