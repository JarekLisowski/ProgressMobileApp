import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CartItem, CartItemWithId } from '../../../domain/cartItem';
import { CartService } from '../../../services/cart.service';
import { ApiService } from '../../../services/api.service';
import { CartPromoItem, CartPromoItemWithId } from '../../../domain/cartPromoItem';
import { CartItemComponent } from "../cart-item/cart-item.component";
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductStockInfo } from '../../../domain/productStock';

@Component({
    selector: 'promo-cart-container',
    imports: [CartItemComponent, RouterLink, DecimalPipe],
    templateUrl: './promo-container.component.html',
    styleUrl: './promo-container.component.scss'
})
export class PromoContainerComponent implements OnInit {

  cartItems: CartItemWithId[] = [];

  @Input() set promoItem(cartPromoItem: CartPromoItemWithId) {
    this.cartPromoItem = cartPromoItem;
    this.cartService.getCartItemsForPromoSet(cartPromoItem.id).subscribe(items => {
      this.cartItems = items;
    });
  }

  @Input() productStockInfoMap: Map<number, ProductStockInfo> | undefined;

  @Output() itemRemove = new EventEmitter<CartPromoItemWithId>();

  cartPromoItem: CartPromoItemWithId | undefined;


  constructor(private cartService: CartService, private apiService: ApiService) {

  }

  deletePromo(item: CartPromoItemWithId | undefined) {
    if (item != undefined)
      this.itemRemove.emit(item )
  }

  ngOnInit(): void {

  }

  getItemStockInfo(productId: number): ProductStockInfo | undefined {
    var result= this.productStockInfoMap?.get(productId);
    //console.log(result);
    return result;
  }



}
