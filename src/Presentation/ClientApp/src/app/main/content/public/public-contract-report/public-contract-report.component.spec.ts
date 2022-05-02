import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PublicContractReportComponent } from './public-contract-report.component';

describe('PublicContractReportComponent', () => {
  let component: PublicContractReportComponent;
  let fixture: ComponentFixture<PublicContractReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PublicContractReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PublicContractReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
