import { AfterViewInit, Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { SlickCarouselModule } from 'ngx-slick-carousel';
import { ApiService } from '../../services/api.service';
import { Product } from '../../domain/generated/apimodel';
import { CartService } from '../../services/cart-service';
import { AddedToCartModalComponent } from "../added-to-cart-modal/added-to-cart-modal.component";

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [SlickCarouselModule, FormsModule, AddedToCartModalComponent],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent implements OnInit {
  
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);
  private readonly cartService = inject(CartService);

  product: Product | null = null;

  _productId: number = 0;

  slideConfig = {"slidesToShow": 1, "slidesToScroll": 1};

  slides: any[] = [];

  quantity: number = 1;

  set productId(id : number) {
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
      this.api.getProduct(this._productId).subscribe(product => {
        if (product != null && product.data != undefined)
        {
          this.product = product.data;
          this.slides = [];
          var n = this.product.imagesCount ?? 1;
          for (let i = 0; i < n; i++)
            this.slides.push({img: 'http://localhost:5085/api/product/image/'+ this.product.id +'?number='+i})
          console.log('loaded: ' + this.product.name);
          console.log(this.slides);
        }
      });
  }

  public amountIncrement() {
    console.log('incrementing: ' + this.quantity);
    this.quantity += 1;
  }

  public amountDecrement() {
    if (this.quantity > 1) {
      this.quantity -= 1;
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
//      const myModalAlternative = new bootstrap.Modal('#myModal', options)
    });
  }
}
