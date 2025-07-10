import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./header/header.component";
import { NavbarComponent } from "./navbar/navbar.component";
import { BottomBarComponent } from "./bottom-bar/bottom-bar.component";
import { SidebarComponent } from "./sidebar/sidebar.component";
import { CommonModule } from '@angular/common';
import { AuthService } from '../services/auth.service';
import { User } from './domain/user';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, HeaderComponent, NavbarComponent, BottomBarComponent, SidebarComponent, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  currentUser: User | null = null;

  constructor(private authService: AuthService) {
      this.authService.currentUser.subscribe(x => this.currentUser = x);
  }

  logout() {
      this.authService.logout();
  }
}
