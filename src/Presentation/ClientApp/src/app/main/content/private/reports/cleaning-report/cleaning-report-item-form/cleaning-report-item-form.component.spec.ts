import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CleaningReportItemFormComponent } from './cleaning-report-item-form.component';

describe('CleaningReportItemFormComponent', () => {
  let component: CleaningReportItemFormComponent;
  let fixture: ComponentFixture<CleaningReportItemFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CleaningReportItemFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CleaningReportItemFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
