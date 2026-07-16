import { Component, Input, ViewChild } from '@angular/core';
import { IProduct } from '../../domain/generated/apimodel';
import { AddProductWindowComponent } from '../add-product-window/add-product-window.component';
import { ProductListItemComponent } from './product-list-item/product-list-item.component';

@Component({
  selector: 'product-list',
  imports: [AddProductWindowComponent, ProductListItemComponent],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss'
})
export class ProductListComponent {

  @ViewChild('addWindow') addWindow!: AddProductWindowComponent;

  @Input() items: IProduct[] | undefined;

  addToCart(productId: number) {
    this.addWindow.show(productId);
  }
}
