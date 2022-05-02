import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportRevenueExpenseCsvFormComponent } from './import-revenue-expense-csv-form.component';

describe('ImportRevenueExpenseCsvFormComponent', () => {
  let component: ImportRevenueExpenseCsvFormComponent;
  let fixture: ComponentFixture<ImportRevenueExpenseCsvFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ImportRevenueExpenseCsvFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportRevenueExpenseCsvFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
