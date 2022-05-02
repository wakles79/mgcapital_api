import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PublicDailyReportOperationsManagerComponent } from './public-daily-report-operations-manager.component';

describe('PublicDailyReportOperationsManagerComponent', () => {
  let component: PublicDailyReportOperationsManagerComponent;
  let fixture: ComponentFixture<PublicDailyReportOperationsManagerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PublicDailyReportOperationsManagerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PublicDailyReportOperationsManagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
