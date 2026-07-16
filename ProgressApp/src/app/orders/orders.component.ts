import { Component, inject, Inject, Input, OnInit } from '@angular/core';
import { DocumentsComponent } from "../documents/documents.component";
import { Customer, Document } from '../../domain/generated/apimodel';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CustomerSelectComponent } from '../customer-select/customer-select.component';

@Component({
  selector: 'orders',
  imports: [DocumentsComponent, FormsModule, CustomerSelectComponent],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss'
})
export class OrdersComponent implements OnInit {

  router = inject(Router);
  apiService = inject(ApiService);

  customer: Customer | undefined;

  private _customerId: number = 0;

  public get customerId(): number {
    return this._customerId;
  }

  @Input()
  public set customerId(value: number) {
    this._customerId = value;
    this.loadData();
  }

  @Input()
  public hideCustomerSelect: boolean = false;

  showCustomerName: boolean = false;
  data: Document[] = [];
  private dataLoaded: boolean = false;

  private _notRealizedOnly: boolean = true;

  set notRealizedOnly(value: boolean) {
    this._notRealizedOnly = value;
    this.dataLoaded = false;
    this.loadData();
  }

  get notRealizedOnly(): boolean {
    return this._notRealizedOnly;
  }

  get orderStatus(): number | null {
    return this.notRealizedOnly ? 2 : 0;
  }

  ngOnInit(): void {
    this.loadData();    
  }

  loadData() {
    if (this.dataLoaded)
      return;

    this.dataLoaded = true;
    if (this.customerId > 0) {

      this.apiService.getOrders(this.customerId, this.orderStatus).subscribe(x => {
        if (x?.data != undefined) {
          this.data = x.data;
        }
      });
    } else {
      this.showCustomerName = true;
      this.apiService.getOrdersOwnCustomers(this.customerId, this.orderStatus).subscribe(x => {
        if (x?.data != undefined) {
          this.data = x.data;
        }
      });
    }
  }

  onDocumentSelected(id: number) {
    this.router.navigate(['/order', id]);
  }

  customerSelected($event: Customer) {
      this.customer = $event;
      if (this.customer == null || this.customer.id == undefined) {
        this.customerId = 0;
      }
      else
      {
        this.customerId = this.customer.id;
      }
      this.dataLoaded = false;
      this.loadData();
    }

}
