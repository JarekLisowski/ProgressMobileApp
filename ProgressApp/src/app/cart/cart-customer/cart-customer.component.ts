import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { CustomerSelectComponent } from "../../customer-select/customer-select.component";
import { Customer } from '../../../domain/generated/apimodel';
import { CartService } from '../../../services/cart.service';
import { Transaction } from '../../../domain/transaction';
import { Subscription } from 'rxjs';

@Component({
  selector: 'cart-customer',
  imports: [CustomerSelectComponent],
  templateUrl: './cart-customer.component.html',
  styleUrl: './cart-customer.component.scss'
})
export class CartCustomerComponent {

  private readonly cartService = inject(CartService);

  customer: Customer | undefined;

  private transaction: Transaction | undefined;
  private subscription: Subscription | undefined;

  ngOnInit(): void {
    this.subscription = this.cartService.subscribeTransaction$().subscribe(trans => {
      console.log("Loading transaction data!");
      this.transaction = trans;
      this.customer = trans.customer;
    });
  }


  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  // loadCustomer() {
  //   this.cartService.getCurrentTransaction().subscribe(transaction => {
  //     //console.log('Transaction:');
  //     //console.log(transaction);
  //     this.customer = transaction.customer;
  //   });
  // }

  private zxc($event: Customer) {
    this.customer = $event;
    if (this.transaction) 
    {
      this.transaction.customer = this.customer;
      this.cartService.updateTransaction(this.transaction).subscribe(x => {
        console.log('Transaction updated:', x);
      });
    }
  }
}
