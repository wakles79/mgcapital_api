import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractBalanceDetailComponent } from './contract-balance-detail.component';

describe('ContractBalanceDetailComponent', () => {
  let component: ContractBalanceDetailComponent;
  let fixture: ComponentFixture<ContractBalanceDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContractBalanceDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractBalanceDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
