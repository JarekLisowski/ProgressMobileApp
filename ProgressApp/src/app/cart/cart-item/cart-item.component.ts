import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CartItem, CartItemWithId } from '../../../domain/cartItem';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { CartService } from '../../../services/cart.service';

@Component({
  selector: 'cart-item',
  standalone: true,
  imports: [FormsModule, NgIf, RouterModule],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.scss'
})
export class CartItemComponent {

  _cartItem: CartItemWithId | undefined;

  constructor(private apiService: ApiService, private cartService: CartService) {
  }

  @Output() itemRemoved = new EventEmitter<number>();


  @Input() set cartItem(item: CartItemWithId) {
    this._cartItem = item;
    this._cartItem.imageUrl = this.apiService.makeUrlImage(this._cartItem.productId, 0);
  }

  get cartItem(): CartItemWithId | undefined {
    return this._cartItem;
  }

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
    this.cartService.updateCartItemQuntity(this.cartItem?.id ?? 0, this.quantity + 1).subscribe(x => {
      this.quantity = x.quantity;
    });
  }

  public amountDecrement() {
    if (this.quantity > 1) {
      this.cartService.updateCartItemQuntity(this.cartItem?.id ?? 0, this.quantity - 1).subscribe(x => {
        this.quantity = x.quantity;
      });
    }
  }

  deleteCartItem(id: number | undefined) {
    if (id)
      this.cartService.removeItemFromCart(id).subscribe(x => {
        console.log('Cart item removed:', x);
        this.itemRemoved.emit(id);
      })
  }


}
