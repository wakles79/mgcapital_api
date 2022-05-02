import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CleaningReportComponent } from './cleaning-report.component';

describe('CleaningReportComponent', () => {
  let component: CleaningReportComponent;
  let fixture: ComponentFixture<CleaningReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CleaningReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CleaningReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
