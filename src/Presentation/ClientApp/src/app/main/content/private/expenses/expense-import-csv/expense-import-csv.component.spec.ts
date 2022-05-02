import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenseImportCsvComponent } from './expense-import-csv.component';

describe('ExpenseImportCsvComponent', () => {
  let component: ExpenseImportCsvComponent;
  let fixture: ComponentFixture<ExpenseImportCsvComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpenseImportCsvComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpenseImportCsvComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
