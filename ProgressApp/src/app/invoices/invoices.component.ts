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

  dateRanges = ['Dzisiaj', 'Ostatnie 30 dni', 'Ostatni rok', 'Niestandardowy'];
  selectedRange: string = 'Ostatnie 30 dni';
  dateFrom: string | null = new Date().toISOString().split('T')[0];
  dateTo: string | null = new Date().toISOString().split('T')[0];

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    if (this.dataLoaded)
      return;

    this.dataLoaded = true;
    if (this.customerId > 0) {
      this.apiService.getInvoices(this.customerId, this.dateFrom ?? '', this.dateTo ?? '').subscribe(x => {
        if (x?.data != undefined) {
          this.allData = x.data;
          this.data = x.data;
        }
      });
    }
    else {
      this.showCustomerName = true;
      this.apiService.getInvoicesOwnCustomers(this.customerId, this.dateFrom ?? '', this.dateTo ?? '').subscribe(x => {
        if (x?.data != undefined) {
          this.allData = x.data;
          this.data = x.data;
        }
      });
    }
  }

  onFilterChanged() {
    if (this.onlyUnpaid) {
      this.data = this.allData.filter(x => x.paymentToBeSettled ?? 0 > 0);
    } else {
      this.data = this.allData;
    }
  }

  onDocumentSelected(id: number) {
    this.router.navigate(['/invoice', id]);
  }

  onCustomDateRangeChange() {
    this.dataLoaded = false;
    this.loadData();
  }

  onDateRangeChange(range: string) {
    this.selectedRange = range;
    this.setDateRange();
    this.dataLoaded = false;
    this.loadData();
  }

  private setDateRange() {
    const today = new Date();
    switch (this.selectedRange) {
      case 'Dzisiaj':
        this.dateFrom = today.toISOString().split('T')[0];
        this.dateTo = today.toISOString().split('T')[0];
        break;
      case 'Ostatnie 30 dni':
        const thirtyDaysAgo = new Date(today);
        thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
        this.dateFrom = thirtyDaysAgo.toISOString().split('T')[0];
        this.dateTo = today.toISOString().split('T')[0];
        break;
      case 'Ostatni rok':
        const yearAgo = new Date(today);
        yearAgo.setDate(yearAgo.getDate() - 365);
        this.dateFrom = yearAgo.toISOString().split('T')[0];
        this.dateTo = today.toISOString().split('T')[0];
        break;
    }
  }
}
