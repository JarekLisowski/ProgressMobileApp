import { Component } from '@angular/core';
import { ProductGrid5Component } from '../product-grid-5/product-grid-5.component';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [ProductGrid5Component],
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent {

}
