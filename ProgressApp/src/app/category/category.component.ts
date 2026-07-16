import { Component, inject, OnInit } from '@angular/core';
import { ProductGrid5Component } from "../product-grid-5/product-grid-5.component";
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Product } from '../../domain/generated/apimodel';
import { FormsModule } from '@angular/forms';
import { ProductListComponent } from "../product-list/product-list.component";
import { AppConfigService } from '../../services/app-config.service';

@Component({
  selector: 'app-category',
  imports: [ProductGrid5Component, FormsModule, ProductListComponent],
  templateUrl: './category.component.html',
  styleUrl: './category.component.scss'
})
export class CategoryComponent implements OnInit {

  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);
  private readonly appSettings = inject(AppConfigService);

  viewStyle = 0;

  products: Product[] = [];

  private _onlyAvailable: boolean = true;

  set onlyAvailable(value: boolean) {
    this._onlyAvailable = value;
    this.loadCategory();
  }

  get onlyAvailable(): boolean {
    return this._onlyAvailable;
  }


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
    return this._categoryId;
  }

  ngOnInit(): void {
    this.viewStyle = this.appSettings.viewStyle;
    this.route.params.subscribe(
      x => {
        this.categoryId = Number(x['name']);
      }
    );
  }

  loadCategory() {
    this.api.getProductList(this.categoryId, this.onlyAvailable).subscribe(x => {
      console.log(x);
      if (x.isError == false && x.data != null) {
        this.products = x.data;
        this.products.forEach(product => {
          product.imgUrl = this.api.makeUrlImage(product.id!, 0);
        });
      }
    });
    this.api.getCategoryInfo(this.categoryId).subscribe(x => {
      if (x.isError == false && x.data != null) {
        this._categoryName = x.data.name ?? "";
      }
    }
    );
  }

  setViewStyle(style: number) {
    this.viewStyle = style;
    this.appSettings.viewStyle = style;
  }
}
