import { Routes } from '@angular/router';

export const DRIVER_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./driver-layout/driver-layout.component').then(m => m.DriverLayoutComponent),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./driver-dashboard/driver-dashboard.component').then(m => m.DriverDashboardComponent)
      },
      {
        path: 'my-bookings',
        loadComponent: () => import('./my-bookings/my-bookings.component').then(m => m.MyBookingsComponent)
      },
      {
        path: 'locations',
        loadComponent: () => import('./driver-locations/driver-locations.component').then(m => m.DriverLocationsComponent)
      }
    ]
  }
];
