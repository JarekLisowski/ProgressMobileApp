export class ProductStockInfo {
    productId: number;
    quantity: number | undefined;
    stock: number | undefined;

    get ouOfStock(): boolean {
        if (this.stock == undefined || this.quantity == undefined)
            return false;
        return this.quantity > this.stock;
    }

    constructor(productId: number, quantity: number | undefined, stock: number | undefined) {
        this.productId = productId;
        this.quantity = quantity;
        this.stock = stock;
    }
}