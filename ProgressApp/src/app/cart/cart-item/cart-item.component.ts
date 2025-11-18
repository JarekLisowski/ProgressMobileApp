import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CartItem, CartItemWithId } from '../../../domain/cartItem';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { CartService } from '../../../services/cart.service';
import { QuantityComponent } from "../../quantity/quantity.component";
import { ProductStockInfo } from '../../../domain/productStock';

@Component({
    selector: 'cart-item',
    imports: [FormsModule, RouterModule, QuantityComponent, DecimalPipe],
    templateUrl: './cart-item.component.html',
    styleUrl: './cart-item.component.scss'
})
export class CartItemComponent {

  _cartItem: CartItemWithId | undefined;

  respectStock: boolean = false;
  
  warnOverStock: boolean = true;

  constructor(private apiService: ApiService, private cartService: CartService) {
  }

  @Output() itemRemove = new EventEmitter<CartItemWithId>();

  @Input() set cartItem(item: CartItemWithId) {
    this._cartItem = item;
    this._cartItem.imageUrl = this.apiService.makeUrlImage(this._cartItem.productId, 0);
  }

  @Input() readonly: boolean = false;

  @Input() productStockInfo: ProductStockInfo | undefined;
  
  get cartItem(): CartItemWithId | undefined {
    return this._cartItem;
  }

  get quantity(): number {
    return this.cartItem?.quantity ?? 0;
  }

  set quantity(value: number) {
    if (this.cartItem) {
      this.cartItem.quantity = value;
    }
  }

  quantityChanged(quantity: number) {
    if (quantity > 0 && this.cartItem?.id != null && this.cartItem.quantity != quantity) {
      var maxQuantity = this.respectStock ? this._cartItem?.stock ?? undefined : undefined;
      this.cartService.updateCartItemQuntity(this.cartItem.id, quantity, maxQuantity).subscribe(x => {
        this.quantity = x.quantity;
      });
    }
  }

  deleteCartItem(item: CartItemWithId | undefined) {
    if (item != undefined)
      this.itemRemove.emit(item)
  }


}
