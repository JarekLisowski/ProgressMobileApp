import { Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { PromoSet } from '../../domain/generated/apimodel';
import { NgFor } from '@angular/common';

@Component({
    selector: 'app-special-offer',
    imports: [NgFor],
    templateUrl: './special-offer.component.html',
    styleUrl: './special-offer.component.scss'
})
export class SpecialOfferComponent implements OnInit {

  private readonly api = inject(ApiService);

  promoList: PromoSet[] = [];

  ngOnInit(): void {
    this.api.getPromoList().subscribe(x => {
      if (x != null && !x.isError && x.data != null)
      {
        this.promoList = x.data;
      }
    });
  }




}
