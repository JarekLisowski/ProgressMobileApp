import { Component } from '@angular/core';
import { CustomerListComponent } from "../customer-list/customer-list.component";
import { Customer } from '../../domain/generated/apimodel';
import { Router } from '@angular/router';

@Component({
    selector: 'app-customers',
    imports: [CustomerListComponent],
    templateUrl: './customers.component.html',
    styleUrl: './customers.component.scss'
})
export class CustomersComponent {

  constructor(private router: Router) {}

  selectCustomer(customer: Customer) {
    this.router.navigate(['/customer', customer.id]);
  }

}
