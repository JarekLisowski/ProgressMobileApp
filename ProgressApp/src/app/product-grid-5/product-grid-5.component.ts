import { Component, Input } from '@angular/core';
import { Product5Component } from "./product-5/product-5.component";

import { IProduct, Product } from '../../domain/generated/apimodel';

@Component({
    selector: 'app-product-grid-5',
    imports: [Product5Component],
    templateUrl: './product-grid-5.component.html',
    styleUrl: './product-grid-5.component.scss'
})
export class ProductGrid5Component {

  @Input() items : IProduct[] | undefined;
}
