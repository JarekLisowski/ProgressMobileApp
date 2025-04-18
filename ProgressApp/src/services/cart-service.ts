import { NgxIndexedDBService } from "ngx-indexed-db";
import { Observable } from "rxjs";
import { Product } from "../domain/generated/apimodel";
import { CartItem } from "../domain/cartItem";
import { inject, Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
  })
export class CartService {
    constructor(private dbService: NgxIndexedDBService) {}

    addItemToCart(product: Product, quantity: number): Observable<any> {
        var cartItem : CartItem = 
        {
            productId: product.id ?? 0,
            name: product.name ?? "",
            code: product.code ?? "",
            priceNet: product.price?.priceNet ?? 0,
            priceGross: product.price?.priceGross ?? 0,
            quantity: quantity
        };
        return this.dbService.add('cart', cartItem);
    }

    updateCartItem(id: number, quantity: number): Observable<any> {
        return this.dbService.update('cart', { id: id, quantity: quantity });
    }

    getCartItems(): Observable<CartItem[]> {
        return this.dbService.getAll<CartItem>('cart');
    }

    removeItemFromCart(id: number): Observable<any> {
        return this.dbService.delete('cart', id);
    }

    clearCart(): Observable<any> {
        return this.dbService.clear('cart');
    }
}