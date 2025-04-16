import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductGrid5Component } from './product-grid-5.component';

describe('ProductGrid5Component', () => {
  let component: ProductGrid5Component;
  let fixture: ComponentFixture<ProductGrid5Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductGrid5Component]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductGrid5Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
