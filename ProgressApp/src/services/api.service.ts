import { Injectable } from "@angular/core";
import { RESTClientService } from "./RESTClient.service";
import { Observable } from "rxjs";
import { IProductListRequest, ProductCategoryInfoResponse, ProductCategoryListResponse, ProductListRequest, ProductListResponse, ProductResponse, PromoSetListResponse } from "../domain/generated/apimodel";

@Injectable({
    providedIn: 'root'
  })
  
  export class ApiService {
    constructor(private apiSerivce: RESTClientService) {
    }
    
    baseUrl() {
      return this.apiSerivce.baseUrl();
    }
    
    getProductList(categoryId: number) : Observable<ProductListResponse>{
      var request = {
        categoryId: categoryId
      };
      return this.apiSerivce.post<ProductListResponse>("api/product/list", request);
    }
    
    getProduct(productId: number) : Observable<ProductResponse>{
      var request = {
        productId: productId
      };
      return this.apiSerivce.post<ProductResponse>("api/product/details", request);
    }
    
    getCategoryList(): Observable<ProductCategoryListResponse>{
      return this.apiSerivce.post<ProductCategoryListResponse>("api/product/category/list", {});
    }
  
    getCategoryInfo(id: number): Observable<ProductCategoryInfoResponse>{
      return this.apiSerivce.post<ProductCategoryInfoResponse>(`api/product/category/info/${id}`, {});
    }

    getPromoList(): Observable<PromoSetListResponse>{
      return this.apiSerivce.post<PromoSetListResponse>("api/promo/list", {});
    }
}