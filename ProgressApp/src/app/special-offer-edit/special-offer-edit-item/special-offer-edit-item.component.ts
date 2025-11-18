import { AfterViewInit, Component, ElementRef, EventEmitter, inject, Input, Output, ViewChild } from '@angular/core';
import { IPromoItem, Product } from '../../../domain/generated/apimodel';
import { NgClass } from '@angular/common';
import { ApiService } from '../../../services/api.service';
import { Modal } from 'bootstrap';
import { SpecialOfferProductItemComponent } from "../special-offer-product-item/special-offer-product-item.component";
import { ProductPromoItem } from '../../../domain/ProductPromoItem';
import { PromoItemEdit } from '../../../domain/specialOfferEdit';

@Component({
    selector: 'app-special-offer-edit-item',
    imports: [NgClass, SpecialOfferProductItemComponent],
    templateUrl: './special-offer-edit-item.component.html',
    styleUrl: './special-offer-edit-item.component.scss'
})
export class SpecialOfferEditItemComponent implements AfterViewInit {

  @Input() promoItemEdit!: PromoItemEdit;
  @Input() promoSetId!: number | undefined;


  @ViewChild('selectProductModal') myModalRef!: ElementRef;

  @Output() cartChangedEvent = new EventEmitter<void>();


  private readonly api = inject(ApiService);

  selectedItemsQuantity: number = 0;

  modal!: Modal;
  idCouner: number = -1;

  promoProduct: ProductPromoItem[] = [];
  maxQuantity: number = 1;
  isModified: boolean = false;

  get isReady(): boolean {
    return this.selectedItemsQuantity == this.promoItemEdit?.quantity;
  }

  ngAfterViewInit(): void {
    this.modal = new Modal(this.myModalRef.nativeElement);
    this.myModalRef.nativeElement.addEventListener("hidden.bs.modal", () => {
      if (this.isModified)
        this.addSelectdItemsToPromoCart();
    });
    this.loadPromoItems();

  }

  addSelectdItemsToPromoCart() {
    this.promoItemEdit.cartItems = this.promoProduct.filter(x => {
      return x.quantity > 0;
    }).map(x => {
      return {
        ...x,
        id: 0,
        productId: x.productId ?? 0,
        quantity: x.quantity,
        promoItemId: this.promoItemEdit.id ?? 0,
        priceNet: x.price?.priceNet ?? 0,
        priceGross: x.price?.priceGross ?? 0,
        taxRate: x.price?.taxPercent ?? 23,
        promoSetId: 0,
        imageUrl: "",
        sumGross: (x.price?.priceGross ?? 0) * x.quantity,
        sumNetto: (x.price?.priceNet ?? 0) * x.quantity,
        stock: undefined
      };
    });
    this.calculateTotalQuantity();
    this.cartChangedEvent.emit();
  }

  loadPromoItems() {
    if (this.promoItemEdit == null || this.promoItemEdit.id == null)
      return;
    this.api.getPromoProductsForPromoItem(this.promoItemEdit.id).subscribe(x => {
      if (x.data != null) {
        x.data.forEach(promoProduct => {
          var existingItem = this.promoProduct.find(x => {
            return x.code == promoProduct.code;
          });
          if (existingItem == null && this.promoItemEdit?.id != null) {
            var productPromoToAdd: ProductPromoItem = {
              ...promoProduct,
              productId: promoProduct.id ?? 0,
              id: this.idCouner,
              quantity: 0,
              promoItemId: this.promoItemEdit.id,
            }
            this.promoProduct.push(productPromoToAdd);
            this.idCouner--;
          }
        });
        this.maxQuantity = this.promoItemEdit?.quantity ?? 1;
        this.updateQuantity();
        this.isModified = false;
        if (this.promoSetId == undefined || this.promoSetId == 0) {
          this.setSingleItems();
        }
      }
    });
  }

  setSingleItems() {
    if (this.promoProduct.length == 1) {
      this.promoProduct[0].quantity = this.maxQuantity;
      this.addSelectdItemsToPromoCart();
    }
  }

  showProductSelectionForPromoItem() {
    this.modal.show();
  }

  updateQuantity() {
    this.promoProduct.forEach(item => {
      var existingItem = this.promoItemEdit.cartItems.find(x => {
        return x.productId == item.productId;
      });
      if (existingItem != null) {
        item.quantity = existingItem.quantity;
      }
      else {
        item.quantity = 0;
      }
    });
    this.quantityChanged();
  }

  quantityChanged() {
    this.isModified = true;
    this.calculateTotalQuantity();
  }

  calculateTotalQuantity() {
    this.selectedItemsQuantity = 0;
    this.promoProduct.forEach(item => {
      this.selectedItemsQuantity += item.quantity;
    });
  }
}
