import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { CartService } from '../../services/cart-service';
import { NgFor, NgIf } from '@angular/common';
import { PromoItemEdit, SpecialOfferEdit } from '../../domain/specialOfferEdit';
import { SpecialOfferEditItemComponent } from "./special-offer-edit-item/special-offer-edit-item.component";

@Component({
  selector: 'app-special-offer-edit',
  standalone: true,
  imports: [NgFor, NgIf, SpecialOfferEditItemComponent],
  templateUrl: './special-offer-edit.component.html',
  styleUrl: './special-offer-edit.component.scss'
})
export class SpecialOfferEditComponent implements OnInit {
 
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);
  private readonly cartService = inject(CartService);
  
  promoSetId: number = 0;
  promoSet: SpecialOfferEdit = new SpecialOfferEdit();

  private _isReady: boolean = false;

  get isReady(): boolean {
    return this._isReady;
  }

  ngOnInit(): void {
    this.promoSetId = this.route.snapshot.params['id'];
    this.api.getPromoSet(this.promoSetId).subscribe(x => {  
      if (x.data != null && x.data.items != null) {
        this.promoSet = new SpecialOfferEdit();
        this.promoSet.name = x.data.name ?? '';
        this.promoSet.promoSetId = x.data.id ?? 0;
        this.promoSet.promoItemsEdit = x.data.items.map(promoItem => {          
          var newPromoItem = new PromoItemEdit();
          newPromoItem.id = promoItem.id;
          newPromoItem.name = promoItem.name;
          newPromoItem.setId = promoItem.setId;
          newPromoItem.gratis = promoItem.gratis;
          newPromoItem.price = promoItem.price;
          newPromoItem.quantity = promoItem.quantity;
          newPromoItem.discountPercent = promoItem.discountPercent;
          newPromoItem.minimumPrice = promoItem.minimumPrice;
          newPromoItem.discountSetId = promoItem.discountSetId;
          newPromoItem.products = promoItem.products;
          return newPromoItem;
        });
      }
    });
  }

  cartChangedEvent() {
    this._isReady = true;
    this.promoSet.promoItemsEdit.forEach(promoItem => {
      console.log(promoItem.isReady());
      this._isReady = this._isReady && promoItem.isReady();
    });
  }

  addPromoSetToCart() {
    this.cartService.addPromoSetToCart(this.promoSet).subscribe(x => {  
    });
  }
  

}
