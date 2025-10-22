import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from '../../../shared/navbar/navbar.component';
import { Sidebar } from '../../../shared/sidebar/sidebar';

@Component({
  selector: 'app-driver-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, NavbarComponent, Sidebar],
  templateUrl: './driver-layout.component.html',
  styleUrls: ['./driver-layout.component.scss']
})
export class DriverLayoutComponent {}
