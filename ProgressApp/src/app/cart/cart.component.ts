import { Component, inject, OnInit } from '@angular/core';
import { CartService } from '../../services/cart-service';
import { CartItem } from '../../domain/cartItem';
import { CartItemComponent } from "./cart-item/cart-item.component";
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CartItemComponent, NgFor],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})

export class CartComponent implements OnInit {
    
  private readonly cartService = inject(CartService);

  cartItems: CartItem[] = [];


  ngOnInit(): void {
    this.cartService.getCartItems().subscribe(items => {
      console.log('Cart items:', items);
      this.cartItems = items;
    });
  }

}
