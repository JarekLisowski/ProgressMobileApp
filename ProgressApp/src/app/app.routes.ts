import { Routes } from '@angular/router';
import { CategoryComponent } from './category/category.component';
import { CartComponent } from './cart/cart.component';
import { SpecialOfferComponent } from './special-offer/special-offer.component';
import { ProductDetailsComponent } from './product-details/product-details.component';
import { CategoriesComponent } from './categories/categories.component';
import { SpecialOffersComponent } from './special-offers/special-offers.component';
import { SpecialOfferEditComponent } from './special-offer-edit/special-offer-edit.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './guards/auth.guard';
import { CustomersComponent } from './customers/customers.component';
import { CustomerComponent } from './customer/customer.component';
import { InvoicesComponent } from './invoices/invoices.component';
import { InvoiceComponent } from './invoice/invoice.component';
import { SaveDocSummaryComponent } from './save-doc-summary/save-doc-summary.component';

export const routes: Routes = [
    { path: 'category/:name', component: CategoryComponent, canActivate: [AuthGuard] },
    { path: 'product/:name', component: ProductDetailsComponent, canActivate: [AuthGuard] },
    { path: 'cart', component: CartComponent, canActivate: [AuthGuard] },
    { path: 'promo', component: SpecialOfferComponent, canActivate: [AuthGuard] },
    { path: 'promos', component: SpecialOffersComponent, canActivate: [AuthGuard] },
    { path: 'promoEdit/:editId', component: SpecialOfferEditComponent, canActivate: [AuthGuard] },
    { path: 'promoCreate/:id', component: SpecialOfferEditComponent, canActivate: [AuthGuard] },
    { path: 'categories', component: CategoriesComponent, canActivate: [AuthGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'customers', component: CustomersComponent },
    { path: 'customer/:id', component: CustomerComponent, canActivate: [AuthGuard] },
    { path: 'invoices', component: InvoicesComponent, canActivate: [AuthGuard] },
    { path: 'invoice/:id', component: InvoiceComponent, canActivate: [AuthGuard] },
    { path: 'saveDocumentSummary/:id?number=:docNumber&payment=:paymentId', component: SaveDocSummaryComponent, canActivate: [AuthGuard] },
    { path: '', redirectTo: '/categories', pathMatch: 'full' },
    { path: '**', redirectTo: '/categories' }
];
