import { Routes } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './services/auth.service';
import { LoginComponent } from './pages/auth/login/login.component';
import { SignupComponent } from './pages/auth/signup/signup.component';

const authGuard = () => {
  const auth = inject(AuthService);
  return auth.isAuthenticated();
};

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/dashboard/admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent)
  },
  {
    path: 'dashboard/users',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/dashboard/user-management/user-management.component').then(m => m.UserManagementComponent)
  },
  {
    path: 'dashboard/locations',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/dashboard/location-management/location-management.component').then(m => m.LocationManagementComponent)
  },
  { path: '**', redirectTo: 'login' }
];
