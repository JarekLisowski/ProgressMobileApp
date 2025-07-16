import { Component, inject, OnInit } from '@angular/core';
import { CustomerSelectComponent } from "../../customer-select/customer-select.component";
import { Customer } from '../../../domain/generated/apimodel';
import { CartService } from '../../../services/cart.service';

@Component({
  selector: 'cart-customer',
  standalone: true,
  imports: [CustomerSelectComponent],
  templateUrl: './cart-customer.component.html',
  styleUrl: './cart-customer.component.scss'
})
export class CartCustomerComponent implements OnInit {
  
  private readonly cartService = inject(CartService);

  customer: Customer | undefined;

  ngOnInit(): void {
    this.loadCustomer();
  }

  loadCustomer() {
    this.cartService.getCurrentTransaction().subscribe(transaction => {
      console.log('Transaction:');
      console.log(transaction);
      this.customer = transaction.customer;
    });
  }

  customerSelected($event: Customer) {
    this.customer = $event;
    this.cartService.getCurrentTransaction().subscribe(transaction => {
      console.log('Transaction:');
      console.log(transaction);
      transaction.customer = this.customer;
      this.cartService.updateTransaction(transaction).subscribe(x => {
        console.log('Transaction updated:', x);
      });

    });
  }
}
