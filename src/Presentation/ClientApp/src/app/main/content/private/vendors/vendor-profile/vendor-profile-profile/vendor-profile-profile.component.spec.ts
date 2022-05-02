import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorProfileProfileComponent } from './vendor-profile-profile.component';

describe('VendorProfileProfileComponent', () => {
  let component: VendorProfileProfileComponent;
  let fixture: ComponentFixture<VendorProfileProfileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VendorProfileProfileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VendorProfileProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
