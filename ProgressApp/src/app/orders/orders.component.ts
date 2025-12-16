import { Component, inject, Inject, Input, OnInit } from '@angular/core';
import { DocumentsComponent } from "../documents/documents.component";
import { Document } from '../../domain/generated/apimodel';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'orders',
  imports: [DocumentsComponent],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss'
})
export class OrdersComponent implements OnInit {

  router = inject(Router);
  apiService = inject(ApiService);

  private _customerId: number = 0;

  public get customerId(): number {
    return this._customerId;
  }

  @Input()
  public set customerId(value: number) {
    this._customerId = value;
    this.loadData();
  }

  showCustomerName: boolean = false;
  data: Document[] = [];
  private dataLoaded: boolean = false;

  ngOnInit(): void {
    this.loadData();    
  }

  loadData() {
    if (this.dataLoaded)
      return;

    this.dataLoaded = true;
    if (this.customerId > 0) {

      this.apiService.getOrders(this.customerId).subscribe(x => {
        if (x?.data != undefined) {
          this.data = x.data;
        }
      });
    } else {
      this.showCustomerName = true;
      this.apiService.getOrdersOwnCustomers(this.customerId).subscribe(x => {
        if (x?.data != undefined) {
          this.data = x.data;
        }
      });
    }
  }

  onDocumentSelected(id: number) {
    this.router.navigate(['/order', id]);
  }


}
