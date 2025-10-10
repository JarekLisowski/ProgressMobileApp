import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Product } from '../../../domain/generated/apimodel';
import { FormsModule } from '@angular/forms';
import { ProductPromoItem } from '../../../domain/ProductPromoItem';
import { QuantityComponent } from "../../quantity/quantity.component";
import { DecimalPipe } from '@angular/common';

@Component({
    selector: 'app-special-offer-product-item',
    imports: [FormsModule, QuantityComponent, DecimalPipe],
    templateUrl: './special-offer-product-item.component.html',
    styleUrl: './special-offer-product-item.component.scss'
})
export class SpecialOfferProductItemComponent {

  @Input() data!: ProductPromoItem;
  @Output() quantityChangedEvent = new EventEmitter<void>();

  get quantity() {
    return this.data.quantity;
  }

  set quantity(value: number) {
    this.data.quantity = value;
  }

  quantityChanged(quantity: number) {
    this.quantity = quantity;
    this.quantityChangedEvent.emit();
  }
}
