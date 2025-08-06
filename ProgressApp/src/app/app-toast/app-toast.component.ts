import { NgClass } from '@angular/common';
import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import bootstrap, { Toast } from 'bootstrap';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [NgClass],
  templateUrl: './app-toast.component.html',
  styleUrl: './app-toast.component.scss'
})
export class AppToastComponent implements AfterViewInit {

  @ViewChild('toastElement') toastElement: ElementRef | undefined;
  toastBootstrap: Toast | undefined;

  caption = "Błąd";
  message = "-";

  isWarning = false;
  isError = false;

  ngAfterViewInit(): void {
    if (this.toastElement != undefined) {
      this.toastBootstrap = Toast.getOrCreateInstance(this.toastElement.nativeElement);      
    }
  }

  showInfo(caption: string, message: string, isWarning: boolean = false, isError: boolean = false) {
    this.isError = isError;
    this.isWarning = isWarning;
    this.caption = caption;
    this.message = message;
    if (this.toastBootstrap != undefined) {
      this.toastBootstrap.show()
    }
  }

}
