import { Component, Input } from '@angular/core';
import { IProduct } from '../../../domain/generated/apimodel';
import { DecimalPipe } from '@angular/common';

@Component({
    selector: 'app-product-5',
    imports: [DecimalPipe],
    templateUrl: './product-5.component.html',
    styleUrl: './product-5.component.scss'
})
export class Product5Component {
  
  @Input() data: IProduct | null = null;  
}
