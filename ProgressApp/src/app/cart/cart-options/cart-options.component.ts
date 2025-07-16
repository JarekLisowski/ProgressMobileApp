import { Component, inject, OnInit } from '@angular/core';
import { CartService } from '../../../services/cart.service';
import { ApiService } from '../../../services/api.service';
import { DeliveryMethod } from '../../../domain/generated/apimodel';
import { NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'cart-options',
  standalone: true,
  imports: [NgFor, FormsModule],
  templateUrl: './cart-options.component.html',
  styleUrl: './cart-options.component.scss'
})
export class CartOptionsComponent implements OnInit {

  private readonly cartService = inject(CartService);
  private readonly apiService = inject(ApiService);

  private _selectedDocument: string = "";
  private _selectedPayment: number | undefined;
  private _secondPaymentAmount: number = 0;
  private _cashAmount: number = 0;
  private _selectedDelivery: number | undefined;
  private _packages: number = 0;
  private _comment: string = "";

  set selectedDocument(value: string) {
    this.cartService.setDocument(value).subscribe(x => {
      this._selectedDocument = x.document;
    });
  }

  get selectedDocument(): string {
    return this._selectedDocument;
  }

  set selectedPayment(value: string) {
    var nValue = Number(value);
    this.cartService.setPayment(nValue).subscribe(x => {
      this._selectedPayment = x.secondPaymentMethod;
    });
  }

  get selectedPayment(): string {
    return this._selectedPayment?.toString() ?? "";
  }

  set secondPaymentAmount(value: number) {
    this.cartService.setPaymentValues(this.cashAmount, value).subscribe(x => {
      this._secondPaymentAmount = x.secondMethodAmount;
    });
  }

  get secondPaymentAmount(): number {
    return this._secondPaymentAmount;
  }

  set selectedDelivery(value: string) {
    var nValue = Number(value);
    this.cartService.setDelivery(nValue).subscribe(x => {
      this._selectedDelivery = x.deliveryMethod;
    });
  }

  get selectedDelivery(): string {
    return this._selectedDelivery?.toString() ?? "";
  }

  set packages(value: number) {
    this.cartService.setPackages(value).subscribe(x => {
      this._packages = x.packagesNumber;
    });
  }

  get packages(): number {
    return this._packages;
  }

  set cashAmount(value: number) {
    this.cartService.setPaymentValues(value, this.secondPaymentAmount).subscribe(x => {
      this._cashAmount = x.cashAmount;
    });
  }

  get cashAmount(): number {
    return this._cashAmount;
  }

  set comment(value: string) {
    this.cartService.setComment(value).subscribe(x => {
      this._comment = x.comment;
    });;
  }

  get comment(): string {
    return this._comment;
  }


  paymentMethods: DeliveryMethod[] = [];

  deliveryMethods: DeliveryMethod[] = [];


  ngOnInit(): void {
    this.apiService.getPaymentMethods().subscribe(x => {
      if (!x.isError && x?.data != null)
        this.paymentMethods = x.data
    });

    this.apiService.getDeliveryMethods().subscribe(x => {
      if (!x.isError && x?.data != null)
        this.deliveryMethods = x.data
    });

    this.cartService.getCurrentTransaction().subscribe(x => {
      this.selectedDocument = x.document;
      this.selectedPayment = x.secondPaymentMethod?.toString() ?? "";
      this.secondPaymentAmount = x.secondMethodAmount;
      this.cashAmount = x.cashAmount;
      this.selectedDelivery = x.deliveryMethod?.toString() ?? "";
      this.comment = x.comment;
      this.packages = x.packagesNumber;
    });
  }



}
