import { Component, EventEmitter, inject, OnInit, Output, OnDestroy, AfterViewInit, ElementRef, Input } from '@angular/core';
import { ApiService } from '../../../services/api.service';
import { CartService } from '../../../services/cart.service';
import { Transaction } from '../../../domain/transaction';
import { CartItem } from '../../../domain/cartItem';
import { DeliveryMethod, PaymentMethod } from '../../../domain/generated/apimodel';
import { CommonModule, NgIf } from '@angular/common';

@Component({
  selector: 'cart-finalize',
  standalone: true,
  imports: [NgIf, CommonModule],
  templateUrl: './cart-finalize.component.html',
  styleUrl: './cart-finalize.component.scss'
})
export class CartFinalizeComponent implements OnInit, OnDestroy, AfterViewInit {

  private readonly cartService = inject(CartService);
  private readonly apiService = inject(ApiService);
  private readonly elementRef = inject(ElementRef);
  private intersectionObserver: IntersectionObserver | undefined;

  @Output() SaveTransaction = new EventEmitter<any>();

  transaction: Transaction | undefined;
  cartItems: CartItem[] = [];
  deliveryMethod: DeliveryMethod | undefined;
  secondPaymentMethod: PaymentMethod | undefined;
  private _deliveryMethods: DeliveryMethod[] = [];
  private _paymentMethods: PaymentMethod[] = [];
  @Input() saving: boolean = false;
  @Input() errorMessage: string = "";

  get cartTotalGross() {
    var sum = 0;
    this.cartItems.forEach(item => {
      sum += item.priceGross * item.quantity;
    });
    return sum
  }

  ngOnInit(): void {
     this.apiService.getDeliveryMethods().subscribe(delivery => {
        if (!delivery.isError && delivery.data != null) {
          this._deliveryMethods = delivery.data;
        }
     });

    this.apiService.getPaymentMethods().subscribe(payment => {
      if (!payment.isError && payment.data != null) {
        this._paymentMethods = payment.data;
      }
    });
  }

  ngAfterViewInit(): void {
    this.intersectionObserver = new IntersectionObserver(entries => {
      if (entries[0].isIntersecting) {
        this.loadTransactionData();
      }
    });
    this.intersectionObserver.observe(this.elementRef.nativeElement);
  }

  ngOnDestroy(): void {
    if (this.intersectionObserver) {
      this.intersectionObserver.disconnect();
    }
  }

  loadTransactionData() {
    console.log("Loading transaction data");
    this.cartService.getCartItems().subscribe(x => {
      this.cartItems = x;
    });
    this.cartService.getCurrentTransaction().subscribe(x => {
      this.transaction = x;
      this.deliveryMethod = this._deliveryMethods.find(x => x.id == this.transaction?.deliveryMethod);
      this.secondPaymentMethod = this._paymentMethods.find(x => x.id == this.transaction?.secondPaymentMethod);
    });
  }

  saveTransaction() {
    this.SaveTransaction.emit();
  }
}
