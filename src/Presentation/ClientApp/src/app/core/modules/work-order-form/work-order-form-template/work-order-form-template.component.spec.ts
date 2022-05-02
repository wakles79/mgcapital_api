import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkOrderFormTemplateComponent } from './work-order-form-template.component';

describe('WorkOrderFormTemplateComponent', () => {
  let component: WorkOrderFormTemplateComponent;
  let fixture: ComponentFixture<WorkOrderFormTemplateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkOrderFormTemplateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkOrderFormTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
