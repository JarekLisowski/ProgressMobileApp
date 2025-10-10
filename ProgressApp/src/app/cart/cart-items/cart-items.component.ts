import { ChangeDetectionStrategy, Component, ElementRef, EventEmitter, inject, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { CartPromoItemWithId } from '../../../domain/cartPromoItem';
import { CartItemWithId } from '../../../domain/cartItem';
import { PromoContainerComponent } from "../promo-container/promo-container.component";
import { CartItemComponent } from "../cart-item/cart-item.component";

import { CartService } from '../../../services/cart.service';
import { ProductRemoveWindowComponent } from "../../product-remove-window/product-remove-window.component";
import { ConfirmModalWindowComponent } from "../../confirm-modal-window/confirm-modal-window.component";
import { Transaction } from '../../../domain/transaction';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { Subscription } from 'rxjs';

@Component({
  selector: 'cart-items',
  imports: [PromoContainerComponent, CartItemComponent, ConfirmModalWindowComponent, DecimalPipe, CurrencyPipe],
  templateUrl: './cart-items.component.html',
  styleUrl: './cart-items.component.scss',
  //changeDetection: ChangeDetectionStrategy.OnPush, 
})
export class CartItemsComponent implements OnInit, OnDestroy {


  private readonly cartService = inject(CartService);

  @ViewChild('productRemoveWindow') removeProductWindowRef!: ConfirmModalWindowComponent;

  _cartItems: CartItemWithId[] = [];

  private transaction: Transaction | undefined;
  private subscription: Subscription | undefined;
  itemsValueGross: number = 0;
  itemsValueNet: number = 0;
  subscriptionCartItems: Subscription | undefined;
  subscriptionPromoItems: Subscription | undefined;

  set cartItems(cartItems: CartItemWithId[]) {
    this._cartItems = cartItems;
    this.cartItemsPromos = cartItems.filter(item => item.promoItemId !== 0).sort((a, b) => a.promoItemId - b.promoItemId);
    this.cartItemsNoPromos = cartItems.filter(item => item.promoItemId === 0);
  }

  cartItemsPromos: CartItemWithId[] = [];

  cartPromoItems: CartPromoItemWithId[] = [];

  cartItemsNoPromos: CartItemWithId[] = [];

  ngOnInit(): void {
    this.subscription = this.cartService.subscribeTransaction$().subscribe(trans => {
       console.log("Loading transaction data!");
       this.transaction = trans;
       this.itemsValueGross = trans.itemsGross;
       this.itemsValueNet = trans.itemsNet;
    });
    this.subscriptionCartItems = this.cartService.subscribeCartItems$().subscribe(items => {
      console.log("Cart items updated!");
      this.cartItems = items;
    });
    this.subscriptionPromoItems = this.cartService.subscribePromoItems$().subscribe(items => {
      console.log("Cart promo items updated!");
      this.cartPromoItems = items;
    });
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
    if (this.subscriptionCartItems) {
      this.subscriptionCartItems.unsubscribe();
    }
    if (this.subscriptionPromoItems) {
      this.subscriptionPromoItems.unsubscribe();
    }
  }

  removeItem(item: CartItemWithId) {
    this.removeProductWindowRef.title = "Usuwanie produktu";
    var message = item.name ?? "";
    this.removeProductWindowRef.buttonAcceptText = "Usuń";
    this.removeProductWindowRef.showObservable(message).subscribe(x => {
      if (x) {
        this.cartService.removeItemFromCart(item.id).subscribe(x => {
          console.log('Cart item removed:', item.name);
          //this.itemRemoved.emit(id);
          //this.loadCart();
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
          //this.loadCart();
        });
      }
    });
  }

}
