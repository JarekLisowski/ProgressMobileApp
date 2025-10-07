import { NgStyle } from '@angular/common';
import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { DropDownItems } from '../../domain/dropDownItem';

@Component({
    selector: 'search-bar',
    imports: [FormsModule, NgStyle],
    templateUrl: './search-bar.component.html',
    styleUrl: './search-bar.component.scss'
})
export class SearchBarComponent implements AfterViewInit {

  @ViewChild('searchGroup') searchGroupRef!: ElementRef;
  dropDownWidth: number = 200;

  dropDownWidthStyles = {
    'width': this.dropDownWidth.toString() + 'px',
  };

  _searchText: string = '';

  _searchTimeout: any | undefined;

  get searchText(): string {
    return this._searchText;
  }

  set searchText(value: string) {
    console.log(value);
    this._searchText = value;
    clearTimeout(this._searchTimeout);
    this._searchTimeout = setTimeout(() => {
      this.startSearch();
    }, 500);
  }

  startSearch() {
    console.log(this.searchText);
    this.dropdownVisible = false;
    this.apiService.search(this.searchText).subscribe(x => {
      var items1 = x.productCategories?.map(c => {
        return {
          link: `/category/` + (c.id ?? ""),
          text: <string>c.name
        }
      });
      var items2 = x.products?.map(p => {
        return {
          link: `/product/${p.id}`,
          text: `${p.name} (${p.code})`
        }
      });

      if (items1 != null)
        this.dropDownCategories = items1;
      if (items2 != null)
        this.dropDownProducts = items2;

      this.openDropDown();

    });
  }

  dropdownVisible: boolean = !true;
  dropDownCategories: DropDownItems[] = []
  dropDownProducts: DropDownItems[] = [];

  constructor(private apiService: ApiService) {
  }

  ngAfterViewInit(): void {
    // Access the nativeElement to get the DOM element

  }

  openDropDown() {
    if (this.searchGroupRef) {
      var searchGroupWidth = this.searchGroupRef.nativeElement.offsetWidth;
      if (searchGroupWidth != undefined) {
        console.log('Div Width after init:', searchGroupWidth, 'px');
        this.dropDownWidth = searchGroupWidth;
      }
    }
    this.dropdownVisible = true;
  }

}