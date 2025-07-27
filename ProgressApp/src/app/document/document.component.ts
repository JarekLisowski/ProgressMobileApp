import { Component, Input } from '@angular/core';
import { Document } from '../../domain/generated/apimodel';
import { CommonModule, NgFor } from '@angular/common';

@Component({
  selector: 'document',
  standalone: true,
  imports: [NgFor, CommonModule],
  templateUrl: './document.component.html',
  styleUrl: './document.component.scss'
})
export class DocumentComponent {
  
  @Input() document: Document | undefined;
}
