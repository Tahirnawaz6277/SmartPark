import { Routes } from '@angular/router';
import { LoginComponent } from './pages/auth/login/login.component';
import { SignupComponent } from './pages/auth/signup/signup.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent)
  },
  {
    path: 'dashboard/users',
    loadComponent: () => import('./pages/dashboard/user-management/user-management.component').then(m => m.UserManagementComponent)
  },
  {
    path: 'dashboard/locations',
    loadComponent: () => import('./pages/dashboard/location-management/location-management.component').then(m => m.LocationManagementComponent)
  },
  { path: '**', redirectTo: 'login' }
];
