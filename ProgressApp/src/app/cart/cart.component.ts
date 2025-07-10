import { Component, inject, OnInit } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { CartItem, CartItemWithId } from '../../domain/cartItem';
import { CartItemComponent } from "./cart-item/cart-item.component";
import { NgFor, NgIf } from '@angular/common';
import { PromoContainerComponent } from "./promo-container/promo-container.component";
import { CartPromoItem, CartPromoItemWithId } from '../../domain/cartPromoItem';
import { CustomerListComponent } from "../customer-list/customer-list.component";
import { CustomerSelectComponent } from "../customer-select/customer-select.component";
import { Customer } from '../../domain/generated/apimodel';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CartItemComponent, NgFor, NgIf, PromoContainerComponent, CustomerListComponent, CustomerSelectComponent],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})

export class CartComponent implements OnInit {


  private readonly cartService = inject(CartService);

  _cartItems: CartItemWithId[] = [];

  customer: Customer | undefined;

  set cartItems(cartItems: CartItemWithId[]) {
    this._cartItems = cartItems;
    this.cartItemsPromos = cartItems.filter(item => item.promoItemId !== 0).sort((a, b) => a.promoItemId - b.promoItemId);
    this.cartItemsNoPromos = cartItems.filter(item => item.promoItemId === 0);
  }

  cartPromoItems: CartPromoItemWithId[] = [];

  cartItemsPromos: CartItemWithId[] = [];

  cartItemsNoPromos: CartItemWithId[] = [];

  currentPromoId: number = 0;

  ngOnInit(): void {
    this.loadCart();
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
    this.cartService.getCurrentTransaction().subscribe(transaction => {
      console.log('Transaction:');
      console.log(transaction);
      this.customer = transaction.customer;
    });
  }

  customerSelected($event: Customer) {
    this.customer = $event;
    this.cartService.getCurrentTransaction().subscribe(transaction => {
      console.log('Transaction:');
      console.log(transaction);
      transaction.customer = this.customer;
      this.cartService.updateTransaction(transaction).subscribe(x => {
        console.log('Transaction updated:', x);
      });

    });
  }

  removeItem($event: number) {
    this.loadCart();
  }

}
