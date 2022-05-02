import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CleaningReportDetailsComponent } from './cleaning-report-details.component';

describe('CleaningReportDetailsComponent', () => {
  let component: CleaningReportDetailsComponent;
  let fixture: ComponentFixture<CleaningReportDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CleaningReportDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CleaningReportDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
