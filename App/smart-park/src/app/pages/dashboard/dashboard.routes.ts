import { Routes } from '@angular/router';
import { AdminDashboard } from './admin-dashboard/admin-dashboard';
import { Header } from '../../shared/header/header';
import { Sidebar } from '../../shared/sidebar/sidebar';
import { Footer } from '../../shared/footer/footer';


export const DASHBOARD_ROUTES: Routes = [
  { path: '', component: AdminDashboard },
  { path: 'header', component: Header },
  { path: 'sidebar', component: Sidebar },
  { path: 'footer', component: Footer },
];