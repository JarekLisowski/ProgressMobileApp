import { Component, ElementRef, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Modal } from 'bootstrap';

@Component({
  selector: 'product-added-window',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './product-added-window.component.html',
  styleUrl: './product-added-window.component.scss'
})
export class ProductAddedWindowComponent {

  @ViewChild('productAddedModal') modalRef!: ElementRef;

  modal: Modal | undefined;

  show(info: string | undefined) {
    if (this.modal === undefined)
      this.modal = new Modal(this.modalRef.nativeElement);
    this.modal!.show();
    console.log('showing');
  }

  hide() {
    this.modal?.hide();
  }

}
