import { Component, inject, Inject, Input, OnInit } from '@angular/core';
import { DocumentsComponent } from "../documents/documents.component";
import { Document } from '../../domain/generated/apimodel';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'invoices',
  standalone: true,
  imports: [DocumentsComponent],
  templateUrl: './invoices.component.html',
  styleUrl: './invoices.component.scss'
})
export class InvoicesComponent implements OnInit {

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


  data: Document[] = [];

  ngOnInit(): void {
    //    this.loadData();    
  }

  loadData() {
    if (this.customerId == 0)
      return;

    this.apiService.getInvoices(this.customerId).subscribe(x => {
      if (x?.data != undefined) {
        this.data = x.data;
      }
    });
  }

  onDocumentSelected(id: number) {
    this.router.navigate(['/invoice', id]);
  }


}
