import { SpecialOfferEdit } from "./specialOfferEdit";

export class CartPromoItem {

    static fromSpecialOfferEdit(specialOfferEdit: SpecialOfferEdit) {
        var result : CartPromoItem = new CartPromoItem();
        result.promoSetId = specialOfferEdit.promoSetId;
        result.priceNet = specialOfferEdit.promoItemsEdit.reduce((acc, item) => acc + (item.sumNet()), 0);
        result.priceGross = specialOfferEdit.promoItemsEdit.reduce((acc, item) => acc + (item.sumGross()), 0);
        return result;
    }

    promoSetId: number = 0;
    priceNet: number = 0;
    priceGross: number = 0;     
}

export class CartPromoItemWithIt extends CartPromoItem {
    id: number = 0;
}