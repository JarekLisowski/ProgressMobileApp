import { NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { PrintService } from '../../services/printService';

@Component({
  selector: 'app-save-doc-summary',
  standalone: true,
  imports: [NgIf, RouterModule],
  templateUrl: './save-doc-summary.component.html',
  styleUrl: './save-doc-summary.component.scss'
})
export class SaveDocSummaryComponent {
  documentId: number | undefined;
  documentNumber: string | undefined;
  paymentId: number | undefined;
  docType: string | undefined;

  route = inject(ActivatedRoute);
  printService = inject(PrintService);


  constructor() {
    var sid = this.route.snapshot.paramMap.get('id');
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id && id > 0) {
      this.documentId = id;
      this.documentNumber = this.route.snapshot.queryParamMap.get('number') ?? "";
      this.paymentId = Number(this.route.snapshot.queryParamMap.get('payment'));
      this.docType = this.route.snapshot.queryParamMap.get('docType') ?? "";
    }
  }

  printCashReceipt() {
    this.printService.printCashReceipt(this.paymentId ?? 0);
  }

  printInvoice() {
    this.printService.printInvoice(this.documentId ?? 0);
  }

}
