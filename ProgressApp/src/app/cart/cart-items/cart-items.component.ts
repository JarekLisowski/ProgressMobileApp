import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { CartPromoItemWithId } from '../../../domain/cartPromoItem';
import { CartItemWithId } from '../../../domain/cartItem';
import { PromoContainerComponent } from "../promo-container/promo-container.component";
import { CartItemComponent } from "../cart-item/cart-item.component";
import { NgFor } from '@angular/common';
import { CartService } from '../../../services/cart.service';

@Component({
  selector: 'cart-items',
  standalone: true,
  imports: [NgFor, PromoContainerComponent, CartItemComponent],
  templateUrl: './cart-items.component.html',
  styleUrl: './cart-items.component.scss'
})
export class CartItemsComponent implements OnInit {

  private readonly cartService = inject(CartService);

  _cartItems: CartItemWithId[] = [];

  set cartItems(cartItems: CartItemWithId[]) {
    this._cartItems = cartItems;
    this.cartItemsPromos = cartItems.filter(item => item.promoItemId !== 0).sort((a, b) => a.promoItemId - b.promoItemId);
    this.cartItemsNoPromos = cartItems.filter(item => item.promoItemId === 0);
  }

  cartItemsPromos: CartItemWithId[] = [];

  cartPromoItems: CartPromoItemWithId[] = [];

  cartItemsNoPromos: CartItemWithId[] = [];

  @Output() itemRemoved = new EventEmitter<number>();

  ngOnInit(): void {
    this.loadCart();
  }

  removeItem($event: number) {
    this.itemRemoved.emit($event);
  }

  loadCart() {
    this.cartService.getCartItems().subscribe(items => {
      console.log('Cart items:', items);
      this.cartItems = items;
    });
    this.cartService.getPromoItems().subscribe(items => {
      console.log('Promo items:', items);
      this.cartPromoItems = items;
    });

  }

}
