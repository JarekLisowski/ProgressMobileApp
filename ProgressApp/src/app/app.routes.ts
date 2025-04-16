import { Routes } from '@angular/router';
import { CategoryComponent } from './category/category.component';
import { CartComponent } from './cart/cart.component';
import { SpecialOfferComponent } from './special-offer/special-offer.component';
import { ProductDetailsComponent } from './product-details/product-details.component';
import { CategoriesComponent } from './categories/categories.component';

export const routes: Routes = [
    { path: 'category/:name', component: CategoryComponent },
    { path: 'product/:name', component: ProductDetailsComponent },
    { path: 'cart', component: CartComponent },
    { path: 'special-offer', component: SpecialOfferComponent },
    { path: 'categories', component: CategoriesComponent }
];
