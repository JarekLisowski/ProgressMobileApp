import { Injectable } from "@angular/core";
import { RESTClientService } from "./RESTClient.service";
import { Observable } from "rxjs";
import { ApiResult, Customer, CustomerListResponse, CustomerResponse, DeliveryMethodsResponse, DocumentResponse, IDocument, IPayment, IProductListRequest, IProductStocksRequest, LoginResponse, Payment, PaymentMethodsResponse, PrintRequestResponse, Product, ProductCategoryInfoResponse, ProductCategoryListResponse, ProductListRequest, ProductListResponse, ProductResponse, ProductsStockResponse, ProductStock, PromoResponse, PromoSetListResponse, PromoSetResponse, SaleSummaryResponse, SaveDocumentResponse, SearchResponse, StringApiResult } from "../domain/generated/apimodel";

@Injectable({
  providedIn: 'root'
})

export class ApiService {


  constructor(private apiSerivce: RESTClientService) {
  }

  baseUrl() {
    return this.apiSerivce.baseUrl();
  }

  getProductList(categoryId: number, onlyAvailable: boolean): Observable<ProductListResponse> {
    var request = {
      categoryId: categoryId,
      onlyAvailable: onlyAvailable
    };
    return this.apiSerivce.post<ProductListResponse>("api/product/list", request);
  }

  getProductListByBrand(brandId: number, onlyAvailable: boolean): Observable<ProductListResponse> {
    var request = {
      brandId: brandId,
      onlyAvailable: onlyAvailable
    };
    return this.apiSerivce.post<ProductListResponse>("api/product/listByBrand", request);
  }

  getStocksForProducts(itemIds: number[]): Observable<ProductsStockResponse> {
    var request: IProductStocksRequest = {
      productIds: itemIds
    };
    return this.apiSerivce.post<ProductsStockResponse>("api/product/stocks", request);
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

  getBrandList(): Observable<ProductCategoryListResponse> {
    return this.apiSerivce.post<ProductCategoryListResponse>("api/product/brand/list", {});
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

  sendDocument(document: IDocument): Observable<SaveDocumentResponse> {
    return this.apiSerivce.post('api/document/send', document)
  }

  getInvoices(customerId: number): Observable<DocumentResponse> {
    return this.apiSerivce.get(`api/document/invoices/${customerId}`)
  }

  getInvoicesOwnCustomers(customerId: number): Observable<DocumentResponse> {
    return this.apiSerivce.get(`api/document/my-invoices/${customerId}`)
  }

  getOrders(customerId: number): Observable<DocumentResponse> {
    return this.apiSerivce.get(`api/document/orders/${customerId}`)
  }

  getOrdersOwnCustomers(customerId: number): Observable<DocumentResponse> {
    return this.apiSerivce.get(`api/document/my-orders/${customerId}`)
  }

  getDocument(id: number): Observable<DocumentResponse> {
    return this.apiSerivce.get(`api/document/document/${id}`)
  }

  getCustomer(id: number): Observable<CustomerResponse> {
    return this.apiSerivce.get<CustomerResponse>(`api/customer/${id}`);
  }

  getSaleSummary(dateFrom: string, dateTo: string): Observable<SaleSummaryResponse> {
    return this.apiSerivce.get<SaleSummaryResponse>(`api/document/sale-summary?from=${dateFrom}&to=${dateTo}`);
  }

  addOrUpdateCustomer(customer: Customer): Observable<StringApiResult> {
    return this.apiSerivce.post<StringApiResult>(`api/customer/update`, customer);
  }

  payForInvoice(payment: IPayment): Observable<ApiResult> {
    return this.apiSerivce.post<ApiResult>(`api/document/pay/`, payment);
  }

  printInvoiceRequest(id: number): Observable<PrintRequestResponse> {
    return this.apiSerivce.post<PrintRequestResponse>(`api/print/request/invoice/${id}`, {});
  }

  printCashReceiptRequest(id: number): Observable<PrintRequestResponse> {
    return this.apiSerivce.post<PrintRequestResponse>(`api/print/request/cashReceipt/${id}`, {});
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