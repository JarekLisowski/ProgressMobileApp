import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Document, IPayment } from '../../domain/generated/apimodel';
import { ApiService } from '../../services/api.service';
import { DocumentComponent } from "../document/document.component";
import { PayModalComponent } from "../pay-modal/pay-modal.component";
import { PrintService } from '../../services/printService';
import { LoggerService } from '../../services/loggerService';

@Component({
    selector: 'invoice',
    imports: [DocumentComponent, PayModalComponent],
    templateUrl: './invoice.component.html',
    styleUrl: './invoice.component.scss'
})
export class InvoiceComponent implements OnInit {

  @ViewChild('payWindow') payWindowRef!: PayModalComponent;

  route = inject(ActivatedRoute);
  apiService = inject(ApiService);
  printService = inject(PrintService);


  invoice: Document | undefined;
  printInProgress = false;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadData(id);
  }

  loadData(id: number) {
    if (id) {
      this.apiService.getDocument(id).subscribe(invoice => {
        if (invoice?.data != undefined && invoice.data.length > 0) {
          this.invoice = invoice.data[0];
        }
      });
    }
  }

  pay() {
    if (this.invoice) {
      this.payWindowRef.showObservable().subscribe(x => {
        if (x) {
          this.doPay(x);
        }
      });
    }
  }

  doPay(payment: IPayment) {
    this.apiService.payForInvoice(payment).subscribe(x => {
      if (!x.isError && this.invoice?.id) {
        this.loadData(this.invoice?.id);
      }
    });
  }

  print() {
    if (this.invoice?.id) {
      this.printInProgress = true;
      this.printService.printInvoice(this.invoice.id).subscribe(x => {
        console.log(x);
        this.printInProgress = false;
      });
    }
  }

}
