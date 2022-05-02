import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenseSubcategoryFormComponent } from './expense-subcategory-form.component';

describe('ExpenseSubcategoryFormComponent', () => {
  let component: ExpenseSubcategoryFormComponent;
  let fixture: ComponentFixture<ExpenseSubcategoryFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpenseSubcategoryFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpenseSubcategoryFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
