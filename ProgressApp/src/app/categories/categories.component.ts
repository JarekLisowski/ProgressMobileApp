import { Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { ProductCategory } from '../../domain/generated/apimodel';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [NgFor],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss'
})
export class CategoriesComponent  implements OnInit {
  
  private readonly api = inject(ApiService);
  
  categoryList: ProductCategory[] = [];
  categoriesGrouped: { [key: string]: ProductCategory[] } = {};
  get categoriesGroupedKeys() {
    return Object.keys(this.categoriesGrouped);
  }

  ngOnInit(): void {
    this.api.getCategoryList().subscribe(response => {
      if (response != null && response.data != null)
        this.categoryList = response.data;
      this.categoriesGrouped = this.groupByFirstLetter(this.categoryList);
    });
  }
  
  groupByFirstLetter(categories : ProductCategory[]) {
    const grouped: { [key: string]: ProductCategory[] } = {};
    for (const category of categories) {
      if (category.name != null && category.name.length > 0) {
        const firstLetter = category.name[0].toUpperCase(); // Ensure consistent case
        if (grouped[firstLetter]) {
          grouped[firstLetter].push(category);
        } else {
          grouped[firstLetter] = [category];
        }
      }
    }
    return grouped;
  }
  
}
