import { Component, inject, OnInit } from '@angular/core';
import { ProductGrid5Component } from "../product-grid-5/product-grid-5.component";
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Product } from '../../domain/generated/apimodel';
import { ApiService } from '../../services/api.service';
import { FormsModule } from '@angular/forms';
import { forkJoin, take } from 'rxjs';

@Component({
  selector: 'app-brand',
  standalone: true,
  imports: [ProductGrid5Component, FormsModule, RouterLink],
  templateUrl: './brand.component.html',
  styleUrl: './brand.component.scss'
})
export class BrandComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);

  products: Product[] = [];
  groupId: number | null = null;

  categories = new Array<{ id: number, name: string }>();

  private _categoryName: string = "";

  set categoryName(name: string) {
    this._categoryName = name;
    this.loadCategory();
  }

  get categoryName() {
    return this._categoryName;
  }

  _categoryId: number = 0;

  set categoryId(categoryId: number) {
    this._categoryId = categoryId;
    this.loadCategory();
  }

  get categoryId() {
    return  this._categoryId;
  }

  private _onlyAvailable: boolean = true;
  
  set onlyAvailable(value: boolean) {
    this._onlyAvailable = value;
    this.loadCategory();
  }

  get onlyAvailable(): boolean {
    return this._onlyAvailable;
  }

  hasCategory = false;
  hasGroup = false;

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.categoryId = Number(params['name']);
      this.hasCategory = true;
      this.loadCategory();
    });
    this.route.queryParams.subscribe(queryParams => {      
      this.groupId = Number(queryParams['group']);
      this.hasGroup = true;
      this.loadCategory();
    });
  }

  loadCategory() {
    if (!this.hasCategory && !this.hasGroup)
      return;
    this.api.getProductListByBrand(this.categoryId, this.groupId, this.onlyAvailable).subscribe(x => {
      if (x.isError == false && x.data != null)
      {
        this.products = x.data;
        this.products.forEach(product => {
          product.imgUrl = this.api.makeUrlImage(product.id!, 0);
        });
      }
    });
    this.api.getBrandInfo(this.categoryId).subscribe(x => {
      if (x.isError == false && x.data != null)
      {
        this._categoryName = x.data.name ?? "";
      }
    }
    );
    this.api.getBrandCategories(this.categoryId).subscribe(x => {
      if (x.isError == false && x.data != null)
      {
        this.categories = x.data.map(cat => {
          return { id: cat.id!, name: cat.name! };
        });
      }
    });
  }
}
