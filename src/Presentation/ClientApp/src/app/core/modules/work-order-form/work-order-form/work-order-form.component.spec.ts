import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkOrderSharedFormComponent } from './work-order-form.component';

describe('WorkOrderFormComponent', () => {
  let component: WorkOrderSharedFormComponent;
  let fixture: ComponentFixture<WorkOrderSharedFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkOrderSharedFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkOrderSharedFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
