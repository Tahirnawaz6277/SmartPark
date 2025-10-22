import { Routes } from '@angular/router';
import { Login } from './pages/auth/login/login';
import { Signup } from './pages/auth/signup/signup';
import { PageNotFound } from './shared/page-not-found/page-not-found';
import { roleGuard } from './core/guards/role-guard';

export const routes: Routes = [
    
    // Public pages without layout
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: Login },
    { path: 'signup', component: Signup },

    // Admin routes - protected with role guard
    {
      path: 'admin',
      loadChildren: () => import('./pages/admin/admin.routes').then(m => m.ADMIN_ROUTES),
      canActivate: [roleGuard],
      data: { role: 'Admin' }
    },

    // Driver routes - protected with role guard
    {
      path: 'driver',
      loadChildren: () => import('./pages/driver/driver.routes').then(m => m.DRIVER_ROUTES),
      canActivate: [roleGuard],
      data: { role: 'Driver' }
    },

    // Legacy dashboard route (kept for backward compatibility)
    {
      path: 'dashboard',
      loadChildren: () => import('./pages/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES)
    },

    // Default redirect
    { path: '**', component: PageNotFound }
];
