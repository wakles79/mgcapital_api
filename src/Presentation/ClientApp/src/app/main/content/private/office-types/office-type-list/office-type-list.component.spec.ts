import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OfficeTypeListComponent } from './office-type-list.component';

describe('OfficeTypeListComponent', () => {
  let component: OfficeTypeListComponent;
  let fixture: ComponentFixture<OfficeTypeListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OfficeTypeListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OfficeTypeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
