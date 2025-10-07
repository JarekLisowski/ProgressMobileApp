import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CartItem, CartItemWithId } from '../../../domain/cartItem';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { CartService } from '../../../services/cart.service';
import { QuantityComponent } from "../../quantity/quantity.component";

@Component({
    selector: 'cart-item',
    imports: [FormsModule, RouterModule, QuantityComponent, CommonModule],
    templateUrl: './cart-item.component.html',
    styleUrl: './cart-item.component.scss'
})
export class CartItemComponent {

  _cartItem: CartItemWithId | undefined;

  constructor(private apiService: ApiService, private cartService: CartService) {
  }

  @Output() itemRemove = new EventEmitter<CartItemWithId>();


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

  // amountIncrement() {
  //   this.cartService.updateCartItemQuntity(this.cartItem?.id ?? 0, this.quantity + 1).subscribe(x => {
  //     this.quantity = x.quantity;
  //   });
  // }

  // amountDecrement() {
  //   if (this.quantity > 1) {
  //     this.cartService.updateCartItemQuntity(this.cartItem?.id ?? 0, this.quantity - 1).subscribe(x => {
  //       this.quantity = x.quantity;
  //     });
  //   }
  // }

  quantityChanged(quantity: number) {
    if (quantity > 1 && this.cartItem?.id != null) {
      this.cartService.updateCartItemQuntity(this.cartItem.id, quantity).subscribe(x => {
        var cartItem = x;
        x.imageUrl = this._cartItem!.imageUrl;
        this._cartItem = x;
        this.quantity = x.quantity;
      });
    }
  }

  deleteCartItem(item: CartItemWithId | undefined) {
    if (item != undefined)
      this.itemRemove.emit(item)
  }


}
