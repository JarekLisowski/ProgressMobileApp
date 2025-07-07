import { AfterViewInit, Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { Modal } from 'bootstrap';
import { CustomerListComponent } from "../customer-list/customer-list.component";
import { Customer } from '../../domain/generated/apimodel';

@Component({
  selector: 'customer-select',
  standalone: true,
  imports: [CustomerListComponent],
  templateUrl: './customer-select.component.html',
  styleUrl: './customer-select.component.scss'
})
export class CustomerSelectComponent implements AfterViewInit {
  
  @ViewChild('selectProductModal') myModalRef!: ElementRef;

  @Output() customerSelectedEvent = new EventEmitter<Customer>();
  
  @Input() customer: Customer | undefined;
  
  modal!: Modal;
  
  ngAfterViewInit(): void {
    this.modal = new Modal(this.myModalRef.nativeElement);
    this.myModalRef.nativeElement.addEventListener("hidden.bs.modal", () => {
      
    });
  }
  
  select() {
    this.modal.show();
  }
  
  customerSelected(event: Customer) {
    this.modal.hide();
    this.customer = event;
    this.customerSelectedEvent.emit(this.customer);
  }
}
