import { IPrice, IProduct } from "./generated/apimodel";

export class ProductPromoItem implements IProduct {
    id: number | undefined;    
    productId!: number;    
    code?: string | undefined;
    name?: string | undefined;
    description?: string | undefined;
    stock?: number;
    price?: IPrice;
    prices?: { [key: string]: IPrice; } | undefined;
    categoryName?: string | undefined;
    categoryId?: number;
    barCode?: string | undefined;
    imagesCount?: number;
    unit?: string | undefined;
    imgUrl?: string | undefined;
    quantity: number = 0;
    promoItemId: number = 0;
    //maxQuantity: number = 0;
}