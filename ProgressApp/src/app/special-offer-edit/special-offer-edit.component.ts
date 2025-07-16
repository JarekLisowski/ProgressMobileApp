import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { CartService } from '../../services/cart.service';
import { NgFor, NgIf } from '@angular/common';
import { PromoItemEdit, SpecialOfferEdit } from '../../domain/specialOfferEdit';
import { SpecialOfferEditItemComponent } from "./special-offer-edit-item/special-offer-edit-item.component";
import { CartItemWithId } from '../../domain/cartItem';

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

  promoEditId: number = 0;
  promoSetId: number = 0;
  promoSet: SpecialOfferEdit = new SpecialOfferEdit();

  private _isReady: boolean = false;

  get isReady(): boolean {
    return this._isReady;
  }

  isNew: boolean = true;

  isModified: boolean = false;

  ngOnInit(): void {
    var initPromoSet = new SpecialOfferEdit();
    this.promoSetId = this.route.snapshot.params['id'];
    this.promoEditId = this.route.snapshot.params['editId'];
    if (this.promoSetId > 0) {
      // New promo set
      this.isNew = true;
      this.api.getPromoSet(this.promoSetId).subscribe(x => {
        if (x.data != null && x.data.items != null) {
          initPromoSet.name = x.data.name ?? '';
          initPromoSet.promoSetId = x.data.id ?? 0;
          initPromoSet.promoItemsEdit = x.data.items.map(promoItem => {
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
          this.promoSet = initPromoSet;
        }
      });
    }
    else if (this.promoEditId != undefined) {
      // Edit existing promo set
      this.isNew = false;
      this.promoEditId = Number(this.promoEditId);
      this.cartService.getPromoItems().subscribe(x => {
        console.log(x);
      });
      this.cartService.getPromoItem(this.promoEditId).subscribe(x => {
        if (x != null) {
          this.promoSetId = this.promoEditId;
          initPromoSet.id = this.promoEditId;
          initPromoSet.name = x.name;
          initPromoSet.promoSetId = x.promoSetId
          this.api.getPromoSet(initPromoSet.promoSetId).subscribe(x => {
            //load items from api
            if (x.data != null && x.data.items != null) {
              initPromoSet.promoItemsEdit = x.data.items.map(promoItem => {
                var newPromoItem = new PromoItemEdit();
                newPromoItem.id = promoItem.id;
                newPromoItem.name = promoItem.name;
                newPromoItem.setId = this.promoEditId;//promoItem.setId;
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
            this.cartService.getCartItemsForPromoSet(this.promoSetId).subscribe(cartItems => {
              initPromoSet.promoItemsEdit.forEach(promoItem => {
                cartItems.forEach(cartItem => {
                  if (promoItem.id == cartItem.promoItemId) {
                    var cartItemEdit: CartItemWithId = {
                      id: 0,
                      productId: cartItem.productId,
                      name: cartItem.name,
                      code: cartItem.code,
                      priceNet: cartItem.priceNet,
                      priceGross: cartItem.priceGross,
                      taxRate: cartItem.taxRate,
                      quantity: cartItem.quantity,
                      promoSetId: cartItem.promoSetId,
                      promoItemId: cartItem.promoItemId,
                      imageUrl: cartItem.imageUrl
                    };
                    promoItem.cartItems.push(cartItemEdit);
                  }
                });
                console.log("Promoset edit:");
                console.dir(this.promoSet);
                console.log("Is ready: " + promoItem.isReady());
              });
              this.promoSet = initPromoSet;
              this.checkPromosetsCompleteness();
            });
          });
        }
      });
    }
  }

  cartChangedEvent() {
    this.isModified = true;
    this.checkPromosetsCompleteness();
  }

  checkPromosetsCompleteness() {
    this._isReady = true;
    this.promoSet.promoItemsEdit.forEach(promoItem => {
      console.log(promoItem.isReady());
      this._isReady = this._isReady && promoItem.isReady();
    });
  }

  addPromoSetToCart() {
    this.cartService.addPromoSetToCart(this.promoSet).subscribe(x => {
      console.log('promo added, key: ', x);
    });
  }


}
