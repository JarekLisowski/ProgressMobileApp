export class CartItem {
    productId: number = 0;
    name?: string = "";
    code?: string = "";
    priceNet: number = 0;
    priceGross: number = 0; 
    taxRate: number = 23;
    quantity: number = 0;
    promoSetId: number = 0;
    promoItemId: number = 0;
    imageUrl: string = "";
    sumNetto: number = 0;
    sumGross: number = 0;
    stock: number | undefined;
}

export class CartItemWithId extends CartItem {
    id: number = 0;
}