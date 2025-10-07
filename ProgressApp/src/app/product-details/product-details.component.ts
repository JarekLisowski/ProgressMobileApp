import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { SlickCarouselModule } from 'ngx-slick-carousel';
import { ApiService } from '../../services/api.service';
import { Product } from '../../domain/generated/apimodel';
import { CartService } from '../../services/cart.service';
import { ProductAddedWindowComponent } from "../product-added-window/product-added-window.component";
import { QuantityComponent } from "../quantity/quantity.component";
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-product-details',
    imports: [SlickCarouselModule, FormsModule, ProductAddedWindowComponent, QuantityComponent, CommonModule],
    templateUrl: './product-details.component.html',
    styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent implements OnInit {

  @ViewChild('productAddedWindow') productAddedWindow!: ProductAddedWindowComponent;

  private readonly route = inject(ActivatedRoute);
  private readonly apiService = inject(ApiService);
  private readonly cartService = inject(CartService);

  product: Product | null = null;

  _productId: number = 0;

  slideConfig = { 
    slidesToShow: 1, 
    slidesToScroll: 1,
    listHeight: 200
  };

  slides: any[] = [];

  quantity: number = 1;

  set productId(id: number) {
    this._productId = id;
    this.loadProduct();
  }

  get productId(): number {
    return this._productId;
  }

  ngOnInit(): void {
    this.route.params.subscribe(
      x => {
        this.productId = Number(x['name']);
        console.log('Product: ' + this._productId);
      }
    );
  }

  loadProduct() {
    this.apiService.getProduct(this._productId).subscribe(product => {
      if (product != null && product.data != undefined) {
        this.product = product.data;
        this.slides = [];
        var n = this.product.imagesCount ?? 1;
        for (let i = 0; i < n; i++)
          this.slides.push({ img: this.apiService.makeUrlImage(this.product.id!, i + i) })
        console.log('loaded: ' + this.product.name);
        console.log(this.slides);
      }
    });
  }

  // public amountIncrement() {
  //   console.log('incrementing: ' + this.quantity);
  //   this.quantity += 1;
  // }

  // public amountDecrement() {
  //   if (this.quantity > 1) {
  //     this.quantity -= 1;
  //   }
  // }

  quantityChanged(quantity: number) {
    if (this.quantity >= 0) {
       this.quantity = quantity;
    }
  }

  public addToCart() {
    console.log('Adding to cart: ' + this.product?.name + ' quantity: ' + this.quantity);
    if (this.product == null) {
      console.log('Product is null');
      return;
    }
    this.cartService.addItemToCart(this.product, this.quantity).subscribe(x => {
      console.log('Added to cart: ');
      console.dir(x);
      this.productAddedWindow.show(this.product?.name);
    });
  }
}
