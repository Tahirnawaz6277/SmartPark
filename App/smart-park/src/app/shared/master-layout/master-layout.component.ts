import { Component } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-master-layout',
  standalone: true,
  imports: [NavbarComponent, SidebarComponent, FooterComponent],
  templateUrl: './master-layout.component.html',
  styleUrls: ['./master-layout.component.scss']
})
export class MasterLayoutComponent {}