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

}