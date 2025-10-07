import { NgClass, NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { PrintService } from '../../services/printService';

@Component({
    selector: 'app-save-doc-summary',
    imports: [NgIf, RouterModule, NgClass],
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

  printInProgress = false;

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
    if (!this.printInProgress && this.paymentId != null) {
      this.printInProgress = true;
      this.printService.printCashReceipt(this.paymentId ?? 0).subscribe({
        next: x => {
          console.log(x);
          this.printInProgress = false;
        },
        error: err => {
          console.error(err);
          this.printInProgress = false;
        }
      });
    }
  }

  printInvoice() {
    if (!this.printInProgress && this.documentId != null) {
      this.printInProgress = true;
      this.printService.printInvoice(this.documentId).subscribe({
        next: x => {
          console.log(x);
          this.printInProgress = false;
        },
        error: err => {
          console.error(err);
          this.printInProgress = false;
        }
      });
    }
  }

}
