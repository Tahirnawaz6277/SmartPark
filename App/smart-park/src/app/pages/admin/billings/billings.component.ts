import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, NavigationEnd } from '@angular/router';
import { BillingService } from '../../../api/billing/billing.service';
import { BillingDto } from '../../../api/billing/billing.models';
import { Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-billings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './billings.component.html',
  styleUrls: ['./billings.component.scss']
})
export class BillingsComponent implements OnInit, OnDestroy {
  billings: BillingDto[] = [];
  loading = true;
  searchTerm = '';
  private routerSubscription?: Subscription;

  constructor(
    private billingService: BillingService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

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
