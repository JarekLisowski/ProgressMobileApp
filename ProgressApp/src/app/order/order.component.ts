import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { concat, concatMap, from, Observable } from 'rxjs';
import { Document, Product } from '../../domain/generated/apimodel';
import { ApiService } from '../../services/api.service';
import { DocumentComponent } from "../document/document.component";
import { ConfirmModalWindowComponent } from "../confirm-modal-window/confirm-modal-window.component";
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'order',
  imports: [DocumentComponent, ConfirmModalWindowComponent],
  templateUrl: './order.component.html',
  styleUrl: './order.component.scss'
})
export class OrderComponent implements OnInit {

  @ViewChild('confirmModal') confirmModalRef!: ConfirmModalWindowComponent;

  private readonly route = inject(ActivatedRoute);
  private readonly apiService = inject(ApiService);
  private readonly cartService = inject(CartService);
  private readonly router = inject(Router);

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

  AddToCart() {
    this.confirmModalRef.title = "Dodawanie do koszyka";
    var message = "Dodajesz produkty z tego dokumentu do koszyka. Zdecyduj co zrobić z produktami, które już są w koszyku.";
    this.confirmModalRef.buttonAcceptText = "Dodaj";
    this.confirmModalRef.checkBox1Value = true;
    this.confirmModalRef.checkBox2Value = true;
    this.confirmModalRef.showObservable(message).subscribe(x => {
      if (x === true) {
        const steps: Observable<any>[] = [];

        if (this.confirmModalRef.checkBox1Value) {
          steps.push(this.cartService.clearCart());
        }

        if (this.confirmModalRef.checkBox2Value && this.order?.customer) {
          steps.push(this.cartService.setCustomer(this.order.customer));
        }

        if (this.order?.items) {
          var codes = this.order.items.map(i => i.product?.code).filter(c => c != undefined) as string[];
          steps.push(
            this.apiService.getProductListByCodes(codes, true).pipe(
              concatMap(res => {
                var itemsToAdd = res.data?.filter(it => it.type == "product") as Product[];
                return from(itemsToAdd).pipe(
                  concatMap(item => {
                    var quantity = this.order?.items?.find(i => i.product?.code == item.code)?.quantity;
                    return this.cartService.addItemToCart(item, quantity!, undefined);
                  })
                );
              })
            )
          );
        }

        if (steps.length > 0) {
          concat(...steps).subscribe(x => {
            this.router.navigate(['/cart']);
          });
        }
      }
    });
  }

}
