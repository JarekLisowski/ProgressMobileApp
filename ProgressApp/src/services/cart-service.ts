import { NgxIndexedDBService, WithID } from "ngx-indexed-db";
import { from, map, Observable, of, switchMap, tap } from "rxjs";
import { Customer, Product } from "../domain/generated/apimodel";
import { CartItem } from "../domain/cartItem";
import { inject, Injectable } from "@angular/core";
import { SpecialOfferEdit } from "../domain/specialOfferEdit";
import { CartPromoItem, CartPromoItemWithId } from "../domain/cartPromoItem";
import { Transaction } from "../domain/transaction";

@Injectable({
    providedIn: 'root'
})
export class CartService {

    constructor(private dbService: NgxIndexedDBService) { }

    addItemToCart(product: Product, quantity: number): Observable<any> {
        var cartItem: CartItem =
        {
            productId: product.id ?? 0,
            name: product.name ?? "",
            code: product.code ?? "",
            priceNet: product.price?.priceNet ?? 0,
            priceGross: product.price?.priceGross ?? 0,
            quantity: quantity,
            promoSetId: 0,
            promoItemId: 0
        };
        return this.dbService.add('cart', cartItem);
    }

    addItemsToCart(cartItems: CartItem[]): Observable<any> {
        return this.dbService.bulkAdd('cart', cartItems);
    }


    updateCartItem(id: number, quantity: number): Observable<any> {
        return this.dbService.update('cart', { id: id, quantity: quantity });
    }

    getCartItems(): Observable<CartItem[]> {
        return this.dbService.getAll<CartItem>('cart');
    }

    getCartItemsForPromoSet(promoSetId: number): Observable<CartItem[]> {
        return this.dbService.getAllByIndex('cart', 'promoSetId', promoSetId);
    }


    removeItemFromCart(id: number): Observable<any> {
        return this.dbService.delete('cart', id);
    }

    clearCart(): Observable<any> {
        return this.dbService.clear('cart');
    }



    addPromoSetToCart(promoSet: SpecialOfferEdit): Observable<any> {
        if (promoSet.id != undefined && promoSet.id > 0) {
            this.dbService.delete('promoSet', promoSet.id).subscribe(x => {
                console.log("Promoset deleted from cart");
                this.dbService.deleteAllByIndex('cart', 'promoSetId', promoSet.id).subscribe(x => {
                    console.log("Promoitems deleted from cart");
                    promoSet.id = undefined;
                    this.addPromoSetToCart(promoSet).subscribe(x => {
                        console.log("Promoset updated in cart:", x);
                    });
                })
            });

        }
        if (promoSet.id == undefined) {
            var cartPromoItem: CartPromoItem = CartPromoItem.fromSpecialOfferEdit(promoSet);
            return this.dbService.add('promoSet', cartPromoItem).pipe(
                tap(promoSetId => {
                    console.log("Promoset added to cart:", promoSetId);
                    var cartItems = promoSet.getAllCartItems(promoSetId.id);
                    this.addItemsToCart(cartItems).subscribe(x =>
                        console.log("Promo items added to cart:", x)
                    );
                })
            );
        }
        return new Observable<any>();
    }

    getPromoItems(): Observable<CartPromoItemWithId[]> {
        return this.dbService.getAll<CartPromoItemWithId>('promoSet');
    }

    getPromoItem(id: number): Observable<CartPromoItem> {
        return this.dbService.getByID<CartPromoItem>('promoSet', id);
    }

    // getTransaction() : Observable<Transaction & WithID> {
    //     var obs = this.dbService.getAll<Transaction & WithID>('transaction').pipe(
    //         tap(x => {
    //             return 1;
    //         }),
    //         map(x => {
    //             if (x.length == 0) {
    //                 var res2 = this.dbService.add('transaction', new Transaction());
    //                 return res2;
    //             }
    //             //else {
    //                 var res: Transaction & WithID = x[0];
    //               //  return res;
    //             //}
    //         })
    //     );
    //     return obs;
    // }

    storeName = 'transaction';

    getCurrentTransaction(): Observable<Transaction> {
        return this.getFirstOrCreateTransaction();
    }

    private getFirstOrCreateTransaction(): Observable<Transaction> {
        // First, try to get all transactions to check if the store is empty.
        return from(this.dbService.getAll<Transaction>(this.storeName)).pipe(
            switchMap((transactions: Transaction[]) => {
                if (transactions && transactions.length > 0) {
                    // If transactions exist, return the first one.
                    console.log('Transaction store is not empty. Returning first transaction:', transactions[0]);
                    return of(transactions[0]);
                } else {
                    // If the store is empty, create a new default transaction.
                    const newTransaction: Transaction = new Transaction();

                    // Add the new transaction to the database.
                    // The 'add' method returns the key of the newly added object.
                    // We then retrieve the full object using the key (or assume the input object is sufficient).
                    console.log('Transaction store is empty. Adding a new transaction:', newTransaction);
                    return from(this.dbService.add<Transaction>(this.storeName, newTransaction));
                    //   .pipe(
                    //     map(t => { 
                    //       return t;
                    //     })
                    //   );
                }
            })
        );
    }

    updateTransaction(transaction: Transaction) {
        return this.dbService.update('transaction', transaction);
    }

    clearTransaction(transaction: Transaction) {
        this.dbService.clear('transaction');
    }



    setCustomer(customer: Customer) {
        this.dbService.getAll('transaction').subscribe(x => {

        });
    }

}