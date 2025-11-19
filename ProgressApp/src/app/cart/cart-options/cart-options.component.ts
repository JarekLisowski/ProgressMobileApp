import { AfterViewInit, Component, ElementRef, inject, OnDestroy, OnInit } from '@angular/core';
import { CartService } from '../../../services/cart.service';
import { ApiService } from '../../../services/api.service';
import { DeliveryMethod, PaymentMethod } from '../../../domain/generated/apimodel';

import { FormsModule } from '@angular/forms';
import { Transaction } from '../../../domain/transaction';
import { filter, map, Subscription, switchMap, take, tap } from 'rxjs';

@Component({
  selector: 'cart-options',
  imports: [FormsModule],
  templateUrl: './cart-options.component.html',
  styleUrl: './cart-options.component.scss'
})
export class CartOptionsComponent implements OnInit, OnDestroy, AfterViewInit {

  private readonly cartService = inject(CartService);
  private readonly apiService = inject(ApiService);

  private _selectedDocument: string = "";
  private _selectedPayment: number | undefined;
  private _secondPaymentAmount: number = 0;
  private _cashAmount: number = 0;
  private _selectedDelivery: number | undefined;
  private _packages: number = 0;
  private _comment: string = "";

  selectedDeliveryMethod: DeliveryMethod | undefined;
  selectedPaymentMethod: PaymentMethod | undefined;

  orderGross: number = 0;
  orderNet: number = 0;

  _paymentDueDays: number = 14;
  
  outOfStock: boolean = false;
  outOfStockSubscription: Subscription | undefined;

  get cashReadOnly(): boolean {
    return Number(this.selectedPaymentMethod?.id ?? 0) == 0;
  }

  get paymentDueDays(): number {
    return this._paymentDueDays;
  }

  set paymentDueDays(value: number) {
    if (value > 14)
      value = 14;
    else if (value < 1)
      value = 1;
    if (this.transaction?.paymentDueDays == value) {
      this._paymentDueDays = value;
      return;
    }
    if (!this.initializing) {
      this.cartService.setPaymentDueDays(value).subscribe(trans => {
        this._paymentDueDays = trans.paymentDueDays;
      })
    }
    else if (this.initializing) {
      this._paymentDueDays = value;
    }
  }

  set selectedDocument(value: string) {
    if (this.transaction?.document == value) {
      this._selectedDocument = value;
      return;
    }
    this.cartService.setDocument(value).subscribe(x => {
      this._selectedDocument = x.document;
    });
  }

  get selectedDocument(): string {
    return this._selectedDocument;
  }

  get paymentMethodsAvailable(): boolean {
    var _selectedDocument = this.selectedDocument;
    return _selectedDocument == "Invoice" || _selectedDocument == "Order";
  }

  get deliveryMethodsAvailable(): boolean {
    var _selectedDocument = this.selectedDocument;
    return _selectedDocument == "Invoice" || _selectedDocument == "Order";
  }

  set selectedPayment(value: string) {
    var nValue = Number(value);
    var paymentMethod = this.paymentMethods.find(x => x.id == nValue);
    if (this.transaction?.secondPaymentMethod == nValue) {
      this._selectedPayment = nValue;
      this.selectedPaymentMethod = paymentMethod;
      return;
    }
    if (!this.initializing && this._selectedPayment != nValue) {
      this.cartService.setPayment(nValue).subscribe(x => {
        this._selectedPayment = x.secondPaymentMethod;
        this.selectedPaymentMethod = paymentMethod;
        this.secondPaymentAmount = x.secondMethodAmount;
        this.cashAmount = x.cashAmount;
      });
    }
    else if (this.initializing) {
      this._selectedPayment = nValue;
      this.selectedPaymentMethod = paymentMethod;
    }
    console.log("selectedPayment: " + value)
  }

  get selectedPayment(): string {
    return this._selectedPayment?.toString() ?? "";
  }

  set secondPaymentAmount(value: number) {
    if (this.transaction?.secondMethodAmount == value) {
      this._secondPaymentAmount = value;
      this._cashAmount = this.transaction?.cashAmount ?? 0;
      return;
    }
    this.cartService.setPaymentValues(undefined, value).subscribe(x => {
      this._secondPaymentAmount = x.secondMethodAmount;
      this._cashAmount = x.cashAmount;
    });
  }

  get secondPaymentAmount(): number {
    return this._secondPaymentAmount;
  }

  set selectedDelivery(value: string) {
    var nValue = Number(value);
    var deliveryMethod = this.deliveryMethods.find(x => x.id == nValue);
    var deliveryMethodId = deliveryMethod?.id ?? 0;
    if (this.transaction?.deliveryMethod == deliveryMethodId) {
      this._selectedDelivery = deliveryMethodId;
      this.selectedDeliveryMethod = deliveryMethod;
      return;
    }
    console.log("selectedDelivery: " + deliveryMethod);
    this.cartService.setDelivery(deliveryMethodId ?? nValue, deliveryMethod?.twId, deliveryMethod?.priceGross, deliveryMethod?.priceNet, deliveryMethod?.taxRate).subscribe(trans => {
      this._selectedDelivery = deliveryMethodId;
      this.selectedDeliveryMethod = deliveryMethod;
      this.secondPaymentAmount = trans.secondMethodAmount;
      this.cashAmount = trans.cashAmount;
    });

  }

  get selectedDelivery(): string {
    return this._selectedDelivery?.toString() ?? "";
  }

  set packages(value: number) {
    if (this.transaction?.packagesNumber == value) {
      this._packages = value;
      return;
    }
    this.cartService.setPackages(value).subscribe(x => {
      this._packages = x.packagesNumber;
    });
  }

