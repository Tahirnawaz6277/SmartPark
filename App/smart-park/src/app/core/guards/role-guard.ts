import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Auth } from '../services/auth';

export const roleGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
): boolean => {
  const authService = inject(Auth);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  const userRole = authService.getUserRole();
  const requiredRole = route.data['role'];

  console.log('RoleGuard - User role:', userRole, 'Required role:', requiredRole);

  // Case-insensitive role comparison
  if (requiredRole && userRole?.toLowerCase() !== requiredRole.toLowerCase()) {
    console.log('RoleGuard - Access denied, redirecting to login');
    router.navigate(['/login']);
    return false;
  }

  console.log('RoleGuard - Access granted');
  return true;
};
