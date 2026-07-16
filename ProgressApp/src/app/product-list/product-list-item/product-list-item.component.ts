import { Component, EventEmitter, Input, Output, output, ViewChild } from '@angular/core';
import { IProduct } from '../../../domain/generated/apimodel';
import { DecimalPipe, NgClass } from '@angular/common';

@Component({
  selector: 'product-list-item',
  imports: [DecimalPipe, NgClass],
  templateUrl: './product-list-item.component.html',
  styleUrl: './product-list-item.component.scss'
})
export class ProductListItemComponent {
  
  @Output() addToCartEvent = new EventEmitter<number>();
  
  @Input() data: IProduct | null = null;    

  addToCart() {
    if (this.data) {
      this.addToCartEvent.emit(this.data.id!);
    }
  }
}
