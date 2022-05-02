import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CleaningReportItemTemplateFormComponent } from './cleaning-report-item-template-form.component';

describe('CleaningReportItemTemplateFormComponent', () => {
  let component: CleaningReportItemTemplateFormComponent;
  let fixture: ComponentFixture<CleaningReportItemTemplateFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CleaningReportItemTemplateFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CleaningReportItemTemplateFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
