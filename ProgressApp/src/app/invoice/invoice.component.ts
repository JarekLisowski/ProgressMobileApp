import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Document } from '../../domain/generated/apimodel';
import { ApiService } from '../../services/api.service';
import { DocumentComponent } from "../document/document.component";

@Component({
  selector: 'invoice',
  standalone: true,
  imports: [DocumentComponent],
  templateUrl: './invoice.component.html',
  styleUrl: './invoice.component.scss'
})
export class InvoiceComponent implements OnInit {

  route = inject(ActivatedRoute);
  apiService = inject(ApiService);

  invoice: Document | undefined;


  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadData(id);
  }

  loadData(id: number) {
    if (id) {
      this.apiService.getInvoice(id).subscribe(invoice => {
        if (invoice?.data != undefined && invoice.data.length > 0) {
          this.invoice = invoice.data[0];
        }
      });
    }
  }

}
