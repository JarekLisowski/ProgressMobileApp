import { Component, Input } from '@angular/core';
import { CartItem } from '../../../domain/cartItem';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-cart-item',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.scss'
})
export class CartItemComponent {

  @Input() cartItem: CartItem | undefined;

  get quantity(): number {
    return this.cartItem?.quantity ?? 0;
  }

  set quantity(value: number) {
    if (this.cartItem) {
      this.cartItem.quantity = value;
    }
  }

  public amountIncrement() {
    console.log('incrementing: ' + this.quantity);
    this.quantity += 1;
  }

  public amountDecrement() {
    if (this.quantity > 1) {
      this.quantity -= 1;
    }
  } 

}