  get packages(): number {
    return this._packages;
  }

  set cashAmount(value: number) {
    if (this.transaction?.cashAmount == value) {
      this._cashAmount = value;
      this._secondPaymentAmount = this.transaction?.secondMethodAmount ?? 0;
      return;
    }
    this.cartService.setPaymentValues(value, this.secondPaymentAmount).subscribe(x => {
      this._cashAmount = x.cashAmount;
      this._secondPaymentAmount = x.secondMethodAmount;
    });
  }

  get cashAmount(): number {
    return this._cashAmount;
  }

  set comment(value: string) {
    if (this.transaction?.comment == value) {
      this._comment = value;
      return;
    }
    this.cartService.setComment(value).subscribe(x => {
      this._comment = x.comment;
    });
  }

  get comment(): string {
    return this._comment;
  }


  paymentMethods: PaymentMethod[] = [];

  deliveryMethods: DeliveryMethod[] = [];

  private initializing: boolean = true;

  private transaction: Transaction | undefined;
  private subscription: Subscription | undefined;

  ngOnInit(): void {
    // 1. Subscribe to the transaction stream
    this.subscription = this.cartService.subscribeTransaction$().pipe(

      // --- PHASE 1: Initial Filter and Data Extraction ---

      // Use filter to skip processing if the transaction ID is undefined.
      // The cast to <any> should be removed by fixing the 'trans' type definition if possible.
      filter(trans => (<any>trans).id !== undefined),

      // Use tap to perform initial synchronous state updates (if needed)
      tap(trans => {
        // Synchronously update the component state with transaction details
        this.transaction = trans;
        this.selectedDocument = trans.document;
        this.selectedPayment = trans.secondPaymentMethod?.toString() ?? "";
        this.secondPaymentAmount = trans.secondMethodAmount;
        this.cashAmount = trans.cashAmount;
        this.comment = trans.comment;
        this.packages = trans.packagesNumber;
        this.paymentDueDays = trans.paymentDueDays;
        this.orderGross = trans.itemsGross ?? 0;
        this.orderNet = trans.itemsNet ?? 0;
        console.log("Loading transaction data");
      }),

      // --- PHASE 2: Chaining Asynchronous Operations ---

      // 2. Load Payment Methods (First Async Call)
      // Use switchMap to wait for the API call and chain the next step.
      switchMap(trans => this.apiService.getPaymentMethods().pipe(
        // take(1) is kept inside the API pipe since it's the standard practice for HTTP calls
        take(1),
        // Return both the API result (x) and the original transaction (trans)
        map(x => ({ paymentResult: x, trans: trans }))
      )),

      // 3. Process Payment Methods Result and Load Delivery Methods (Second Async Call)
      switchMap(data => {
        const { paymentResult: x, trans } = data;

        // SIDE EFFECT 1: Update component state with payment methods
        if (!x.isError && x?.data != null) {
          this.paymentMethods = x.data;
        }

        // Now start the second async call: getDeliveryMethods()
        return this.apiService.getDeliveryMethods().pipe(
          take(1),
          // Return both the API result (y) and the original data object
          map(y => ({ deliveryResult: y, trans: trans }))
        );
      }),

      // --- PHASE 3: Final Processing and Component Setup ---

      // 4. Final Subscription Block (process delivery methods and set flags)
    ).subscribe(data => {
      const { deliveryResult: x, trans } = data;

      // SIDE EFFECT 2: Process delivery methods and update component state
      if (!x.isError && x?.data != null) {
        this.deliveryMethods = x.data.filter(v => {
          return (v.minValue == null || (this.orderGross >= v.minValue)) &&
            (v.maxValue == null || (this.orderGross <= v.maxValue));
        });
        this.selectedDelivery = trans.deliveryMethod?.toString() ?? "";
      }

      // Final synchronous state update
      this.initializing = false;
    });

    this.outOfStockSubscription = this.cartService.subscribeOutOfStock$().subscribe(value => {
      this.outOfStock = value;
      console.log("Out of stock status updated: " + value);
    });
  }

  // ngOnInit(): void {

  //   this.subscription = this.cartService.subscribeTransaction$().subscribe(trans => {
  //     if ((<any>trans).id === undefined)
  //       return;

  //     this.apiService.getPaymentMethods().pipe(take(1)).subscribe(x => {
  //       if (!x.isError && x?.data != null) {
  //         this.paymentMethods = x.data;
  //       }

  //       console.log("Loading transaction data");
  //       this.transaction = trans;
  //       this.selectedDocument = trans.document;
  //       this.selectedPayment = trans.secondPaymentMethod?.toString() ?? "";
  //       this.secondPaymentAmount = trans.secondMethodAmount;
  //       this.cashAmount = trans.cashAmount;
  //       this.comment = trans.comment;
  //       this.packages = trans.packagesNumber;
  //       this.paymentDueDays = trans.paymentDueDays;
  //       this.orderGross = trans.itemsGross ?? 0;
  //       this.orderNet = trans.itemsNet ?? 0;

  //       this.apiService.getDeliveryMethods().pipe(take(1)).subscribe(x => {
  //         if (!x.isError && x?.data != null) {
  //           this.deliveryMethods = x.data.filter(v => {
  //             return (v.minValue == null || (this.orderGross >= v.minValue)) &&
  //               (v.maxValue == null || (this.orderGross <= v.maxValue));
  //           });
  //           this.selectedDelivery = trans.deliveryMethod?.toString() ?? "";
  //         }
  //       });

  //       this.initializing = false;
  //     });
  //   });
  // }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
    if (this.outOfStockSubscription) {
      this.outOfStockSubscription.unsubscribe();
    }
  }

  ngAfterViewInit(): void {
  }
}
