import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Document } from '../../domain/generated/apimodel';
import { NgFor } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'documents',
  standalone: true,
  imports: [NgFor, RouterModule],
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
