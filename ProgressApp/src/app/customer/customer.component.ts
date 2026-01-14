import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Customer, Document } from '../../domain/generated/apimodel';
import { InvoicesComponent } from "../invoices/invoices.component";
import { FormsModule } from '@angular/forms';
import { OrdersComponent } from '../orders/orders.component';
import { ConfirmModalWindowComponent } from '../confirm-modal-window/confirm-modal-window.component';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-customer',
  imports: [InvoicesComponent, FormsModule, OrdersComponent, ConfirmModalWindowComponent],
  templateUrl: './customer.component.html',
  styleUrl: './customer.component.scss'
})
export class CustomerComponent implements OnInit {

  @ViewChild('assignCustomerWindow') assignCustomerWindowRef!: ConfirmModalWindowComponent;

  apiService = inject(ApiService);
  private readonly cartService = inject(CartService);

  customer: Customer | undefined;
  customerEdit: Customer = new Customer();
  editMode = false;
  customerId: number = 0;

  constructor(
    private route: ActivatedRoute
  ) {

  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id && id > 0) {
      this.apiService.getCustomer(id).subscribe(customer => {
        this.customer = customer.data;
        console.log(this.customer);
        if (this.customer?.id != undefined)
          this.customerId = this.customer.id;
      });
    }
    else if (id == 0) {
      this.customerEdit = new Customer();
      this.editMode = true;
    }
  }

  edit(): void {
    this.customerEdit = JSON.parse(JSON.stringify(this.customer));
    this.editMode = true;
  }

  save(): void {
    console.log('Saving customer:', this.customerEdit);
    if (this.customerEdit) {
      this.apiService.addOrUpdateCustomer(this.customerEdit).subscribe(x => {
        console.log(x);
        this.customer = this.customerEdit;
      });
    }
    this.editMode = false;
  }

  cancel(): void {
    this.editMode = false;
  }

  assignCustomer() {
    this.assignCustomerWindowRef.title = "Przypisywanie klienta";
    var message = "Przypisać klienta do bieżącej transakcji?";
    this.assignCustomerWindowRef.buttonAcceptText = "Przypisz";
    this.assignCustomerWindowRef.showObservable(message).subscribe(x => {
      if (x && this.customer) {
        this.cartService.setCustomer(this.customer).subscribe(x => {
          console.log('Customer assigned to cart:', x);
        });
      }
    });
  }
}
