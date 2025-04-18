import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { IProduct, Product, PromoSet } from '../../domain/generated/apimodel';
import { ProductGrid5Component } from "../product-grid-5/product-grid-5.component";

@Component({
  selector: 'app-special-offers',
  standalone: true,
  imports: [ProductGrid5Component],
  templateUrl: './special-offers.component.html',
  styleUrl: './special-offers.component.scss'
})
export class SpecialOffersComponent implements OnInit {

  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);
  
  promoList: PromoSet[] = [];

  products: IProduct[] = [];

  ngOnInit(): void {
    this.api.getPromoList().subscribe(
      x => {
        console.log(x);
        if (x.isError == false && x.data != null)
        {
          this.promoList = x.data;
          console.log('Promo loaded');
          this.products = this.promoList.map(p => {
            return {
              id: p.id,
              name: p.name,
              description: p.name,
              imgUrl: p.imgUrl,
            }
          });
          console.log(this.products);
        }
      });
  }

}
