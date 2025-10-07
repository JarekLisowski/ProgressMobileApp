import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./header/header.component";
import { NavbarComponent } from "./navbar/navbar.component";
import { BottomBarComponent } from "./bottom-bar/bottom-bar.component";
import { SidebarComponent } from "./sidebar/sidebar.component";

import { AuthService } from '../services/auth.service';
import { User } from './domain/user';
import { AppToastComponent } from './app-toast/app-toast.component';
import { LoggerService } from '../services/loggerService';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet, HeaderComponent, HeaderComponent, NavbarComponent, BottomBarComponent, SidebarComponent, AppToastComponent],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss'
})
export class AppComponent implements AfterViewInit {

  @ViewChild('appToast') toastElement: AppToastComponent | undefined;

  currentUser: User | null = null;

  constructor(private authService: AuthService, private loggerService: LoggerService) {
    this.authService.currentUser.subscribe(x => this.currentUser = x);
  }
  ngAfterViewInit(): void {
    if (this.toastElement != undefined) {
      this.loggerService.toast = this.toastElement;
      //this.loggerService.showWarn("Witaj test jest taki dłuższy test musi się złamać !!!! długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst ");
    }
  }

  logout() {
    this.authService.logout();
  }
}
