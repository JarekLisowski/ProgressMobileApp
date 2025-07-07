import { Component, EventEmitter, Output } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { FormsModule, NgModel } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { Customer } from '../../domain/generated/apimodel';

@Component({
  selector: 'customer-list',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf],
  templateUrl: './customer-list.component.html',
  styleUrl: './customer-list.component.scss'
})
export class CustomerListComponent {

  selectedCustomer: Customer | undefined;
  _searchPattern: string = '';
  searchTimeout: any;
  
  customerList: Customer[] = [];
  loading: boolean = false;
  
  @Output() customerSelectedEvent = new EventEmitter<Customer>();

  set searchPattern(v: string) {
    this._searchPattern = v
    this.loading = false;;
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.search();
    }, 500);
  }

  constructor(private api: ApiService) {

  }

  search() {
    this.loading = true;
    this.customerList = [];
    this.api.getCustomerList(this._searchPattern).subscribe(response => {
      this.customerList = response.data ?? [];
      this.loading = false;
    });
  }

  selectCustomer(customerId: number | undefined) {
    this.selectedCustomer = this.customerList.find(customer => customer.id == customerId);
    this.customerSelectedEvent.emit(this.selectedCustomer);
  }

}
