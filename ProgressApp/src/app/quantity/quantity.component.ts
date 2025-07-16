import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'quantity',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './quantity.component.html',
  styleUrl: './quantity.component.scss'
})
export class QuantityComponent {

  @Output() quntityChanged = new EventEmitter<number>();

  private _minQuantity: number = 1;
  
  @Input() set minQuantity(value: number) {
    this._minQuantity = value;
    //this.quantity = value;
  }
  
  get minQuantity(): number {
    return this._minQuantity;
  }
  
  private _quantity: number = 1;

  @Input() get quantity(): number {
    return this._quantity;
  }

  set quantity(value: number) {
    if (value < this.minQuantity) {
      value = this.minQuantity;
    }
    if (value != this._quantity) {
      this._quantity = value;
      this.quntityChanged.emit(value);
    }
  }

  quantityDecrement() {
    this.quantity = this.quantity - 1;
  }
  
  quantityIncrement() {
    this.quantity = this.quantity + 1;
  }



}
