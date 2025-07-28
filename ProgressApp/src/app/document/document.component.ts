import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Document } from '../../domain/generated/apimodel';
import { CommonModule, NgFor } from '@angular/common';
import { PayModalComponent } from '../pay-modal/pay-modal.component';

@Component({
  selector: 'document',
  standalone: true,
  imports: [NgFor, CommonModule],
  templateUrl: './document.component.html',
  styleUrl: './document.component.scss'
})
export class DocumentComponent {

  @Input() document: Document | undefined;

  @Output() onPay: EventEmitter<any> = new EventEmitter();

  pay() {
    this.onPay.emit();
  }

}
