import { Component, Input, OnInit } from '@angular/core';
import { CartItem } from '../../../domain/cartItem';
import { CartService } from '../../../services/cart-service';
import { ApiService } from '../../../services/api.service';
import { CartPromoItem, CartPromoItemWithId } from '../../../domain/cartPromoItem';
import { CartItemComponent } from "../cart-item/cart-item.component";
import { NgFor } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'promo-cart-container',
  standalone: true,
  imports: [CartItemComponent, NgFor, RouterLink],
  templateUrl: './promo-container.component.html',
  styleUrl: './promo-container.component.scss'
})
export class PromoContainerComponent implements OnInit {
  
  cartItems: CartItem[] = [];

  @Input() set promoItem(cartPromoItem: CartPromoItemWithId) {
    this.cartPromoItem = cartPromoItem;
    this.cartService.getCartItemsForPromoSet(cartPromoItem.id).subscribe(items => {
      console.log('Cart promo items:', items);
      this.cartItems = items;
    });
  }

  cartPromoItem: CartPromoItemWithId | undefined;
  

  constructor(private cartService: CartService, private apiService: ApiService) {

  }

  ngOnInit(): void {
    
  }



}
