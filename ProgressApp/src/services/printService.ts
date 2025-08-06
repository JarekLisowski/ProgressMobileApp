import { inject, Injectable } from "@angular/core";
import { ApiService } from "./api.service";
import { Observable, tap } from "rxjs";
import { PrintRequestResponse } from "../domain/generated/apimodel";
import { LoggerService } from "./loggerService";

@Injectable({
    providedIn: 'root'
})
export class PrintService {

    private apiService = inject(ApiService);
    private loggerService = inject(LoggerService);

    printInvoice(id: number): Observable<PrintRequestResponse> {
        return this.apiService.printInvoiceRequest(id).pipe(
            tap({
                next: x => {
                    if (!x.isError && x.data) {
                        window.open(`https://progress.ifox.com.pl/invoice/${x.data}`, '_blank');
                    } else {
                        console.error(x.message);
                        this.loggerService.showError((x.message ?? "Nieznany błąd drukowania.") + `. Invoice ID: ${id} `);
                    }
                },
                error: err => {
                    console.error(err);
                    this.loggerService.showError((err ?? "Nieznany błąd drukowania.") + `. Invoice ID: ${id} `);
                }
            })
        )
    }

    printCashReceipt(id: number): Observable<PrintRequestResponse> {
        return this.apiService.printCashReceiptRequest(id).pipe(
            tap({
                next: x => {
                    if (!x.isError && x.data) {
                        window.open(`https://progress.ifox.com.pl/cashReceipt/${x.data}`, '_blank');
                    } else {
                        this.loggerService.showError((x.message ?? "Nieznany błąd drukowania.") + `. KP ID: ${id} `);
                    }
                },
                error: err => {
                    console.error(err);
                    this.loggerService.showError((err ?? "Nieznany błąd drukowania.") + `. Invoice ID: ${id} `);
                }
            })
        );
    }
}