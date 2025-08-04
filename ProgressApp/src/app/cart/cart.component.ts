import { Component, inject } from '@angular/core';
import { CartItemsComponent } from "./cart-items/cart-items.component";
import { CartCustomerComponent } from "./cart-customer/cart-customer.component";
import { CartOptionsComponent } from "./cart-options/cart-options.component";
import { CartFinalizeComponent } from "./cart-finalize/cart-finalize.component";
import { ApiService } from '../../services/api.service';
import { CartService } from '../../services/cart.service';
import { WithID } from 'ngx-indexed-db';
import { CartItem, CartItemWithId } from '../../domain/cartItem';
import { Transaction } from '../../domain/transaction';
import { Document, IDocument, User } from '../../domain/generated/apimodel';
import { UserService } from '../../services/user.service';
import { CartPromoItemWithId } from '../../domain/cartPromoItem';
import { Router } from '@angular/router';
import { error } from 'jquery';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CartItemsComponent, CartCustomerComponent, CartOptionsComponent, CartFinalizeComponent],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})

export class CartComponent {

  cartService = inject(CartService);
  apiService = inject(ApiService);
  userService = inject(UserService);
  router = inject(Router);

  saving: boolean = false;
  errorMessage: string = "";

  sendDocument() {
    this.cartService.getCartItems().subscribe(items => {
      this.cartService.getPromoItems().subscribe(promoItems => {
        this.cartService.getCurrentTransaction().subscribe(transaction => {
          this.userService.getUser().subscribe(user => {
            if (user != null)
              this.sendDocument2(items, promoItems, transaction, user);
          })
        })
      })
    });
  }

  private sendDocument2(items: (CartItemWithId)[], promoItems: CartPromoItemWithId[], transaction: Transaction, user: User) {
    var document: IDocument = {
      id: undefined,
      documentType: transaction.document,
      cashPayment: transaction.cashAmount,
      secondPaymentMethod: transaction.secondPaymentMethod,
      secondPaymentAmount: transaction.secondMethodAmount,
      paymentDueDays: transaction.paymentDueDays,
      customerId: transaction.customer?.id,
      deliveryMethod: transaction.deliveryServiceId,
      comment: transaction.comment,
      packagesNumber: transaction.packagesNumber,
      userId: user.id,
      items: items.map(item => {
        return {
          productId: item.productId,
          quantity: item.quantity,
          priceNet: item.priceNet,
          priceGross: item.priceGross,
          discountRate: 0,
          discountAmount: 0,
          taxRate: item.taxRate,
          taxAmount: 0,
          promoSetId: item.promoSetId != null ? (promoItems.find(x => x.id == item.promoSetId)?.promoSetId) : undefined,
          promoItemId: item.promoItemId,
        }
      })
    };

    this.apiService.sendDocument(document).subscribe({
      next: (x) => {
        //console.log(x);
        if (x.isError == false) {
          this.saving = true;
          // this.cartService.clearTransaction(transaction).subscribe(x => {
          //   this.cartService.clearCart().subscribe(x=> {

          //   })
          // });
          this.router.navigate(['/saveDocumentSummary', x.documentId], { queryParams: { number: x.documentNumber, payment: x.payDocumentId } });
        }
        else {
          this.saving = false;
          this.errorMessage = x.message ?? "Wystąpił błąd podczas wysyłania dokumentu. Spróbuj ponownie później.";
        }
      },
      error: (error) => {
        this.saving = false;
        console.log(error);
        this.errorMessage = error;
      }
    });
  }

  saveTransaction() {
    this.saving = true;
    this.errorMessage = "";
    this.sendDocument();
  }

}
