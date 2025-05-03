export class CartItem {
    productId: number = 0;
    name?: string = "";
    code?: string = "";
    priceNet: number = 0;
    priceGross: number = 0; 
    quantity: number = 0;
    promoSetId: number = 0;
    promoItemId: number = 0;
}

export class CartItemEdit extends CartItem {
    id: number = 0;
}