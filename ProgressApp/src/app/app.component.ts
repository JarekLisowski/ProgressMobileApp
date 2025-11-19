import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./header/header.component";
import { NavbarComponent } from "./navbar/navbar.component";
import { BottomBarComponent } from "./bottom-bar/bottom-bar.component";
import { SidebarComponent } from "./sidebar/sidebar.component";

import { User } from './domain/user';
import { AppToastComponent } from './app-toast/app-toast.component';
import { LoggerService } from '../services/loggerService';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, HeaderComponent, NavbarComponent, BottomBarComponent, SidebarComponent, AppToastComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements AfterViewInit {

  @ViewChild('appToast') toastElement: AppToastComponent | undefined;

  currentUser: User | null = null;

  constructor(private userService: UserService, private loggerService: LoggerService) {
    this.userService.getUser().subscribe(x => {
      if (x != null && x.id != null && x.id > 0) {
        this.currentUser =
        {
          id: x.id,
          username: x.name ?? "",
        }
      } else {
        //this.currentUser = null;
      }
    });
  }

  ngAfterViewInit(): void {
    if (this.toastElement != undefined) {
      this.loggerService.toast = this.toastElement;
      //this.loggerService.showWarn("Witaj test jest taki dłuższy test musi się złamać !!!! długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst długi tekst ");
    }
  }

  logout() {
    this.userService.logout().subscribe(() => {
      this.loggerService.showInfo("Wylogowano użytkownika.");
      location.reload();
    });
  }

}
