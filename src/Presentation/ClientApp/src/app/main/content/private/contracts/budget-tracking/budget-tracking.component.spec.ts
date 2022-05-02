import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BudgetTrackingComponent } from './budget-tracking.component';

describe('BudgetTrackingComponent', () => {
  let component: BudgetTrackingComponent;
  let fixture: ComponentFixture<BudgetTrackingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BudgetTrackingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BudgetTrackingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
