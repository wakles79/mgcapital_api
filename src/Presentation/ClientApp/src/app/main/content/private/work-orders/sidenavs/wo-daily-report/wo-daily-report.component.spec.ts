import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WoDailyReportComponent } from './wo-daily-report.component';

describe('WoDailyReportComponent', () => {
  let component: WoDailyReportComponent;
  let fixture: ComponentFixture<WoDailyReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WoDailyReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WoDailyReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
