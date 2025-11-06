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
  isEditMode = false;
  editingBillingId: string | null = null;

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

  resetForm(): void {
    this.billingForm.reset();
    this.isEditMode = false;
    this.editingBillingId = null;
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
      billing.paymentMethod?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      billing.userName?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      billing.locationName?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      billing.slotNumber?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      billing.slotNumbers?.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  // Open add billing modal
  openAddModal(): void {
    this.showModal = true;
    this.modalLoading = true;
    this.resetForm();
    
    // Load unpaid bookings for dropdown
    this.bookingService.getUnpaidBookings().subscribe({
      next: (bookings) => {
        this.bookings = bookings;
        this.modalLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading unpaid bookings:', err);
        alert('Error loading unpaid bookings: ' + (err.error?.Message || err.message));
        this.modalLoading = false;
        this.showModal = false;
      }
    });
  }

  // Open edit billing modal
  openEditModal(billing: BillingDto): void {
    this.isEditMode = true;
    this.editingBillingId = billing.id;
    this.showModal = true;
    this.modalLoading = true;

    // Load unpaid bookings for dropdown
    this.bookingService.getUnpaidBookings().subscribe({
      next: (bookings) => {
        this.bookings = bookings;
        
        // Patch form with existing billing data
        this.billingForm.patchValue({
          bookingId: billing.bookingId,
          amount: billing.amount
        });
        
        this.modalLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading unpaid bookings:', err);
        alert('Error loading unpaid bookings: ' + (err.error?.Message || err.message));
        this.modalLoading = false;
        this.showModal = false;
      }
    });
  }

  // Close modal
  closeModal(): void {
    this.showModal = false;
    this.resetForm();
    this.modalLoading = false;
  }

  // Save billing (create or update)
  saveBilling(): void {
    if (this.billingForm.invalid) {
      alert('Please fill all required fields correctly');
      return;
    }

    this.modalLoading = true;
    const formData = this.billingForm.getRawValue();

    if (this.isEditMode && this.editingBillingId) {
      // Update existing billing
      this.billingService.updateBilling(this.editingBillingId, formData).subscribe({
        next: () => {
          alert('Billing updated successfully');
          this.closeModal();
          this.loadBillings();
        },
        error: (err) => {
          console.error('Error updating billing:', err);
          alert('Error updating billing: ' + (err.error?.Message || err.message));
          this.modalLoading = false;
        }
      });
    } else {
      // Create new billing
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
