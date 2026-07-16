import { Component, Input, ViewChild } from '@angular/core';
import { IProduct } from '../../../domain/generated/apimodel';
import { DecimalPipe, NgClass } from '@angular/common';
import { AddProductWindowComponent } from '../../add-product-window/add-product-window.component';

@Component({
    selector: 'app-product-5',
    standalone: true,
    imports: [DecimalPipe, NgClass, AddProductWindowComponent],
    templateUrl: './product-5.component.html',
    styleUrl: './product-5.component.scss'
})
export class Product5Component {

  @ViewChild('addWindow') addWindow!: AddProductWindowComponent;

  addToCart() {
    if (this.data) {
      this.addWindow.show(this.data.id!);
    }
  }
  
  @Input() data: IProduct | null = null;    
}

