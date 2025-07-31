import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-save-doc-summary',
  standalone: true,
  imports: [],
  templateUrl: './save-doc-summary.component.html',
  styleUrl: './save-doc-summary.component.scss'
})
export class SaveDocSummaryComponent {

  documentId: number | undefined;
  documentNumber: string | undefined;
  paymentId: number | undefined;

  route = inject(ActivatedRoute);

  constructor() {
    var sid= this.route.snapshot.paramMap.get('id');
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id && id > 0) {
      this.documentId = id;
      this.documentNumber = this.route.snapshot.queryParamMap.get('number') ?? "";
      this.paymentId = Number(this.route.snapshot.queryParamMap.get('payment'));
    }
  }

}
