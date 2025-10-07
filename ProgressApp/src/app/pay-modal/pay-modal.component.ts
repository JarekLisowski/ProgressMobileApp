import { AfterViewInit, Component, ElementRef, EventEmitter, inject, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { Modal } from 'bootstrap';
import { Observable, Subject, tap } from 'rxjs';
import { Document, IPayment, Payment, PaymentMethod } from '../../domain/generated/apimodel';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { CommonModule, NgFor } from '@angular/common';

@Component({
    selector: 'pay-modal',
    imports: [NgFor, FormsModule, CommonModule],
    templateUrl: './pay-modal.component.html',
    styleUrl: './pay-modal.component.scss'
})
export class PayModalComponent implements OnInit, AfterViewInit, OnDestroy {
  
  @ViewChild('modalWindow') modalRef!: ElementRef;

  @Input() set document(value: Document | undefined) {
    this._document = value;
    if (this._document && this._document.paymentToBeSettled) {
      this.payValue = this._document.paymentToBeSettled;
    }
  }

  get document(): Document | undefined {
    return this._document;
  }

  @Output() onAccept: EventEmitter<any> = new EventEmitter();
  @Output() onReject: EventEmitter<any> = new EventEmitter();

  private subject: Subject<any> | null = null;
  private _document: Document | undefined;

  payValue: number = 0;
  
  paymentMethods: PaymentMethod[] = [];
  private _selectedPayment: number | undefined;
  
  selectedPaymentMethod: PaymentMethod | undefined;
  
  constructor(private apiService: ApiService) { 
  }

  ngOnInit(): void {
    this.apiService.getPaymentMethods().subscribe(x => {
      if (!x.isError && x?.data != null)
        this.paymentMethods = x.data
    });
  }

  set selectedPayment(value: string) {
    var nValue = Number(value);
    var paymentMethod = this.paymentMethods.find(x => x.id == nValue);
    if (paymentMethod != null)
      this.selectedPaymentMethod = paymentMethod;
    console.log("selectedPayment: " + value)
  }

  get selectedPayment(): string {
    return this._selectedPayment?.toString() ?? "0";
  }

  clickAccept() {
    if (this.document) {

      var payment: IPayment = {
        relatedDocumentId: this.document.id,
        relatedDocumentNumber: this.document.number,
        issueDate: new Date(),
        value: this.payValue,
        paymentType: this.selectedPaymentMethod?.id ?? 0
      }
      this.subject?.next(payment);
    }

    this.onAccept.emit();
    this.subject?.complete();
    this.hide();
  }

  clickReject() {
    this.onReject.emit();
    this.hide();
    this.subject?.complete();
  }
  modal: Modal | undefined;

  ngAfterViewInit(): void {
    this.modal = new Modal(this.modalRef.nativeElement);
    this.modalRef.nativeElement.addEventListener("hidden.bs.modal", () => {
      this.subject?.complete();
    });
  }

  show() {
    this.modal!.show();
    console.log('showing');
  }

  showObservable(): Observable<Payment> {
    this.show();
    console.log('showing');
    this.subject = new Subject<Payment>();
    var observable = this.subject.pipe(
      tap(x => {
        console.log(x);
      })
    );
    return observable;
  }

  ngOnDestroy(): void {
    console.log('destroying');
    this.subject?.complete();
  }

  hide() {
    this.modal?.hide();
  }
}
