import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, NavigationEnd } from '@angular/router';
import { BillingService } from '../../../api/billing/billing.service';
import { BillingDto } from '../../../api/billing/billing.models';
import { BookingService } from '../../../api/booking/booking.service';
import { BookingDto } from '../../../api/booking/booking.models';
import { Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-billings',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './billings.component.html',
  styleUrls: ['./billings.component.scss']
})
export class BillingsComponent implements OnInit, OnDestroy {
  billings: BillingDto[] = [];
  bookings: BookingDto[] = [];
  loading = true;
  searchTerm = '';
  private routerSubscription?: Subscription;
  
  // Modal state
  showModal = false;
  modalLoading = false;
  billingForm!: FormGroup;

  constructor(
    private billingService: BillingService,
    private bookingService: BookingService,
    private cdr: ChangeDetectorRef,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.initForm();
  }

  initForm(): void {
    this.billingForm = this.fb.group({
      bookingId: ['', Validators.required],
      amount: [0, [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    this.loadBillings();
    
    // Reload data when navigating back to this component
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        if (event.url.includes('/admin/billings')) {
          console.log('Navigated to billings, reloading data...');
          this.loadBillings();
        }
      });
  }

  ngOnDestroy(): void {
    this.routerSubscription?.unsubscribe();
  }

  loadBillings(): void {
    this.loading = true;
    this.cdr.detectChanges();
    
    console.log('Loading billings...');
    this.billingService.getAllBillings().subscribe({
      next: (data) => {
        console.log('Billings loaded successfully:', data);
        console.log('Number of billings:', data?.length || 0);
        
        this.billings = Array.isArray(data) ? data : [];
        this.loading = false;
        this.cdr.detectChanges();
        
        console.log('Billings assigned to component:', this.billings.length);
      },
      error: (err) => {
        console.error('Error loading billings:', err);
        alert('Error loading billings: ' + (err.error?.Message || err.message));
        this.billings = [];
        this.loading = false;
        this.cdr.detectChanges();
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

  // Open add billing modal
  openAddModal(): void {
    this.showModal = true;
    this.modalLoading = true;
    this.billingForm.reset();
    
    // Load bookings for dropdown
    this.bookingService.getAllBookings().subscribe({
      next: (bookings) => {
        this.bookings = bookings;
        this.modalLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading bookings:', err);
        alert('Error loading bookings: ' + (err.error?.Message || err.message));
        this.modalLoading = false;
        this.showModal = false;
      }
    });
  }

  // Close modal
  closeModal(): void {
    this.showModal = false;
    this.billingForm.reset();
    this.modalLoading = false;
  }

  // Save billing
  saveBilling(): void {
    if (this.billingForm.invalid) {
      alert('Please fill all required fields correctly');
      return;
    }

    this.modalLoading = true;
    const formData = this.billingForm.getRawValue();

    this.billingService.createBilling(formData).subscribe({
      next: () => {
        alert('Billing created successfully');
        this.closeModal();
        this.loadBillings();
      },
      error: (err) => {
        console.error('Error creating billing:', err);
        alert('Error creating billing: ' + (err.error?.Message || err.message));
        this.modalLoading = false;
      }
    });
  }

  deleteBilling(id: string): void {
    if (confirm('Are you sure you want to delete this billing record?')) {
      this.billingService.deleteBilling(id).subscribe({
        next: () => {
          console.log('Billing deleted successfully');
          alert('Billing deleted successfully');
          this.loadBillings();
        },
        error: (err) => {
          console.error('Error deleting billing:', err);
          alert('Error deleting billing: ' + (err.error?.Message || err.message));
        }
      });
    }
  }
}
