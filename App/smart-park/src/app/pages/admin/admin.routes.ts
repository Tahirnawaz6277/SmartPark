import { Routes } from '@angular/router';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./admin-layout/admin-layout.component').then(m => m.AdminLayoutComponent),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent)
      },
      {
        path: 'bookings',
        loadComponent: () => import('./bookings/bookings.component').then(m => m.BookingsComponent)
      },
      {
        path: 'billings',
        loadComponent: () => import('./billings/billings.component').then(m => m.BillingsComponent)
      },
      {
        path: 'locations',
        loadComponent: () => import('./locations/locations.component').then(m => m.LocationsComponent)
      },
      {
        path: 'users',
        loadComponent: () => import('./users/users.component').then(m => m.UsersComponent)
      }
    ]
  }
];
