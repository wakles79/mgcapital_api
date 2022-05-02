import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduleSettingsSubcategoryFormComponent } from './schedule-settings-subcategory-form.component';

describe('ScheduleSettingsSubcategoryFormComponent', () => {
  let component: ScheduleSettingsSubcategoryFormComponent;
  let fixture: ComponentFixture<ScheduleSettingsSubcategoryFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScheduleSettingsSubcategoryFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduleSettingsSubcategoryFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
