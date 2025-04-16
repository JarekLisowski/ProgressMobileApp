import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-product-5',
  standalone: true,
  imports: [],
  templateUrl: './product-5.component.html',
  styleUrl: './product-5.component.scss'
})
export class Product5Component {
  
  @Input() data: any = null;  
}
