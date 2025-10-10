import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Document } from '../../domain/generated/apimodel';
import { ApiService } from '../../services/api.service';
import { DocumentComponent } from "../document/document.component";

@Component({
    selector: 'order',
    imports: [DocumentComponent],
    templateUrl: './order.component.html',
    styleUrl: './order.component.scss'
})
export class OrderComponent implements OnInit {

  route = inject(ActivatedRoute);
  apiService = inject(ApiService);

  order: Document | undefined;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadData(id);
  }

  loadData(id: number) {
    if (id) {
      this.apiService.getDocument(id).subscribe(order => {
        if (order?.data != undefined && order.data.length > 0) {
          this.order = order.data[0];
        }
      });
    }
  }

}
