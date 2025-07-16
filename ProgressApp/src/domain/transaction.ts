import { Customer } from "./generated/apimodel";

export class Transaction {
    customer: Customer | undefined;
    document: string = '';
    secondPaymentMethod: number | undefined;
    deliveryMethod: number | undefined;
    comment: string = '';
    cashAmount: number = 0;
    secondMethodAmount: number = 0;    
    packagesNumber: number = 0;
    itemsNet: number = 0;
    itemsGross: number = 0;
    totalNet: number = 0;
    totalGross: number = 0;
    deliveryNet: number = 0;
    deliveryGross: number = 0;
    paymentDueDays: number = 14;
    deliveryServiceId: number | undefined;;
}