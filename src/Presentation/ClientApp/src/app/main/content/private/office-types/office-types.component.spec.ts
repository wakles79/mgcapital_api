import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OfficeTypesComponent } from './office-types.component';

describe('OfficeTypesComponent', () => {
  let component: OfficeTypesComponent;
  let fixture: ComponentFixture<OfficeTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OfficeTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OfficeTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
