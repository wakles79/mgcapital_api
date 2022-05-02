import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PublicCleaningReportComponent } from './public-cleaning-report.component';

describe('PublicCleaningReportComponent', () => {
  let component: PublicCleaningReportComponent;
  let fixture: ComponentFixture<PublicCleaningReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PublicCleaningReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PublicCleaningReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
