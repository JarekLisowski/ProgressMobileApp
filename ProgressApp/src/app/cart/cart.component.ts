import { Component, inject } from '@angular/core';
import { CartItemsComponent } from "./cart-items/cart-items.component";
import { CartCustomerComponent } from "./cart-customer/cart-customer.component";
import { CartOptionsComponent } from "./cart-options/cart-options.component";
import { CartFinalizeComponent } from "./cart-finalize/cart-finalize.component";
import { ApiService } from '../../services/api.service';
import { CartService } from '../../services/cart.service';
import { WithID } from 'ngx-indexed-db';
import { CartItem } from '../../domain/cartItem';
import { Transaction } from '../../domain/transaction';
import { Document, IDocument, User } from '../../domain/generated/apimodel';
import { UserService } from '../../services/user.service';

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

  sendDocument() {
    this.cartService.getCartItems().subscribe(items => {
      this.cartService.getCurrentTransaction().subscribe(transaction => {
        this.userService.getUser().subscribe(user => {
          if (user != null)
            this.sendDocument2(items, transaction, user);
        })
      })
    });
  }

  private sendDocument2(items: (CartItem & WithID)[], transaction: Transaction, user: User) {
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
          taxAmount: 0
        }})
    };
    this.apiService.sendDocument(document).subscribe(x => {
      console.log(x);
      // this.cartService.clearTransaction(transaction).subscribe(x => {
      //   this.cartService.clearCart().subscribe(x=> {

      //   })
      // });
    });

  }

  saveTransaction() {
    this.sendDocument();
  }

}
