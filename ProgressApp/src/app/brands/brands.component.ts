import { Component, inject, Input } from '@angular/core';
import { ProductCategory } from '../../domain/generated/apimodel';
import { ApiService } from '../../services/api.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-brands',
  imports: [FormsModule],
  templateUrl: './brands.component.html',
  styleUrl: './brands.component.scss'
})
export class BrandsComponent {

  private readonly api = inject(ApiService);

  @Input() showFilter: boolean = true;

  categoryList: ProductCategory[] = [];
  categoriesGrouped: { [key: string]: ProductCategory[] } = {};
  filter: string = "";

  get categoriesGroupedKeys() {
    return Object.keys(this.categoriesGrouped);
  }

  ngOnInit(): void {
    this.api.getBrandList().subscribe(response => {
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
      var filtered = this.categoryList.filter(x => x.name?.toLowerCase().includes(this.filter.toLowerCase()));
      this.categoriesGrouped = this.groupByFirstLetter(filtered);
    }
  }

  clearFilter() {
    this.filter = "";
  }

}
