import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Customer, Document } from '../../domain/generated/apimodel';
import { CommonModule } from '@angular/common';
import { InvoicesComponent } from "../invoices/invoices.component";

@Component({
  selector: 'app-customer',
  standalone: true,
  imports: [CommonModule, InvoicesComponent],
  templateUrl: './customer.component.html',
  styleUrl: './customer.component.scss'
})
export class CustomerComponent implements OnInit {

  customer: Customer | undefined;
  
  customerId: number = 0;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService
  ) 
  { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.apiService.getCustomer(id).subscribe(customer => {
        this.customer = customer.data;
        console.log(this.customer);
        if (this.customer?.id != undefined)
          this.customerId = this.customer.id;
      });
    }
  }
}
