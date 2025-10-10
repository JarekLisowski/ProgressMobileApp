import { AfterViewInit, Component, ElementRef, inject, OnDestroy, OnInit } from '@angular/core';
import { CartService } from '../../../services/cart.service';
import { ApiService } from '../../../services/api.service';
import { DeliveryMethod, PaymentMethod } from '../../../domain/generated/apimodel';

import { FormsModule } from '@angular/forms';
import { Transaction } from '../../../domain/transaction';
import { Subscription } from 'rxjs';

@Component({
  selector: 'cart-options',
  imports: [FormsModule],
  templateUrl: './cart-options.component.html',
  styleUrl: './cart-options.component.scss'
})
export class CartOptionsComponent implements OnInit, OnDestroy, AfterViewInit {

  private readonly cartService = inject(CartService);
  private readonly apiService = inject(ApiService);
  private readonly elementRef = inject(ElementRef);

  private intersectionObserver: IntersectionObserver | undefined;

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

  get paymentDueDays(): number {
    return this._paymentDueDays;
  }

  set paymentDueDays(value: number) {
    if (value > 14)
      value = 14;
    else if (value < 1)
      value = 1;
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
    this.cartService.setPaymentValues(undefined, value).subscribe(x => {
      this._secondPaymentAmount = x.secondMethodAmount;
      this._cashAmount = x.cashAmount;
    });
  }

  get secondPaymentAmount(): number {
    return this._secondPaymentAmount;
  }

  set selectedDelivery(value: string) {
    console.log("selectedDelivery: " + value);
    var nValue = Number(value);
    var deliveryMethod = this.deliveryMethods.find(x => x.id == nValue);
    if (!this.initializing && deliveryMethod?.id != this._selectedDelivery) {
      console.log("selectedDelivery in transaction: " + value);
      this.cartService.setDelivery(deliveryMethod?.id ?? nValue, deliveryMethod?.twId, deliveryMethod?.priceGross, deliveryMethod?.priceNet, deliveryMethod?.taxRate).subscribe(trans => {
        this._selectedDelivery = nValue;
        this.selectedDeliveryMethod = deliveryMethod;
        this.secondPaymentAmount = trans.secondMethodAmount;
        this.cashAmount = trans.cashAmount;
      });
    }
    if (this.initializing) {
      this._selectedDelivery = nValue;
      this.selectedDeliveryMethod = deliveryMethod;
    }
  }

  get selectedDelivery(): string {
    return this._selectedDelivery?.toString() ?? "";
  }

  set packages(value: number) {
    this.cartService.setPackages(value).subscribe(x => {
      this._packages = x.packagesNumber;
    });
  }

  get packages(): number {
    return this._packages;
  }

  set cashAmount(value: number) {
    this.cartService.setPaymentValues(value, this.secondPaymentAmount).subscribe(x => {
      this._cashAmount = x.cashAmount;
      this._secondPaymentAmount = x.secondMethodAmount;
    });
  }

  get cashAmount(): number {
    return this._cashAmount;
  }

  set comment(value: string) {
    this.cartService.setComment(value).subscribe(x => {
      this._comment = x.comment;
    });;
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
    this.apiService.getPaymentMethods().subscribe(x => {
      if (!x.isError && x?.data != null)
        this.paymentMethods = x.data

    });

    this.subscription = this.cartService.subscribeTransaction$().subscribe(trans => {
      console.log("Loading transaction data");
      this.transaction = trans;
      this.selectedDocument = trans.document;
      this.selectedPayment = trans.secondPaymentMethod?.toString() ?? "";
      this.secondPaymentAmount = trans.secondMethodAmount;
      this.cashAmount = trans.cashAmount;
      this.selectedDelivery = trans.deliveryMethod?.toString() ?? "";
      this.comment = trans.comment;
      this.packages = trans.packagesNumber;
      this.paymentDueDays = trans.paymentDueDays;
      this.orderGross = trans.itemsGross ?? 0;
      this.orderNet = trans.itemsNet ?? 0;

      this.apiService.getDeliveryMethods().subscribe(x => {
        if (!x.isError && x?.data != null) {
          this.deliveryMethods = x.data.filter(v => {
            return (v.minValue == null || (this.orderGross >= v.minValue)) &&
              (v.maxValue == null || (this.orderGross <= v.maxValue));
          });
        }
      });
      this.initializing = false;
    });
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  ngAfterViewInit(): void {
  //   this.intersectionObserver = new IntersectionObserver(entries => {
  //     if (entries[0].isIntersecting) {
  //       this.loadTransactionData();
  //     }
  //   });
  //   this.intersectionObserver.observe(this.elementRef.nativeElement);
  }

  // ngOnDestroy(): void {
  //   if (this.intersectionObserver) {
  //     this.intersectionObserver.disconnect();
  //   }
  // }



}
