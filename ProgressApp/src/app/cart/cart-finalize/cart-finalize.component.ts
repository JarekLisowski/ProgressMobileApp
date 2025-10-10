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
  //private intersectionObserver: IntersectionObserver | undefined;

  @Output() SaveTransaction = new EventEmitter<any>();

  //transaction: Transaction | undefined;
  cartItems: CartItem[] = [];
  deliveryMethod: DeliveryMethod | undefined;
  secondPaymentMethod: PaymentMethod | undefined;
  private _deliveryMethods: DeliveryMethod[] = [];
  private _paymentMethods: PaymentMethod[] = [];
  @Input() saving: boolean = false;
  @Input() errorMessage: string = "";

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

  get cartTotalGross() {
    var sum = 0;
    this.cartItems.forEach(item => {
      sum += item.priceGross * item.quantity;
    });
    return sum
  }

  transaction: Transaction | undefined;
  private subscription: Subscription | undefined;

  ngOnInit(): void {
    this.apiService.getPaymentMethods().subscribe(payment => {
      if (!payment.isError && payment.data != null) {
        this._paymentMethods = payment.data;
      }
    });
    this.subscription = this.cartService.subscribeTransaction$().subscribe(trans => {
      this.transaction = trans;
      console.log("Loading transaction data!");
      this.cartService.getCartItems().subscribe(x => {
        this.cartItems = x;
      });
      this.deliveryMethod = this._deliveryMethods.find(x => x.id == this.transaction?.deliveryMethod);
      this.secondPaymentMethod = this._paymentMethods.find(x => x.id == this.transaction?.secondPaymentMethod);
      this.loadDeliveryMethods();
    });
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  ngAfterViewInit(): void {
    // this.intersectionObserver = new IntersectionObserver(entries => {
    //   if (entries[0].isIntersecting) {
    //     this.loadTransactionData();
    //   }
    // });
    // this.intersectionObserver.observe(this.elementRef.nativeElement);
  }

  // ngOnDestroy(): void {
  //   if (this.intersectionObserver) {
  //     this.intersectionObserver.disconnect();
  //   }
  // }

  // loadTransactionData() {
  //   console.log("Loading transaction data!");
  //   this.cartService.getCartItems().subscribe(x => {
  //     this.cartItems = x;
  //   });
  //   this.cartService.getCurrentTransaction().subscribe(x => {
  //     this.transaction = x;
  //     this.deliveryMethod = this._deliveryMethods.find(x => x.id == this.transaction?.deliveryMethod);
  //     this.secondPaymentMethod = this._paymentMethods.find(x => x.id == this.transaction?.secondPaymentMethod);
  //     this.loadDeliveryMethods();
  //   });
  // }

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
