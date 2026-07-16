import { Component, inject, OnInit } from '@angular/core';
import { ProductGrid5Component } from "../product-grid-5/product-grid-5.component";
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Product } from '../../domain/generated/apimodel';
import { ApiService } from '../../services/api.service';
import { FormsModule } from '@angular/forms';
import { ProductListComponent } from '../product-list/product-list.component';
import { AppConfigService } from '../../services/app-config.service';

@Component({
  selector: 'app-brand',
  standalone: true,
  imports: [ProductGrid5Component, FormsModule, RouterLink, ProductListComponent],
  templateUrl: './brand.component.html',
  styleUrl: './brand.component.scss'
})
export class BrandComponent implements OnInit {
  
  
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);
  private readonly appSettings = inject(AppConfigService);

  viewStyle = 0;

  products: Product[] = [];
  groupId: number | null = null;

  categories = new Array<{ id: number, name: string }>();
  categoriesFiltered = new Array<{ id: number, name: string }>();

  private _categoryName: string = "";

  set categoryName(name: string) {
    this._categoryName = name;
    this.loadCategory();
  }

  get categoryName() {
    return this._categoryName;
  }

  get showAllGroupsButton(): boolean {
    return this.categories.length > 5 && this.categoriesFiltered.length < this.categories.length;
  }

  get showAllButton(): boolean {
    return this.groupId != null;
  }

  _categoryId: number = 0;

  set categoryId(categoryId: number) {
    this._categoryId = categoryId;
    this.loadCategory();
  }

  get categoryId() {
    return this._categoryId;
  }

  isGroupSelected(groupId: number) {
    return this.groupId === groupId;
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
    this.viewStyle = this.appSettings.viewStyle;
    this.route.params.subscribe(params => {
      this.categoryId = Number(params['name']);
      this.hasCategory = true;
      this.loadCategory();
    });
    this.route.queryParams.subscribe(queryParams => {
      var groupIdParam = queryParams['group'];
      if (groupIdParam == null || groupIdParam == undefined || groupIdParam == "") {
        this.groupId = null;
      } else {
        this.groupId = Number(groupIdParam);
      }
      this.hasGroup = this.groupId != null;
      this.loadCategory();
    });
  }

  loadCategory() {
    if (!this.hasCategory && !this.hasGroup)
      return;
    this.api.getProductListByBrand(this.categoryId, this.groupId, this.onlyAvailable).subscribe(x => {
      if (x.isError == false && x.data != null) {
        this.products = x.data;
        this.products.forEach(product => {
          product.imgUrl = this.api.makeUrlImage(product.id!, 0);
        });
      }
    });
    this.api.getBrandInfo(this.categoryId).subscribe(x => {
      if (x.isError == false && x.data != null) {
        this._categoryName = x.data.name ?? "";
      }
    }
    );
    this.api.getBrandCategories(this.categoryId).subscribe(x => {
      if (x.isError == false && x.data != null) {
        this.categories = x.data.map(cat => {
          return { id: cat.id!, name: cat.name! };
        });
        if (this.groupId == null || isNaN(this.groupId)) {
          this.categoriesFiltered = this.categories.slice(0, 5);
        } else {
          const index = this.categories.findIndex(cat => cat.id === this.groupId);
          if (index !== -1 && index < 5) {
            this.categoriesFiltered = this.categories.slice(0, 5);
          } else {            
            this.categoriesFiltered = this.categories;
          }
        }
      }
    });
  }

  showAllCategories() {
    this.categoriesFiltered = this.categories;
  }

  setViewStyle(style: number) {
    this.viewStyle = style;
    this.appSettings.viewStyle = style;
  }
}
