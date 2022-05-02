import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkOrderServicesComponent } from './work-order-services.component';

describe('WorkOrderServicesComponent', () => {
  let component: WorkOrderServicesComponent;
  let fixture: ComponentFixture<WorkOrderServicesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkOrderServicesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkOrderServicesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
