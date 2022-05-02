import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WoTaskBillingFormComponent } from './wo-task-billing-form.component';

describe('WoTaskBillingFormComponent', () => {
  let component: WoTaskBillingFormComponent;
  let fixture: ComponentFixture<WoTaskBillingFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WoTaskBillingFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WoTaskBillingFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
