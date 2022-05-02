import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CleaningReportListComponent } from './cleaning-report-list.component';

describe('CleaningReportListComponent', () => {
  let component: CleaningReportListComponent;
  let fixture: ComponentFixture<CleaningReportListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CleaningReportListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CleaningReportListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
