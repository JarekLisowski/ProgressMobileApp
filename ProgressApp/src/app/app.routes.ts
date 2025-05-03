import { Routes } from '@angular/router';
import { CategoryComponent } from './category/category.component';
import { CartComponent } from './cart/cart.component';
import { SpecialOfferComponent } from './special-offer/special-offer.component';
import { ProductDetailsComponent } from './product-details/product-details.component';
import { CategoriesComponent } from './categories/categories.component';
import { SpecialOffersComponent } from './special-offers/special-offers.component';
import { SpecialOfferEditComponent } from './special-offer-edit/special-offer-edit.component';

export const routes: Routes = [
    { path: 'category/:name', component: CategoryComponent },
    { path: 'product/:name', component: ProductDetailsComponent },
    { path: 'cart', component: CartComponent },
    { path: 'special-offer', component: SpecialOfferComponent },
    { path: 'special-offers', component: SpecialOffersComponent },
    { path: 'specialOfferEdit/:id', component: SpecialOfferEditComponent },
    { path: 'categories', component: CategoriesComponent }
];
