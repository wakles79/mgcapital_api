import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduleSettingsCategoryComponent } from './schedule-settings-category.component';

describe('ScheduleSettingsCategoryComponent', () => {
  let component: ScheduleSettingsCategoryComponent;
  let fixture: ComponentFixture<ScheduleSettingsCategoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScheduleSettingsCategoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduleSettingsCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
