import { Component, Input } from '@angular/core';
import { SpecialOfferItemComponent } from "./special-offer-item/special-offer-item.component";
import { NgFor } from '@angular/common';
import { IPromoSet } from '../../domain/generated/apimodel';

@Component({
    selector: 'app-special-offer-list',
    imports: [SpecialOfferItemComponent, NgFor],
    templateUrl: './special-offer-list.component.html',
    styleUrl: './special-offer-list.component.scss'
})
export class SpecialOfferListComponent {
  @Input() items : IPromoSet[] | undefined;
}
