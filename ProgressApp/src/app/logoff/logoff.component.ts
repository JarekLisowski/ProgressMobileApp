import { Component, inject } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-logoff',
  imports: [],
  templateUrl: './logoff.component.html',
  styleUrl: './logoff.component.scss'
})
export class LogoffComponent {
  ApiService = inject(ApiService);
  UserService = inject(UserService);

  constructor() {
    this.logoff();
  }

  logoff() {
    //this.UserService. logoff();
  }
}
