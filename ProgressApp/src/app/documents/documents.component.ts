import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Document } from '../../domain/generated/apimodel';
import { CommonModule, NgFor } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'documents',
    imports: [NgFor, RouterModule, CommonModule],
    templateUrl: './documents.component.html',
    styleUrl: './documents.component.scss'
})
export class DocumentsComponent {
  @Input()
  data: Document[] = [];

  @Output()
  documentSelected: EventEmitter<number> = new EventEmitter<number>();

  selectDocument(document: Document) {
    this.documentSelected.emit(document.id);
  }
}
