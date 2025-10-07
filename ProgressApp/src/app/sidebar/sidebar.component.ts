import { Component } from '@angular/core';
import { CategoriesComponent } from "../categories/categories.component";

@Component({
    selector: 'app-sidebar',
    imports: [CategoriesComponent],
    templateUrl: './sidebar.component.html',
    styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {

}
