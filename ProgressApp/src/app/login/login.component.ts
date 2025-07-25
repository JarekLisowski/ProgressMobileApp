import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, NgIf],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  username = '';
  password = '';
  error = '';

  constructor(private api: ApiService, private userService: UserService) { }

  onLogin() {
    this.api.login(this.username, this.password)
      .subscribe({
        next: (data) => {
          if (!data.isError && data.data?.user != null) {
            console.log(data);
            this.userService.setUser(data.data.user).subscribe(x => {
              console.log("Login ok");
            });
          }
        },
        error: (error) => {
          this.error = error;
        },
      });
  }
}
