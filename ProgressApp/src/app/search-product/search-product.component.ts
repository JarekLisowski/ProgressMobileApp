import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-search-product',
  imports: [],
  templateUrl: './search-product.component.html',
  styleUrl: './search-product.component.scss'
})
export class SearchProductComponent implements OnInit {

  productSearch: string | null = null;
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.productSearch = params['search'];
      this.searchProducts()
    });
  }

  searchProducts() {
    this.api.searchProducts(this.productSearch || "", true).subscribe(products => {
      console.log(products);
    });
  }
}
