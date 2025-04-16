import { Component } from '@angular/core';
import { CategoriesComponent } from "../categories/categories.component";

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CategoriesComponent],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {

}
