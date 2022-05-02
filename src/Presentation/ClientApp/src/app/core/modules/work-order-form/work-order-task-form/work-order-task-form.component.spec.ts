import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkOrderTaskFormComponent } from './work-order-task-form.component';

describe('WorkOrderTaskFormComponent', () => {
  let component: WorkOrderTaskFormComponent;
  let fixture: ComponentFixture<WorkOrderTaskFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkOrderTaskFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkOrderTaskFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
