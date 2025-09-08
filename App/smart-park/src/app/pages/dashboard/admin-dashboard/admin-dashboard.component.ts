import { Component, NgModule } from '@angular/core';
import { MasterLayoutComponent } from '../../../shared/master-layout/master-layout.component';
import { DashboardCardsComponent } from './dashboard-cards.component';
import { DashboardWidgetsComponent } from './dashboard-widgets.component';
import { DashboardStatsComponent } from './dashboard-stats.component';


@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [MasterLayoutComponent, DashboardCardsComponent, DashboardWidgetsComponent, DashboardStatsComponent],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent {} 