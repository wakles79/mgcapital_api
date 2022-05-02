import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CleaningReportFormComponent } from './cleaning-report-form.component';

describe('CleaningReportFormComponent', () => {
  let component: CleaningReportFormComponent;
  let fixture: ComponentFixture<CleaningReportFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CleaningReportFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CleaningReportFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
