import { Component, inject, Inject, Input, OnInit } from '@angular/core';
import { DocumentsComponent } from "../documents/documents.component";
import { Document } from '../../domain/generated/apimodel';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'invoices',
  imports: [DocumentsComponent, FormsModule],
  templateUrl: './invoices.component.html',
  styleUrl: './invoices.component.scss'
})
export class InvoicesComponent implements OnInit {

  router = inject(Router);
  apiService = inject(ApiService);

  private _customerId: number = 0;
  onlyUnpaid: boolean = false;

  public get customerId(): number {
    return this._customerId;
  }

  @Input()
  public set customerId(value: number) {
    this._customerId = value;
    this.loadData();
  }

  showCustomerName: boolean = false;
  allData: Document[] = [];
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
      this.apiService.getInvoices(this.customerId).subscribe(x => {
        if (x?.data != undefined) {
          this.allData = x.data;
          this.data = x.data;
        }
      });
    }
    else {
      this.showCustomerName = true;
      this.apiService.getInvoicesOwnCustomers(this.customerId).subscribe(x => {
        if (x?.data != undefined) {
          this.allData = x.data;
          this.data = x.data;
        }
      });
    }
  }

  onFilterChanged() {
    //this.dataLoaded = false;
    //this.loadData();
    if (this.onlyUnpaid) {
      this.data = this.allData.filter(x => x.paymentToBeSettled ?? 0 > 0);
    } else {
      this.data = this.allData;
    }
  }

  onDocumentSelected(id: number) {
    this.router.navigate(['/invoice', id]);
  }


}
