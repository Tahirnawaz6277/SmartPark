import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Auth } from '../../core/services/auth';

interface MenuItem {
  label: string;
  icon: string;
  route: string;
  roles: string[];
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class Sidebar implements OnInit {
  userRole: string | null = '';
  menuItems: MenuItem[] = [];

  private allMenuItems: MenuItem[] = [
    { label: 'Dashboard', icon: 'bi-speedometer2', route: '/admin/dashboard', roles: ['Admin'] },
    { label: 'Bookings', icon: 'bi-calendar-check', route: '/admin/bookings', roles: ['Admin'] },
    { label: 'Billings', icon: 'bi-receipt', route: '/admin/billings', roles: ['Admin'] },
    { label: 'Locations', icon: 'bi-geo-alt', route: '/admin/locations', roles: ['Admin'] },
    { label: 'Users', icon: 'bi-people', route: '/admin/users', roles: ['Admin'] },
    { label: 'Dashboard', icon: 'bi-speedometer2', route: '/driver/dashboard', roles: ['Driver'] },
    { label: 'My Bookings', icon: 'bi-calendar-check', route: '/driver/my-bookings', roles: ['Driver'] },
    { label: 'Locations', icon: 'bi-geo-alt', route: '/driver/locations', roles: ['Driver'] },
    { label: 'My Billings', icon: 'bi-receipt', route: '/driver/my-billings', roles: ['Driver'] }
  ];

  constructor(private authService: Auth) {}

  ngOnInit(): void {
    this.userRole = this.authService.getUserRole();
    this.filterMenuItems();
  }

  private filterMenuItems(): void {
    if (this.userRole) {
      this.menuItems = this.allMenuItems.filter(item => 
        item.roles.includes(this.userRole!)
      );
    }
  }
}
