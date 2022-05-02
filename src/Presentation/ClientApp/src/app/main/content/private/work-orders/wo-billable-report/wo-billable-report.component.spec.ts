import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WoBillableReportComponent } from './wo-billable-report.component';

describe('WoBillableReportComponent', () => {
  let component: WoBillableReportComponent;
  let fixture: ComponentFixture<WoBillableReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WoBillableReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WoBillableReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
