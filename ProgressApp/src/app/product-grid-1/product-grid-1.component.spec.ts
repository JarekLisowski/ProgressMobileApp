import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductGrid1Component } from './product-grid-1.component';

describe('ProductGrid1Component', () => {
  let component: ProductGrid1Component;
  let fixture: ComponentFixture<ProductGrid1Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductGrid1Component]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductGrid1Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
