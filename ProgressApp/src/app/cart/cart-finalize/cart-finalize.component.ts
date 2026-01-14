import { Component, EventEmitter, inject, OnInit, Output, OnDestroy, AfterViewInit, ElementRef, Input } from '@angular/core';
import { ApiService } from '../../../services/api.service';
import { CartService } from '../../../services/cart.service';
import { Transaction } from '../../../domain/transaction';
import { CartItem } from '../../../domain/cartItem';
import { DeliveryMethod, PaymentMethod } from '../../../domain/generated/apimodel';
import { DecimalPipe } from '@angular/common';
import { Subscription } from 'rxjs';

@Component({
  selector: 'cart-finalize',
  imports: [DecimalPipe],
  templateUrl: './cart-finalize.component.html',
  styleUrl: './cart-finalize.component.scss'
})
export class CartFinalizeComponent implements OnInit, OnDestroy, AfterViewInit {

  private readonly cartService = inject(CartService);
  private readonly apiService = inject(ApiService);
  private readonly elementRef = inject(ElementRef);

  @Output() SaveTransaction = new EventEmitter<any>();

  cartItems: CartItem[] = [];
  deliveryMethod: DeliveryMethod | undefined;
  secondPaymentMethod: PaymentMethod | undefined;
  private _deliveryMethods: DeliveryMethod[] = [];
  private _paymentMethods: PaymentMethod[] = [];
  @Input() saving: boolean = false;
  @Input() errorMessage: string = "";

  outOfStock: boolean = false;
  outOfStockSubscription: Subscription | undefined;

  get nazwaDokumentu(): string {
    switch (this.transaction?.document) {
      case "Invoice": return "Faktura";
      case "Order": return "Zamówienie";
      case "Order internal": return "Zamówienie międzymagazynowe";
      default: return "";
    }
  }

  get isInvoice(): boolean {
    return this.transaction?.document == "Invoice";
  }

  get paymentMethodsAvailable(): boolean {
    var _selectedDocument = this.transaction?.document;
    return _selectedDocument == "Invoice" || _selectedDocument == "Order";
  }

  get deliveryMethodsAvailable(): boolean {
    var _selectedDocument = this.transaction?.document;
    return _selectedDocument == "Invoice" || _selectedDocument == "Order";
  }

  get cartTotalGross() {
    var sum = 0;
    this.cartItems.forEach(item => {
      sum += item.priceGross * item.quantity;
    });
    return sum
  }

  get comment(): string {
    return this.transaction?.comment || '';
  }

  get savePossible(): boolean {
    if (this.isInvoice && this.outOfStock) {
      return false;
    }
    return true;
  }

  transaction: Transaction | undefined;
  private subscription: Subscription | undefined;

  ngOnInit(): void {

    this.subscription = this.cartService.subscribeTransaction$().subscribe(trans => {
      this.transaction = trans;

      this.apiService.getPaymentMethods().subscribe(payment => {
        if (!payment.isError && payment.data != null) {
          this._paymentMethods = payment.data;
        }
        this.secondPaymentMethod = this._paymentMethods.find(x => x.id == this.transaction?.secondPaymentMethod);
      });
      console.log("Loading transaction data!");
      this.cartService.getCartItems().subscribe(x => {
        this.cartItems = x;
      });

      this.apiService.getDeliveryMethods().subscribe(delivery => {
        if (!delivery.isError && delivery.data != null) {
          this._deliveryMethods = delivery.data.filter(v => {
            return (v.minValue == null || (this.transaction?.itemsGross && this.transaction?.itemsGross >= v.minValue)) &&
              (v.maxValue == null || (this.transaction?.itemsGross && this.transaction?.itemsGross <= v.maxValue));
          });
          this.deliveryMethod = this._deliveryMethods.find(x => x.id == this.transaction?.deliveryMethod);
        }
      });

    });

    this.outOfStockSubscription = this.cartService.subscribeOutOfStock$().subscribe(value => {
      this.outOfStock = value;
      console.log("Out of stock status updated: " + value);
    });

  }

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

  loadDeliveryMethods() {
    this.apiService.getDeliveryMethods().subscribe(delivery => {
      if (!delivery.isError && delivery.data != null) {
        this._deliveryMethods = delivery.data.filter(v => {
          return (v.minValue == null || (this.transaction?.itemsGross && this.transaction?.itemsGross >= v.minValue)) &&
            (v.maxValue == null || (this.transaction?.itemsGross && this.transaction?.itemsGross <= v.maxValue));
        }
        );
      }
    });
  }

  saveTransaction() {
    this.SaveTransaction.emit();
  }
}
