import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractReportDetailComponent } from './contract-report-detail.component';

describe('ContractReportDetailComponent', () => {
  let component: ContractReportDetailComponent;
  let fixture: ComponentFixture<ContractReportDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContractReportDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractReportDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
