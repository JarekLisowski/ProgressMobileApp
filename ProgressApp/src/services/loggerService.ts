import { Injectable } from "@angular/core";
import { AppToastComponent } from "../app/app-toast/app-toast.component";

@Injectable({
    providedIn: 'root'
})
export class LoggerService {
    
    set toast(toast: AppToastComponent) {
        this._appToast = toast;
    }

    private _appToast: AppToastComponent | undefined;

    log(message: string): void {
        console.log(message);        
    }

    showError(message: string): void {
        console.error(message);
        this.showToast("Błąd", message, false, true);
    }

    showWarn(message: string): void {
        console.warn(message);
        this.showToast("Uwaga", message, true);
    }

    showInfo(message: string): void {
        console.info(message);
        this.showToast("Informacja", message);
    }

    showToast(caption: string, message: string, isWarning: boolean = false, isError: boolean = false) {
        if (this._appToast != undefined) {
            this._appToast.showInfo(caption, message, isWarning, isError);
        }
    }



}