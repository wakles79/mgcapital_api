import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OfficeTypeFormComponent } from './office-type-form.component';

describe('OfficeTypeFormComponent', () => {
  let component: OfficeTypeFormComponent;
  let fixture: ComponentFixture<OfficeTypeFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OfficeTypeFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OfficeTypeFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
