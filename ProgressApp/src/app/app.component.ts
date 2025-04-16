import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./header/header.component";
import { NavbarComponent } from "./navbar/navbar.component";
import { BottomBarComponent } from "./bottom-bar/bottom-bar.component";
import { ContentTest1Component } from "./content-test1/content-test1.component";
import { ContentTest2Component } from "./content-test2/content-test2.component";
import { ModalTest1Component } from "./modal-test1/modal-test1.component";
import { ProductsComponent } from "./products/products.component";
import { SidebarComponent } from "./sidebar/sidebar.component";


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, HeaderComponent, NavbarComponent, BottomBarComponent, ContentTest1Component, ContentTest2Component, ModalTest1Component, ProductsComponent, SidebarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'ProgressApp';
}
