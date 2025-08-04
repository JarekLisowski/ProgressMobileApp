import { inject, Injectable } from "@angular/core";
import { ApiService } from "./api.service";

@Injectable({
    providedIn: 'root'
})
export class PrintService {

    private apiService = inject(ApiService);

    printInvoice(id: number) {
        this.apiService.printInvoiceRequest(id).subscribe(x => {
            if (!x.isError && x.data) {
                window.open(`https://progress.ifox.com.pl/invoice/${x.data}`, '_blank');
            }
        });
    }

    printCashReceipt(id: number) {
        this.apiService.printCashReceiptRequest(id).subscribe(x => {
            if (!x.isError && x.data) {
                window.open(`https://progress.ifox.com.pl/cashReceipt/${x.data}`, '_blank');
            }
        });
    }

}