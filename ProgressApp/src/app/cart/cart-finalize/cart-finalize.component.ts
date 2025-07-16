import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { ApiService } from '../../../services/api.service';
import { CartService } from '../../../services/cart.service';
import { Transaction } from '../../../domain/transaction';
import { CartItem } from '../../../domain/cartItem';

@Component({
  selector: 'cart-finalize',
  standalone: true,
  imports: [],
  templateUrl: './cart-finalize.component.html',
  styleUrl: './cart-finalize.component.scss'
})
export class CartFinalizeComponent implements OnInit {

  private readonly cartService = inject(CartService);
  private readonly apiService = inject(ApiService);

  @Output() SaveTransaction = new EventEmitter<any>();

  transaction: Transaction | undefined;
  cartItems: CartItem[] = [];
  deliveryMethodName: string | undefined = "";
  secondPaymentMethodName: string | undefined = "";

  get cartTotalGross() {
    var sum = 0;
    this.cartItems.forEach(item => {
      sum += item.priceGross * item.quantity;
    });
    return sum
  }

  ngOnInit(): void {
    this.cartService.getCurrentTransaction().subscribe(x => {
      this.transaction = x;
    });
    this.cartService.getCartItems().subscribe(x => {
      this.cartItems = x;
      this.apiService.getDeliveryMethods().subscribe(delivery => {
        if (!delivery.isError && delivery.data != null) {
          this.deliveryMethodName = delivery.data.find(x => x.id == this.transaction?.deliveryMethod)?.name;
        }
      });
      this.apiService.getPaymentMethods().subscribe(payment => {
        if (!payment.isError && payment.data != null) {
          this.secondPaymentMethodName = payment.data.find(x => x.id == this.transaction?.secondPaymentMethod)?.name ?? "";
        }
      });
    });
  }

  saveTransaction() {
    this.SaveTransaction.emit();
  }
}
