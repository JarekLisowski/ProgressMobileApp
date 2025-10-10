import { NgxIndexedDBService, WithID } from "ngx-indexed-db";
import { BehaviorSubject, from, map, Observable, of, switchMap, tap } from "rxjs";
import { Customer, Product, PromoSet } from "../domain/generated/apimodel";
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
    //private currentTransaction: Transaction | undefined;

    private _transaction$: BehaviorSubject<Transaction> = new BehaviorSubject<Transaction>(new Transaction());
    private _cartItems$: BehaviorSubject<CartItemWithId[]> = new BehaviorSubject<CartItemWithId[]>([]);
    private _promoItems$: BehaviorSubject<CartPromoItemWithId[]> = new BehaviorSubject<CartPromoItemWithId[]>([]);

    subscribeTransaction$(): Observable<Transaction> {
        return this._transaction$.asObservable();
    }

    getTransaction(): Transaction {
        return this._transaction$.getValue();
    }

    subscribeCartItems$(): Observable<CartItemWithId[]> {
        return this._cartItems$.asObservable();
    }

    subscribePromoItems$(): Observable<CartPromoItemWithId[]> {
        return this._promoItems$.asObservable();
    }

    constructor(private dbService: NgxIndexedDBService) {
        this.getFirstOrCreateTransaction().subscribe(x => {
            this._transaction$.next(x);
        });
        this.getCartItems().subscribe(items => {
            this._cartItems$.next(items);
        });
        this.getPromoItems().subscribe(items => {
            this._promoItems$.next(items);
        });
    }

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
                        imageUrl: "",
                        sumNetto: this.round2((product.price?.priceNet ?? 0) * quantity),
                        sumGross: this.round2(((product.price?.priceNet ?? 0) * quantity) * (1 + (product.price?.taxPercent ?? 23) / 100)),
                    };
                    return this.dbService.add('cart', cartItem).pipe(
                        switchMap(addedItem => {
                            console.log("Add item to cart: " + addedItem.name + " " + addedItem.quantity);                            
                            return this.getPromoItems().pipe(
                                switchMap(promoItems => {
                                    this._promoItems$.next(promoItems);
                                    return this.getCartItems().pipe(
                                        switchMap(cartItems => {
                                            this._cartItems$.next(cartItems);
                                            return this.calculateTotalTransactionValues().pipe(
                                                switchMap(y => of(addedItem))
                                            );
                                        })
                                    );
                                })
                            );
                        })
                    );
                }
            }),
        );
        return res;
    }

    updateCartItemQuntity(id: number, quantity: number): Observable<CartItemWithId> {
        return this.dbService.getByID<CartItemWithId>('cart', id).pipe(
            switchMap(item => {
                if (item.quantity != quantity) {
                    item.quantity = quantity;
                    item.sumNetto = this.round2(item.priceNet * item.quantity);
                    item.sumGross = this.round2(item.sumNetto * (1 + item.taxRate / 100));
                    return this.dbService.update('cart', item).pipe(
                        switchMap(x => {
                            console.log("updateCartItemQuntity: " + x.name + " " + x.quantity);
                            return this.getCartItems().pipe(
                                switchMap(cartItems => {
                                    this._cartItems$.next(cartItems);
                                    return this.calculateTotalTransactionValues().pipe(
                                        switchMap(y => of(item))
                                    );
                                })
                            );
                        })
                    );
                } else {
                    return new Observable<CartItemWithId>();
                }
            }
            ));
    }

    getCartItems(): Observable<(CartItem & WithID)[]> {
        return this.dbService.getAll<CartItem & WithID>('cart');
    }

    getCartItemsForPromoSet(promoSetId: number): Observable<CartItemWithId[]> {
        return this.dbService.getAllByIndex('cart', 'promoSetId', promoSetId);
    }

    removeItemFromCart(id: number): Observable<any> {
        console.log("Removing item from cart: " + id);
        return this.dbService.delete('cart', id).pipe(
            switchMap(x => {
                console.log("Cart item removed from cart: " + id);
                return this.getPromoItems().pipe(
                    switchMap(promoItems => {
                        console.log("Cart item removed - update promo items: ");
                        this._promoItems$.next(promoItems);
                        return this.getCartItems().pipe(
                            switchMap(cartItems => {
                                console.log("Cart item removed - update cart items: ");
                                this._cartItems$.next(cartItems);
                                return this.calculateTotalTransactionValues().pipe(
                                    switchMap(y => of(id))
                                );
                            })
                        );
                    })
                );
            })
        );
    }

    clearCart(): Observable<any> {
        return this.dbService.clear('cart').pipe(
            switchMap(x => this.calculateTotalTransactionValues() )
        );;
    }

    addOrUpdatePromoSetOnCart(promoSet: SpecialOfferEdit): Observable<CartPromoItemWithId> {
        if (promoSet.id != undefined && promoSet.id > 0) {
            var res = this.removePromoSetFromCart(promoSet.id).pipe(
                switchMap(x => {
                    return this.addPromoSetToCart(promoSet);
                })
            );
            return res;
        }
        else {
            return this.addPromoSetToCart(promoSet);
        }
    }

    private addPromoSetToCart(promoSet: SpecialOfferEdit): Observable<any> {
        if (promoSet.id == undefined) {
            var cartPromoItem: CartPromoItem = CartPromoItem.fromSpecialOfferEdit(promoSet);
            return this.dbService.add('promoSet', cartPromoItem).pipe(
                switchMap(promoSetNew => {
                    console.log("Promoset added to cart:", promoSetNew);
                    var cartItems = promoSet.getAllCartItems(promoSetNew.id);
                    return this.addItemsToCart(cartItems).pipe(
                        tap(x =>
                            console.log("Promo items added to cart:", x)
                        )
                    );
                })
            );
        }
        return new Observable<any>();
    }

    private addItemsToCart(cartItems: CartItem[]): Observable<number[]> {
        cartItems.forEach(item => {
            item.sumNetto = this.round2(item.priceNet * item.quantity);
            item.sumGross = this.round2(item.sumNetto * (1 + (item.taxRate) / 100));
        });
        return this.dbService.bulkAdd<CartItem>('cart', cartItems).pipe(
            switchMap(addedIds => {
                console.log("Add promo items to cart: " + addedIds.length);
                return this.getPromoItems().pipe(
                    switchMap(promoItems => {
                        this._promoItems$.next(promoItems);
                        return this.getCartItems().pipe(
                            switchMap(cartItems => {
                                this._cartItems$.next(cartItems);
                                return this.calculateTotalTransactionValues().pipe(
                                    switchMap(y => of(addedIds))
                                );
                            })
                        );
                    })
                );
            })
        );
    }

    removePromoSetFromCart(promoSetId: number): Observable<any> {
        return this.dbService.deleteAllByIndex('cart', 'promoSetId', IDBKeyRange.only(promoSetId)).pipe(
            switchMap(x => {
                return this.dbService.delete('promoSet', promoSetId).pipe(
                    switchMap(y => this.getPromoItems().pipe(
                        switchMap(promoItems => {
                            this._promoItems$.next(promoItems);
                            return this.getCartItems().pipe(
                                switchMap(cartItems => {
                                    this._cartItems$.next(cartItems);
                                    return this.calculateTotalTransactionValues().pipe(
                                        switchMap(z => of(promoSetId))
                                    );
                                })
                            );
                        })
                    ))
                )
            })
        )
    }

    getPromoItems(): Observable<CartPromoItemWithId[]> {
        return this.dbService.getAll<CartPromoItemWithId>('promoSet');
    }

    getPromoItem(id: number): Observable<CartPromoItem> {
        return this.dbService.getByID<CartPromoItem>('promoSet', id);
    }

    private getCurrentTransaction(): Transaction { //Observable<Transaction> {
        return this._transaction$.getValue();
    }

    private getFirstOrCreateTransaction(): Observable<Transaction> {
        // First, try to get all transactions to check if the store is empty.
        return this.dbService.getAll<Transaction>(this.storeTransaction).pipe(
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
                    return this.dbService.add<Transaction>(this.storeTransaction, newTransaction);
                }
            })
        );
    }

    updateTransaction(transaction: Transaction): Observable<Transaction> {
        return this.dbService.update('transaction', transaction).pipe(
            tap(x => this._transaction$.next(x) )
        );
    }

    clearTransaction(transaction: Transaction): Observable<any> {
        return this.dbService.clear('transaction').pipe(
            switchMap(x => {
                return this.getFirstOrCreateTransaction().pipe(
                    tap(newTran => {
                        this._transaction$.next(newTran);
                        this._cartItems$.next([]);
                        this._promoItems$.next([]);
                        return this.calculateTotalTransactionValues();
                    })
                )
            })
        );
    }

    setCustomer(customer: Customer): Observable<Transaction> {
        var tran = this.getCurrentTransaction();
        tran.customer = customer;
        return this.updateTransaction(tran);
    }

    setDocument(value: string) {
        var tran = this.getCurrentTransaction();
        if (tran.document == value) {
            return of(tran);
        }
        tran.document = value;
        return this.updateTransaction(tran);
    }

    setPayment(value: number) {
        var tran = this.getCurrentTransaction();
        if (tran.secondPaymentMethod == value) {
            return of(tran);
        }
        tran.secondPaymentMethod = value;
        if (value == 0) {
            tran.secondMethodAmount = 0;
            tran.cashAmount = tran.totalGross;
        } else {
            tran.cashAmount = 0;
            tran.secondMethodAmount = tran.totalGross;
        }
        return this.updateTransaction(tran);
    }

    setDelivery(id: number, serviceId: number | undefined, priceGross: number | undefined, priceNet: number | undefined, taxRate: number | undefined) {
        var tran = this.getCurrentTransaction();
        if (tran.deliveryMethod == id &&
            (serviceId == undefined || tran.deliveryServiceId == serviceId) &&
            (taxRate == undefined || tran.deliveryTaxRate == taxRate) &&
            (priceNet == undefined || tran.deliveryNet == priceNet)) {
            return of(tran);
        }
        tran.deliveryMethod = id;
        tran.deliveryServiceId = serviceId;
        tran.deliveryTaxRate = taxRate ?? 23;
        tran.deliveryNet = priceNet ?? 0;
        tran.deliveryGross = this.round2(tran.deliveryNet * (1 + (tran.deliveryTaxRate / 100)));
        return this.updateTransaction(tran).pipe(
            switchMap(x => {
                return this.calculateTotalTransactionValues();
            })
        );
    }

    setPaymentValues(cash: number | undefined, other: number | undefined): Observable<Transaction> {
        var tran = this.getCurrentTransaction();
        if (cash != undefined) {
            cash = this.round2(cash);
            var second = this.round2(tran.totalGross - cash);
            if (cash == tran.cashAmount && second == tran.secondMethodAmount) {
                return of(tran);
            }
            tran.cashAmount = cash;
            tran.secondMethodAmount = second;
        }
        else if (other != undefined) {
            cash = this.round2(tran.totalGross - other);
            if (other == tran.secondMethodAmount && cash == tran.cashAmount) {
                return of(tran);
            }
            tran.cashAmount = cash;
            tran.secondMethodAmount = other;
        }
        return this.updateTransaction(tran);
    }

    setPaymentDueDays(days: number) {
        var tran = this.getCurrentTransaction();
        if (tran.paymentDueDays == days) {
            return of(tran);
        }
        tran.paymentDueDays = days;
        return this.updateTransaction(tran);
    }

    setComment(comment: string) {
        var tran = this.getCurrentTransaction();
        if (tran.comment == comment) {
            return of(tran);
        }
        tran.comment = comment;
        return this.updateTransaction(tran);
    }

    setPackages(amount: number) {
        var tran = this.getCurrentTransaction();
        if (tran.packagesNumber == amount) {
            return of(tran);
        }
        tran.packagesNumber = amount;
        return this.updateTransaction(tran);
    }

    calculateTotalTransactionValues(): Observable<Transaction> {
        console.log("calculateTotalTransactionValues");
        var items = this._cartItems$.getValue();
        var vatGroups = this.summarizeCartItemsByVatRate(items);
        console.log(vatGroups);
        var totalItemsNet = 0;
        var totalItemsGross = 0;
        Object.entries(vatGroups).forEach(([key, value]) => {
            var vatRate = this.extractVatRate(key);
            var net = Number(value);
            totalItemsNet += net;
            totalItemsGross += this.round2(net * (1 + vatRate / 100));
        });
        console.log("Total: Net: " + totalItemsNet + ", Gross: " + totalItemsGross);
        var tran = this._transaction$.getValue();
        tran.itemsNet = this.round2(totalItemsNet);
        tran.itemsGross = this.round2(totalItemsGross);
        tran.totalNet = this.round2(tran.itemsNet + tran.deliveryNet);
        tran.totalGross = this.round2(tran.itemsGross + tran.deliveryGross);
        if (tran.totalGross != tran.cashAmount + tran.secondMethodAmount) {
            if (tran.secondMethodAmount > 0) {
                tran.secondMethodAmount = this.round2(tran.totalGross - tran.cashAmount);
            }
            else {
                tran.cashAmount = tran.totalGross;
            }

        }
        console.log("calculateTotalTransactionValues:updateTransaction");
        return this.updateTransaction(tran);
    }

    private round2(value: number): number {
        return Math.round(value * 100) / 100;
    }

    private summarizeCartItemsByVatRate(cartItems: CartItem[]) {
        const summarizedData = cartItems.reduce((accumulator: any, item) => {
            // If the vatRate key doesn't exist in the accumulator, initialize it to 0
            var key = "VAT_" + item.taxRate;
            if (!accumulator.hasOwnProperty(key)) {
                accumulator[key] = 0;
            }
            // Add the current item's lineTotalNet to the corresponding vatRate
            accumulator[key] += item.sumNetto;
            return accumulator;
        }, {}); // Initialize the accumulator as an empty object

        return summarizedData;
    }

    extractVatRate(vatString: string): number {
        // Remove "VAT_" prefix
        const numberString = vatString.replace("VAT_", "");
        // Convert the remaining string to a number
        const vatRate = Number(numberString);
        return vatRate;
    }
}