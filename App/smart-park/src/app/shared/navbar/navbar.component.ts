import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar.component.html',
})
export class NavbarComponent {
  isDark = false;
  toggleTheme() {
    this.isDark = !this.isDark;
    document.body.classList.toggle('dark', this.isDark);
  }
}