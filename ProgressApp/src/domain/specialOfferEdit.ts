import { CartItem, CartItemWithId } from "./cartItem";
import { IPrice, IPromoItem, IPromoItemProduct, IPromoSet, Price } from "./generated/apimodel";
import { v4 as uuidv4 } from 'uuid';

export class SpecialOfferEdit {
    constructor() {
        this.specialOfferSetId = uuidv4();
    }
    id: number | undefined;
    promoSetId: number = 0;
    specialOfferSetId: string;
    name: string = '';
    //promoSet:IPromoSet | null = null;
    //key = promoItemId, values = selected products
    //items: Map<number, CartItemEdit[]> = new Map<number, CartItemEdit[]>();
    totalPrice: Price = new Price();
    promoItemsEdit: PromoItemEdit[] = [];

    getAllCartItems(newPromoSetId: number | undefined = undefined): CartItem[] {
        var res = this.promoItemsEdit.flatMap(x => x.getAllCartItems(newPromoSetId));
        return res;
    }
}

export class PromoItemEdit implements IPromoItem {
    id?: number;
    name?: string | undefined;
    setId?: number;
    gratis?: boolean;
    price?: IPrice;
    quantity?: number;
    discountPercent?: number;
    minimumPrice?: number | undefined;
    discountSetId?: number;
    products?: IPromoItemProduct[] | undefined;
    promoSet?: IPromoSet;
    cartItems: CartItemWithId[] = [];

    isReady(): boolean {
        var count = 0;
        this.cartItems.forEach(item => {
            if (item.quantity != null) {
                count += item.quantity;
            }
        });
        return count == this.quantity;
    }

    sumNet(): number {
        var sum = 0;
        this.cartItems.forEach(item => {
            if (item.priceNet != null) {
                sum += item.priceNet * item.quantity;
            }
        });
        return sum;
    }

    sumGross(): number {
        var sum = 0;
        this.cartItems.forEach(item => {
            if (item.priceGross != null) {
                sum += item.priceGross * item.quantity;
            }
        });
        return sum;
    }

    getAllCartItems(newPromoSetId: number | undefined = undefined): CartItem[] {
        var res = this.cartItems.map(x => {
            var cartItem: CartItem = {
                productId: x.productId,
                name: x.name,
                code: x.code,
                priceNet: x.priceNet,
                priceGross: x.priceGross,
                taxRate: x.taxRate,
                quantity: x.quantity,
                promoSetId: newPromoSetId ?? x.promoSetId,
                promoItemId: x.promoItemId,
                imageUrl: x.imageUrl
            };
            return cartItem;
        });
        return res;
    }




}