import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BillingService } from '../../../api/billing/billing.service';
import { BillingDto } from '../../../api/billing/billing.models';
import { Auth } from '../../../core/services/auth';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-my-billings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './my-billings.component.html',
  styleUrls: ['./my-billings.component.scss']
})
export class MyBillingsComponent implements OnInit, OnDestroy {
  billings: BillingDto[] = [];
  loading = true;
  searchTerm = '';
  userId: string | null = '';

  private destroy$ = new Subject<void>();

  constructor(
    private billingService: BillingService,
    private authService: Auth,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.userId = this.authService.getUserId();
    this.loadBillings();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadBillings(): void {
    this.loading = true;
    this.cdr.detectChanges();

    // Check if user is authenticated before making API call
    if (!this.authService.isLoggedIn()) {
      console.warn('User not authenticated, cannot load billings');
      this.loading = false;
      this.cdr.detectChanges();
      return;
    }

    // Get user's billings directly from the API
    this.billingService.getMyBillings()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (billings) => {
          this.billings = billings || [];
          this.loading = false;
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Error loading billings:', err);
          this.billings = [];
          this.loading = false;
          this.cdr.detectChanges();

          if (err.status === 401) {
            console.warn('Unauthorized - please log in again');
          }
        }
      });
  }

  get filteredBillings(): BillingDto[] {
    if (!this.searchTerm) {
      return this.billings;
    }
    return this.billings.filter(billing =>
      billing.paymentStatus?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      billing.paymentMethod?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      billing.locationName?.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  getTotalAmount(): number {
    return this.billings.reduce((sum, bill) => sum + (bill.amount || 0), 0);
  }
}
