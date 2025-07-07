import { Component, Input } from '@angular/core';
import { CartItem } from '../../../domain/cartItem';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'cart-item',
  standalone: true,
  imports: [FormsModule, NgIf, RouterModule],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.scss'
})
export class CartItemComponent {

  @Input() cartItem: CartItem | undefined;

  @Input() readonly: boolean = false;

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
