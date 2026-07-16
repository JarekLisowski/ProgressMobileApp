import { Component, ElementRef, EventEmitter, Input, OnDestroy, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Modal } from 'bootstrap';
import { Observable, Subject, tap } from 'rxjs';

@Component({
    selector: 'confirm-modal-window',
    imports: [FormsModule],
    templateUrl: './confirm-modal-window.component.html',
    styleUrl: './confirm-modal-window.component.scss'
})

export class ConfirmModalWindowComponent implements OnDestroy {

  @ViewChild('modalWindow') modalRef!: ElementRef;

  @Input() title: string = '';
  @Input() message: string = '';
  @Input() buttonRejectText: string = 'Anuluj';
  @Input() buttonAcceptText: string = 'Ok';
  @Input() checkbox1Text: string = '';
  @Input() checkbox2Text: string = '';

  @Output() onAccept: EventEmitter<any> = new EventEmitter();
  @Output() onReject: EventEmitter<any> = new EventEmitter();

  public checkBox1Value: boolean = false;
  public checkBox2Value: boolean = false;

  private subject: Subject<any> | null = null;

  clickAccept() {
    this.subject?.next(true);
    this.onAccept.emit();
    this.hide();
  }

  clickReject() {
    this.subject?.next(false);
    this.onReject.emit();
    this.hide();
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

  showObservable(message: string | null): Observable<any> {
    if (message != undefined)
      this.message = message;
    this.show();
    console.log('showing');
    this.subject = new Subject<any>();
    var observable = this.subject.pipe(
      tap(x => {
        console.log(x);
      })
    );
    return observable;
  }

  ngOnDestroy(): void {    
    this.subject?.complete();
  }


  hide() {
    this.modal?.hide();
  }

}


