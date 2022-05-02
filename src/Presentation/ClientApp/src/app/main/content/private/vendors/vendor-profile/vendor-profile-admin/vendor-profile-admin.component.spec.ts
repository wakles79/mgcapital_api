import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorProfileAdminComponent } from './vendor-profile-admin.component';

describe('VendorProfileAdminComponent', () => {
  let component: VendorProfileAdminComponent;
  let fixture: ComponentFixture<VendorProfileAdminComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VendorProfileAdminComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VendorProfileAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
