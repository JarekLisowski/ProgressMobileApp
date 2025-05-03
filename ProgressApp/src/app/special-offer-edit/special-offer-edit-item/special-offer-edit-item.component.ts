import { AfterViewInit, Component, ElementRef, EventEmitter, inject, Input, Output, ViewChild } from '@angular/core';
import { IPromoItem, Product } from '../../../domain/generated/apimodel';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { ApiService } from '../../../services/api.service';
import { Modal } from 'bootstrap';
import { SpecialOfferProductItemComponent } from "../special-offer-product-item/special-offer-product-item.component";
import { ProductPromoItem } from '../../../domain/ProductPromoItem';
import { PromoItemEdit } from '../../../domain/specialOfferEdit';

@Component({
  selector: 'app-special-offer-edit-item',
  standalone: true,
  imports: [NgIf, NgFor, NgClass, SpecialOfferProductItemComponent],
  templateUrl: './special-offer-edit-item.component.html',
  styleUrl: './special-offer-edit-item.component.scss'
})
export class SpecialOfferEditItemComponent implements AfterViewInit {

  @Input() promoItem!: PromoItemEdit;

  @ViewChild('selectProductModal') myModalRef!: ElementRef;

  @Output() cartChangedEvent = new EventEmitter<void>();
  

  private readonly api = inject(ApiService);

  selectedItemsQuantity: number = 0;
  
  modal!: Modal;
  idCouner : number = -1;
  
  promoProductSelection: ProductPromoItem[] = [];
  maxQuantity: number = 1;
  selectedPromoItemId?: number;

  get isReady() : boolean {
    return this.selectedItemsQuantity == this.promoItem?.quantity;
  } 
  
  ngAfterViewInit(): void {
    console.log(this.myModalRef);
    this.modal = new Modal(this.myModalRef.nativeElement);
    this.myModalRef.nativeElement.addEventListener("hidden.bs.modal", () => {  
      if (this.selectedPromoItemId != undefined) {
        this.promoItem.cartItems = this.promoProductSelection.filter(x => {
          return x.quantity > 0
        }).map(x => {
          return {
            ...x,
            id: 0,
            productId: x.productId ?? 0,
            quantity: x.quantity,
            promoItemId: this.promoItem.id ?? 0,
            priceNet: x.price?.priceNet ?? 0,
            priceGross: x.price?.priceGross ?? 0,
            promoSetId: 0
          }
        });
        //console.log(this.promoItem.cartItems);
        this.cartChangedEvent.emit();
      }
    });
  }

  showProductSelectionForPromoItem(selectedPromoItem: IPromoItem) {    
    this.selectedPromoItemId = selectedPromoItem.id;
    if (selectedPromoItem.id == null) {
      return;
    }
    //this.promoProductSelection = [];
    this.api.getPromoProductsForPromoItem(selectedPromoItem.id).subscribe(x => {
      if (x.data != null) {
        x.data.forEach(promoProduct => {
          var existingItem = this.promoProductSelection.find(x => {
            return x.code == promoProduct.code;
          });
          if (existingItem == null) {
            var productPromoToAdd : ProductPromoItem = {
              ...promoProduct,
              productId: promoProduct.id ?? 0,
              id: this.idCouner,
              quantity: 0,
              promoItemId: selectedPromoItem.id ?? 0,
              //maxQuantity: selectedPromoItem.quantity ?? 1,
            }
            this.promoProductSelection.push(productPromoToAdd);
            this.idCouner--;
          }
          else {
            //this.promoProductSelection.push(existingItem);
          }
          
        });
        this.maxQuantity = selectedPromoItem.quantity ?? 1;
        this.modal.show();
      }
    });
  }

  quantityChanged() {
    console.log("Quantity changed");
    this.calculateTotalQuantity();
  }

  calculateTotalQuantity() {
    this.selectedItemsQuantity = 0;
    this.promoProductSelection.forEach(item => {
      this.selectedItemsQuantity += item.quantity;
    });
  }
}
