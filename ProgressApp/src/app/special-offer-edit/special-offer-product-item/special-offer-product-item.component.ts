import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Product } from '../../../domain/generated/apimodel';
import { FormsModule } from '@angular/forms';
import { ProductPromoItem } from '../../../domain/ProductPromoItem';

@Component({
  selector: 'app-special-offer-product-item',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './special-offer-product-item.component.html',
  styleUrl: './special-offer-product-item.component.scss'
})
export class SpecialOfferProductItemComponent {

  @Input() data! : ProductPromoItem;
  @Output() quantityChangedEvent = new EventEmitter<void>();
  
  get quantity() {
    return this.data.quantity;
  }
  set quantity(value: number) {
    this.data.quantity = value;
  }

  public amountIncrement() {
    this.data.quantity += 1;
    this.quantityChangedEvent.emit(); 
    console.log("Quantity changed: " + this.data.quantity);
  }

  public amountDecrement() {
    if (this.data.quantity >= 1) {
      this.data.quantity -= 1;
      this.quantityChangedEvent.emit(); 
      console.log("Quantity changed: " + this.data.quantity);
    }
  } 
}
