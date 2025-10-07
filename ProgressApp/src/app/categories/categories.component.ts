import { Component, inject, Input, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { ProductCategory } from '../../domain/generated/apimodel';

import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-categories',
    imports: [FormsModule],
    templateUrl: './categories.component.html',
    styleUrl: './categories.component.scss'
})
export class CategoriesComponent implements OnInit {
  private readonly api = inject(ApiService);

  @Input() showFilter: boolean = true;

  categoryList: ProductCategory[] = [];
  categoriesGrouped: { [key: string]: ProductCategory[] } = {};
  filter: string = "";

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

  groupByFirstLetter(categories: ProductCategory[]) {
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

  filterProducts($event: KeyboardEvent) {
    if (this.filter == "") {
      this.categoriesGrouped = this.groupByFirstLetter(this.categoryList);
    } else {
      var filtered = this.categoryList.filter(x => x.name?.toLowerCase().includes(this.filter.toLowerCase()) );
      this.categoriesGrouped = this.groupByFirstLetter(filtered);
    }
  }

  clearFilter() {
    this.filter = "";
  }


}
