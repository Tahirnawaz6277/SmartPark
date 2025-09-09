import { Component, OnInit } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { MasterLayoutComponent } from '../../../shared/master-layout/master-layout.component';
import { DashboardCardsComponent } from './dashboard-cards.component';
import { DashboardWidgetsComponent } from './dashboard-widgets.component';
import { DashboardStatsComponent } from './dashboard-stats.component';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [RouterModule, MasterLayoutComponent, DashboardCardsComponent, DashboardWidgetsComponent, DashboardStatsComponent],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {
  currentUser: any = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Check if user is authenticated
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    // Get current user info
    this.currentUser = this.authService.getCurrentUser();
  }
} 