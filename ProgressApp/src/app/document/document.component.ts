import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { Document } from '../../domain/generated/apimodel';
import { CommonModule, NgFor } from '@angular/common';
import { PayModalComponent } from '../pay-modal/pay-modal.component';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'document',
  standalone: true,
  imports: [NgFor, CommonModule],
  templateUrl: './document.component.html',
  styleUrl: './document.component.scss'
})
export class DocumentComponent {

  @Input() document: Document | undefined;
  @Input() printButton: boolean = true;

  @Output() onPay: EventEmitter<any> = new EventEmitter();
  @Output() onPrint: EventEmitter<any> = new EventEmitter();

  pay() {
    this.onPay.emit();
  }

  print() {
    this.onPrint.emit();    
  }

}
