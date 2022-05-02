import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WoDailyReportOmDocumentComponent } from './wo-daily-report-om-document.component';

describe('WoDailyReportOmDocumentComponent', () => {
  let component: WoDailyReportOmDocumentComponent;
  let fixture: ComponentFixture<WoDailyReportOmDocumentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WoDailyReportOmDocumentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WoDailyReportOmDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
