import { Component, ElementRef, ViewChild, Input, inject } from '@angular/core';
import { Modal } from 'bootstrap';
import { QuantityComponent } from '../quantity/quantity.component';
import { CartService } from '../../services/cart.service';
import { IProduct, Product } from '../../domain/generated/apimodel';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-add-product-window',
  imports: [QuantityComponent],
  templateUrl: './add-product-window.component.html',
  styleUrl: './add-product-window.component.scss'
})
export class AddProductWindowComponent {

  @ViewChild('addProductModal') modalRef!: ElementRef;
  
  productName: string = "";

  product: Product | null = null;

  private quantityMax: number = 999;

  private readonly cartService = inject(CartService);
  private readonly apiService = inject(ApiService);
  
  modal: Modal | undefined;
  quantity: number = 1;

  show(productId: number) {
    this.loadProduct(productId);    
    if (this.modal === undefined) {
      this.modal = new Modal(this.modalRef.nativeElement);
    }
    this.modal.show();
  }

  hide() {
    this.modal?.hide();
  }

  addToCart() {
    if (this.product === null) {
      console.error('Product is null. Cannot add to cart.');
      return;
    }
    this.cartService.addItemToCart(this.product, this.quantity, this.quantityMax).subscribe(x => {
      console.log('Added to cart: ');
      console.dir(x);
      this.hide();
    });
  }

  quantityChanged(value: number) {
    this.quantity = value;
  }

  loadProduct(productId: number) {
    this.apiService.getProduct(productId).subscribe(product => {
      if (product != null && product.data != undefined) {
        this.product = product.data;        
      }
    });
  }
}

