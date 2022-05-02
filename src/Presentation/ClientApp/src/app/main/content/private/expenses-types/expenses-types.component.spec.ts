import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpensesTypesComponent } from './expenses-types.component';

describe('ExpensesTypesComponent', () => {
  let component: ExpensesTypesComponent;
  let fixture: ComponentFixture<ExpensesTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpensesTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpensesTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
