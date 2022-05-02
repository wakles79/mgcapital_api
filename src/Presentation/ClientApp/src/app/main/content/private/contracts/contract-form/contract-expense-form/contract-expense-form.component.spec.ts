import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractExpenseFormComponent } from './contract-expense-form.component';

describe('ContractExpenseFormComponent', () => {
  let component: ContractExpenseFormComponent;
  let fixture: ComponentFixture<ContractExpenseFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContractExpenseFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractExpenseFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
