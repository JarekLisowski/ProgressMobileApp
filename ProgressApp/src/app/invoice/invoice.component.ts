import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Document, IPayment } from '../../domain/generated/apimodel';
import { ApiService } from '../../services/api.service';
import { DocumentComponent } from "../document/document.component";
import { PayModalComponent } from "../pay-modal/pay-modal.component";
import { PrintService } from '../../services/printService';
import { LoggerService } from '../../services/loggerService';
import { ConfirmModalWindowComponent } from '../confirm-modal-window/confirm-modal-window.component';

@Component({
  selector: 'invoice',
  imports: [DocumentComponent, PayModalComponent, ConfirmModalWindowComponent],
  templateUrl: './invoice.component.html',
  styleUrl: './invoice.component.scss'
})
export class InvoiceComponent implements OnInit {

  @ViewChild('payWindow') payWindowRef!: PayModalComponent;

  @ViewChild('printReceiptWindow') printReceiptWindowRef!: ConfirmModalWindowComponent;

  route = inject(ActivatedRoute);
  apiService = inject(ApiService);
  printService = inject(PrintService);


  invoice: Document | undefined;
  printInProgress = false;
  paymentInProgress = false;
  errorMessage: string = ''; 

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
    this.paymentInProgress = true;
    this.apiService.payForInvoice(payment).subscribe({
      next: x => {
        if (!x.isError && this.invoice?.id) {
          this.loadData(this.invoice?.id);
          if (x.payDocumentId) {
            this.printCashReceiptWindow(x.payDocumentId);
          }
        }
        this.paymentInProgress = false;
      },
      error: err => {
        console.error('Payment error', err);
        this.paymentInProgress = false;
        this.errorMessage = err ?? "Błąd podczas przetwarzania płatności.";
      }
    });
  }

  printInvoice() {
    if (this.invoice?.id) {
      this.printInProgress = true;
      this.printService.printInvoice(this.invoice.id).subscribe(x => {
        console.log(x);
        this.printInProgress = false;
      });
    }
  }

  printCashReceiptWindow(paymentId: number) {
    this.printReceiptWindowRef.title = "Drukowanie?";
    var message = "Wydrukpować potwierdzenie wpłaty?";
    this.printReceiptWindowRef.buttonAcceptText = "Drukuj";
    this.printReceiptWindowRef.showObservable(message).subscribe(x => {
      if (x) {
        this.printCashReceipt(paymentId);
      }
    });
  }

  printCashReceipt(paymentId: number) {
    if (!this.printInProgress && paymentId != null) {
      this.printInProgress = true;
      this.printService.printCashReceipt(paymentId ?? 0).subscribe({
        next: x => {
          console.log(x);
          this.printInProgress = false;
        },
        error: err => {
          console.error(err);
          this.printInProgress = false;
          this.errorMessage = err ?? "Błąd drukowania.";
        }
      });
    }
  }


}
