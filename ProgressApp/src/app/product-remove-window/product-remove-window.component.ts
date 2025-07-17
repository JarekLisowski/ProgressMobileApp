import { Component, ElementRef, EventEmitter, ViewChild } from '@angular/core';
import { Modal } from 'bootstrap';

@Component({
  selector: 'product-remove-window',
  standalone: true,
  imports: [],
  templateUrl: './product-remove-window.component.html',
  styleUrl: './product-remove-window.component.scss'
})
export class ProductRemoveWindowComponent {

  @ViewChild('productRemoveModal') modalRef!: ElementRef;
  
  confirmRemoveEvent: EventEmitter<any> = new EventEmitter();

  modal: Modal | undefined;

  info: string = "";

  show(info: string | undefined) {
    if (info != undefined)
      this.info = info;
    else
      this.info = "Czy usunąć produkt z koszyka?";
    if (this.modal === undefined)
      this.modal = new Modal(this.modalRef.nativeElement);
    this.modal!.show();
  }

  hide() {
    this.modal?.hide();
  }

  confirm() {
    
  }
}
