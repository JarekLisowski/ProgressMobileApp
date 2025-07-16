import { NgxIndexedDBService, WithID } from "ngx-indexed-db";
import { from, map, Observable, of, switchMap, tap } from "rxjs";
import { Customer, Product } from "../domain/generated/apimodel";
import { CartItem, CartItemWithId } from "../domain/cartItem";
import { inject, Injectable } from "@angular/core";
import { SpecialOfferEdit } from "../domain/specialOfferEdit";
import { CartPromoItem, CartPromoItemWithId } from "../domain/cartPromoItem";
import { Transaction } from "../domain/transaction";

@Injectable({
    providedIn: 'root'
})
export class CartService {

    private readonly storeTransaction = 'transaction';

    constructor(private dbService: NgxIndexedDBService) { }

    addItemToCart(product: Product, quantity: number): Observable<any> {
        var res = this.dbService.getAllByIndex<CartItemWithId>('cart', 'code', IDBKeyRange.only(product.code)).pipe(
            tap(x => {
                console.log(x);
            }),
            map(itemsWithCode => itemsWithCode.find(item => item.promoSetId == 0)),
            switchMap(itemExistsingOrEmpty => {
                if (itemExistsingOrEmpty) {
                    return this.updateCartItemQuntity(itemExistsingOrEmpty.id, itemExistsingOrEmpty.quantity + quantity);
                }
                else {
                    var cartItem: CartItem =
                    {
                        productId: product.id ?? 0,
                        name: product.name ?? "",
                        code: product.code ?? "",
                        priceNet: product.price?.priceNet ?? 0,
                        priceGross: product.price?.priceGross ?? 0,
                        taxRate: product.price?.taxPercent ?? 23,
                        quantity: quantity,
                        promoSetId: 0,
                        promoItemId: 0,
                        imageUrl: ""
                    };
                    return this.dbService.add('cart', cartItem);
                }
            })
        );
        return res;
    }

    addItemsToCart(cartItems: CartItem[]): Observable<number[]> {
        return this.dbService.bulkAdd<CartItem>('cart', cartItems);
    }


    updateCartItemQuntity(id: number, quantity: number): Observable<CartItem> {
        return this.dbService.getByID<CartItem>('cart', id).pipe(
            switchMap(item => {
                item.quantity = quantity;
                return this.dbService.update('cart', item);
            })
        );
    }

    getCartItems(): Observable<(CartItem & WithID)[]> {
        return this.dbService.getAll<CartItem & WithID>('cart');
    }

    getCartItemsForPromoSet(promoSetId: number): Observable<CartItemWithId[]> {
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

    removePromoSetFromCart(promoSetId: number): Observable<any> {
        return this.dbService.deleteAllByIndex('cart', 'promoSetId', IDBKeyRange.only(promoSetId)).pipe(
            switchMap(x => {
                return this.dbService.delete('promoSet', promoSetId);
            })
        )
    }

    getPromoItems(): Observable<CartPromoItemWithId[]> {
        return this.dbService.getAll<CartPromoItemWithId>('promoSet');
    }

    getPromoItem(id: number): Observable<CartPromoItem> {
        return this.dbService.getByID<CartPromoItem>('promoSet', id);
    }

    getCurrentTransaction(): Observable<Transaction> {
        return this.getFirstOrCreateTransaction();
    }

    private getFirstOrCreateTransaction(): Observable<Transaction> {
        // First, try to get all transactions to check if the store is empty.
        return from(this.dbService.getAll<Transaction>(this.storeTransaction)).pipe(
            switchMap((transactions: Transaction[]) => {
                if (transactions && transactions.length > 0) {
                    // If transactions exist, return the first one.
                    //console.log('Transaction store is not empty. Returning first transaction:', transactions[0]);
                    return of(transactions[0]);
                } else {
                    // If the store is empty, create a new default transaction.
                    const newTransaction: Transaction = new Transaction();

                    // Add the new transaction to the database.
                    // The 'add' method returns the key of the newly added object.
                    // We then retrieve the full object using the key (or assume the input object is sufficient).
                    //console.log('Transaction store is empty. Adding a new transaction:', newTransaction);
                    return from(this.dbService.add<Transaction>(this.storeTransaction, newTransaction));
                    //   .pipe(
                    //     map(t => { 
                    //       return t;
                    //     })
                    //   );
                }
            })
        );
    }

    updateTransaction(transaction: Transaction): Observable<Transaction> {
        return this.dbService.update('transaction', transaction);
    }

    clearTransaction(transaction: Transaction): Observable<void> {
        return this.dbService.clear('transaction');
    }

    setCustomer(customer: Customer): Observable<Transaction> {
        return this.getCurrentTransaction().pipe(
            switchMap(tran => {
                tran.customer = customer;
                return this.updateTransaction(tran);
            })
        );
    }

    setDocument(value: string) {
        return this.getCurrentTransaction().pipe(
            switchMap(tran => {
                tran.document = value;
                return this.updateTransaction(tran);
            })
        );
    }

    setPayment(value: number) {
        return this.getCurrentTransaction().pipe(
            switchMap(tran => {
                tran.secondPaymentMethod = value;
                return this.updateTransaction(tran);
            })
        );
    }

    setDelivery(value: number) {
        return this.getCurrentTransaction().pipe(
            switchMap(tran => {
                tran.deliveryMethod = value;
                return this.updateTransaction(tran);
            })
        )
    }

    setPaymentValues(cash: number, other: number) {
        return this.getCurrentTransaction().pipe(
            switchMap(tran => {
                tran.cashAmount = cash;
                tran.secondMethodAmount = other;
                return this.updateTransaction(tran);
            })
        )
    }

    setComment(comment: string) {
        return this.getCurrentTransaction().pipe(
            switchMap(tran => {
                tran.comment = comment;
                return this.updateTransaction(tran);
            })
        );
    }

    setPackages(amount: number) {
        return this.getCurrentTransaction().pipe(
            switchMap(tran => {
                tran.packagesNumber = amount;
                return this.updateTransaction(tran);
            })
        );
    }

}