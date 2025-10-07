import { Component, Input } from '@angular/core';
import { RouterModule } from '@angular/router';
import { IPromoSet } from '../../../domain/generated/apimodel';

@Component({
    selector: 'app-special-offer-item',
    imports: [RouterModule],
    templateUrl: './special-offer-item.component.html',
    styleUrl: './special-offer-item.component.scss'
})
export class SpecialOfferItemComponent {
  @Input() data: IPromoSet | null = null;  
}
