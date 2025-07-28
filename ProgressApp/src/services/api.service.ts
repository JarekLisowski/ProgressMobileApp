import { Injectable } from "@angular/core";
import { RESTClientService } from "./RESTClient.service";
import { Observable } from "rxjs";
import { ApiResult, Customer, CustomerListResponse, CustomerResponse, DeliveryMethodsResponse, DocumentResponse, IDocument, IPayment, IProductListRequest, LoginResponse, Payment, PaymentMethodsResponse, Product, ProductCategoryInfoResponse, ProductCategoryListResponse, ProductListRequest, ProductListResponse, ProductResponse, PromoResponse, PromoSetListResponse, PromoSetResponse, SearchResponse, StringApiResult } from "../domain/generated/apimodel";

@Injectable({
  providedIn: 'root'
})

export class ApiService {

  constructor(private apiSerivce: RESTClientService) {
  }

  baseUrl() {
    return this.apiSerivce.baseUrl();
  }

  getProductList(categoryId: number): Observable<ProductListResponse> {
    var request = {
      categoryId: categoryId
    };
    return this.apiSerivce.post<ProductListResponse>("api/product/list", request);
  }

  getProduct(productId: number): Observable<ProductResponse> {
    var request = {
      productId: productId
    };
    return this.apiSerivce.post<ProductResponse>("api/product/details", request);
  }

  getCategoryList(): Observable<ProductCategoryListResponse> {
    return this.apiSerivce.post<ProductCategoryListResponse>("api/product/category/list", {});
  }

  getCategoryInfo(id: number): Observable<ProductCategoryInfoResponse> {
    return this.apiSerivce.post<ProductCategoryInfoResponse>(`api/product/category/info/${id}`, {});
  }

  getPromoList(): Observable<PromoSetListResponse> {
    return this.apiSerivce.post<PromoSetListResponse>("api/promo/list", {});
  }

  getPromoSet(id: number): Observable<PromoSetResponse> {
    return this.apiSerivce.post<PromoSetResponse>(`api/promo/${id}`, {});
  }

  getPromoProductsForPromoItem(id: number): Observable<ProductListResponse> {
    return this.apiSerivce.post<ProductListResponse>(`api/promo/productsForPromoItem/${id}`, {});
  }

  getCustomerList(pattern: string): Observable<CustomerListResponse> {
    return this.apiSerivce.get<CustomerListResponse>(`api/customer/search/${pattern}`);
  }

  getPaymentMethods(): Observable<PaymentMethodsResponse> {
    return this.apiSerivce.get<PaymentMethodsResponse>(`api/configuration/paymentMethods`);
  }

  getDeliveryMethods(): Observable<DeliveryMethodsResponse> {
    return this.apiSerivce.get<DeliveryMethodsResponse>(`api/configuration/deliveryMethods`);
  }

  sendDocument(document: IDocument):Observable<any> {
    return this.apiSerivce.post('api/document/invoice', document)
  }
  
  getInvoices(customerId: number):Observable<DocumentResponse> {
    return this.apiSerivce.get(`api/document/invoices/${customerId}`)
  }

  getInvoice(id: number):Observable<DocumentResponse> {
    return this.apiSerivce.get(`api/document/invoice/${id}`)
  }

  getCustomer(id: number): Observable<CustomerResponse> {
    return this.apiSerivce.get<CustomerResponse>(`api/customer/${id}`);
  }

  addOrUpdateCustomer(customer: Customer): Observable<StringApiResult> {
    return this.apiSerivce.post<StringApiResult>(`api/customer/update`, customer);
  }

  payForInvoice(payment: IPayment): Observable<ApiResult> {
    return this.apiSerivce.post<ApiResult>(`api/document/pay/`, payment);
  }









  makeUrlImage(productId: number, number: number) {
    if (number == undefined) {
      number = 0;
    }
    return `${this.baseUrl()}api/product/image/${productId}?number=${number}`;
  }

  makeUrlPromoImage(id: number | undefined): string {
    return `${this.baseUrl()}api/promo/image/${id}`;
  }

  login(username: string, password: string): Observable<LoginResponse> {
    var request = {
      username: username,
      password: password
    };
    return this.apiSerivce.post<LoginResponse>('api/auth/login', request);
  }

  search(searchText: string): Observable<SearchResponse> {
    return this.apiSerivce.get<SearchResponse>(`api/product/search?searchtext=${searchText}`);
  }

}