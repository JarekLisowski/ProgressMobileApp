import { Component, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CartPromoItemWithId } from '../../../domain/cartPromoItem';
import { CartItemWithId } from '../../../domain/cartItem';
import { PromoContainerComponent } from "../promo-container/promo-container.component";
import { CartItemComponent } from "../cart-item/cart-item.component";
import { CartService } from '../../../services/cart.service';
import { ConfirmModalWindowComponent } from "../../confirm-modal-window/confirm-modal-window.component";
import { Transaction } from '../../../domain/transaction';
import { DecimalPipe } from '@angular/common';
import { map, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { ApiService } from '../../../services/api.service';
import { ProductStockInfo } from '../../../domain/productStock';

@Component({
  selector: 'cart-items',
  imports: [PromoContainerComponent, CartItemComponent, ConfirmModalWindowComponent, DecimalPipe],
  templateUrl: './cart-items.component.html',
  styleUrl: './cart-items.component.scss',
})
export class CartItemsComponent implements OnInit, OnDestroy {


  private readonly cartService = inject(CartService);

  @ViewChild('productRemoveWindow') removeProductWindowRef!: ConfirmModalWindowComponent;

  _cartItems: CartItemWithId[] = [];

  private transaction: Transaction | undefined;
  private subscriptionTransaction: Subscription | undefined;
  productStocksMap: Map<number, ProductStockInfo> = new Map<number, ProductStockInfo>();
  itemsValueGross: number = 0;
  itemsValueNet: number = 0;
  subscriptionCartItems: Subscription | undefined;
  subscriptionPromoItems: Subscription | undefined;
  apiService = inject(ApiService);
  outOfStock = false;

  SetCartItems(cartItems: CartItemWithId[]): Observable<boolean> {
    this._cartItems = cartItems;
    return this.getStocksForCartItems().pipe(
      switchMap(stockInfo => this.cartService.checkProductsAvailability(stockInfo)),
      switchMap(productStocks => {
        console.log("Product stocks received:")
        console.dir(productStocks);
        this.setProductStockMap(productStocks);
        this.cartItemsPromos = cartItems.filter(item => item.promoItemId !== 0).sort((a, b) => a.promoItemId - b.promoItemId);
        this.cartItemsNoPromos = cartItems.filter(item => item.promoItemId === 0);
        return of(true);
      })
    );
  }

  private setProductStockMap(productStocks: ProductStockInfo[]) {
    this.outOfStock = false;
    this.productStocksMap = productStocks.reduce((accMap, item) => {
      accMap.set(item.productId, item);
      this.outOfStock = this.outOfStock || item.ouOfStock;
      return accMap;
    }, new Map());
    this.cartService.setOutOfStock(this.outOfStock);
  }

  // SetCartItems(cartItems: CartItemWithId[]): Observable<boolean> {
  //   this._cartItems = cartItems;
  //   return this.getStocksForCartItems().pipe(
  //     switchMap(stockInfo => {
  //       return this.cartService.checkProductsAvailability(stockInfo).pipe(
  //         switchMap(productStocks => {
  //           this.productStocksMap = productStocks.reduce((accMap, item) => {
  //             accMap.set(item.productId, item);
  //             this.outOfStock = this.outOfStock || item.ouOfStock;
  //             return accMap;
  //           }, new Map());
  //           this.cartItemsPromos = cartItems.filter(item => item.promoItemId !== 0).sort((a, b) => a.promoItemId - b.promoItemId);
  //           this.cartItemsNoPromos = cartItems.filter(item => item.promoItemId === 0);
  //           return of(true);
  //         }));
  //     })
  //   );
  // }

  cartItemsPromos: CartItemWithId[] = [];

  cartPromoItems: CartPromoItemWithId[] = [];

  cartItemsNoPromos: CartItemWithId[] = [];

  ngOnInit(): void {
    this.subscriptionTransaction = this.cartService.subscribeTransaction$().subscribe(trans => {
      console.log("Loading transaction data!");
      this.transaction = trans;
      this.itemsValueGross = trans.itemsGross;
      this.itemsValueNet = trans.itemsNet;
    });
    this.subscriptionCartItems = this.cartService.subscribeCartItems$().pipe(
      tap(items => console.log("Cart items updated!", items)),
      switchMap(items => this.SetCartItems(items)),
      //switchMap(() => this.cartService.subscribePromoItems$()),
      //tap(promoItems => console.log("Cart promo items updated!", promoItems))
    ).subscribe();

    this.subscriptionPromoItems = this.cartService.subscribePromoItems$().subscribe(promoItems => {
      console.log("Cart promo items updated!", promoItems)
      this.cartPromoItems = promoItems;
    });
  }

  getStocksForCartItems(): Observable<ProductStockInfo[]> {
    var itemIds = this._cartItems.map(x => x.productId);
    var result = this.apiService.getStocksForProducts(itemIds).pipe(
      map(x => {
        console.log("Stocks for products received:")
        console.dir(x);
        var productStocksResult: ProductStockInfo[] = [];
        if (x != null && x.data != undefined) {
          this._cartItems.forEach(item => {
            item.stock = x.data!.find(stock => stock.stTowId == item.productId)?.stStan ?? undefined;
          });
          productStocksResult = x.data!
            .filter(stock => stock.stTowId !== undefined)
            .map(stock => new ProductStockInfo(stock.stTowId!, 0, stock.stStan));
        }
        return productStocksResult;
      })
    );
    return result;
  }

  ngOnDestroy(): void {
    if (this.subscriptionTransaction) {
      this.subscriptionTransaction.unsubscribe();
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

  getItemStockInfo(productId: number): ProductStockInfo | undefined {
    return this.productStocksMap.get(productId);
  }

}
