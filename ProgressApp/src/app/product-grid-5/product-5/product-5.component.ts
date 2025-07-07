import { Component, Input } from '@angular/core';
import { IProduct } from '../../../domain/generated/apimodel';

@Component({
  selector: 'app-product-5',
  standalone: true,
  imports: [],
  templateUrl: './product-5.component.html',
  styleUrl: './product-5.component.scss'
})
export class Product5Component {
  
  @Input() data: IProduct | null = null;  
}
