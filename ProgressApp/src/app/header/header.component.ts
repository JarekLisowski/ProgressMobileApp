import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SearchBarComponent } from "../search-bar/search-bar.component";

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterModule, SearchBarComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {

}
