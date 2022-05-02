import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduleSettingsCategoryFormComponent } from './schedule-settings-category-form.component';

describe('ScheduleSettingsCategoryFormComponent', () => {
  let component: ScheduleSettingsCategoryFormComponent;
  let fixture: ComponentFixture<ScheduleSettingsCategoryFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScheduleSettingsCategoryFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduleSettingsCategoryFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
