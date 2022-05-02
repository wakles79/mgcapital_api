import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OfficeSpaceFormComponent } from './office-space-form.component';

describe('OfficeSpaceFormComponent', () => {
  let component: OfficeSpaceFormComponent;
  let fixture: ComponentFixture<OfficeSpaceFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OfficeSpaceFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OfficeSpaceFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
