import { Component, ElementRef, EventEmitter, inject, Input, OnInit, Output, ViewChild } from '@angular/core';
import { CartPromoItemWithId } from '../../../domain/cartPromoItem';
import { CartItemWithId } from '../../../domain/cartItem';
import { PromoContainerComponent } from "../promo-container/promo-container.component";
import { CartItemComponent } from "../cart-item/cart-item.component";
import { NgFor } from '@angular/common';
import { CartService } from '../../../services/cart.service';
import { ProductRemoveWindowComponent } from "../../product-remove-window/product-remove-window.component";
import { ConfirmModalWindowComponent } from "../../confirm-modal-window/confirm-modal-window.component";

@Component({
    selector: 'cart-items',
    imports: [NgFor, PromoContainerComponent, CartItemComponent, ConfirmModalWindowComponent],
    templateUrl: './cart-items.component.html',
    styleUrl: './cart-items.component.scss'
})
export class CartItemsComponent implements OnInit {

  private readonly cartService = inject(CartService);

  @ViewChild('productRemoveWindow') removeProductWindowRef!: ConfirmModalWindowComponent;

  _cartItems: CartItemWithId[] = [];

  set cartItems(cartItems: CartItemWithId[]) {
    this._cartItems = cartItems;
    this.cartItemsPromos = cartItems.filter(item => item.promoItemId !== 0).sort((a, b) => a.promoItemId - b.promoItemId);
    this.cartItemsNoPromos = cartItems.filter(item => item.promoItemId === 0);
  }

  cartItemsPromos: CartItemWithId[] = [];

  cartPromoItems: CartPromoItemWithId[] = [];

  cartItemsNoPromos: CartItemWithId[] = [];

  //@Output() itemRemoved = new EventEmitter<number>();

  ngOnInit(): void {
    this.loadCart();
  }

  removeItem(item: CartItemWithId) {
    this.removeProductWindowRef.title = "Usuwanie produktu";
    var message = item.name??"";
    this.removeProductWindowRef.buttonAcceptText = "Usuń";
    this.removeProductWindowRef.showObservable(message).subscribe(x => {
      if (x) {
          this.cartService.removeItemFromCart(item.id).subscribe(x => {
            console.log('Cart item removed:', item.name);
            //this.itemRemoved.emit(id);
            this.loadCart();
          });
      }
    });
  }

  removePromoItem(item: CartPromoItemWithId) {
    this.removeProductWindowRef.title = "Usuwanie zestawu promocyjnego";
    var message = item.name;
    this.removeProductWindowRef.buttonAcceptText = "Usuń";
    this.removeProductWindowRef.showObservable(message).subscribe(x => {
      if (x) {
        this.cartService.removePromoSetFromCart(item.id).subscribe(x => {
          console.log('Promo set removed from cart:', x);
          this.loadCart();
        });
      }
    });
  }

  loadCart() {
    this.cartService.getCartItems().subscribe(items => {
      this.cartItems = items;
    });
    this.cartService.getPromoItems().subscribe(items => {
      this.cartPromoItems = items;
    });

  }

}
