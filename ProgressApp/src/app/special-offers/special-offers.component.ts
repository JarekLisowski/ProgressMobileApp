import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { IProduct, IPromoSet, Product, PromoSet } from '../../domain/generated/apimodel';
import { ProductGrid5Component } from "../product-grid-5/product-grid-5.component";
import { SpecialOfferListComponent } from "../special-offer-list/special-offer-list.component";

@Component({
  selector: 'app-special-offers',
  standalone: true,
  imports: [SpecialOfferListComponent],
  templateUrl: './special-offers.component.html',
  styleUrl: './special-offers.component.scss'
})
export class SpecialOffersComponent implements OnInit {

  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);
  
  promoList: IPromoSet[] = [];

  ngOnInit(): void {
    this.api.getPromoList().subscribe(
      x => {
        console.log(x);
        if (x.isError == false && x.data != null)
        {
          this.promoList = x.data;
        }
      });
  }

}
