import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduleSettingsCategoryListComponent } from './schedule-settings-category-list.component';

describe('ScheduleSettingsCategoryListComponent', () => {
  let component: ScheduleSettingsCategoryListComponent;
  let fixture: ComponentFixture<ScheduleSettingsCategoryListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScheduleSettingsCategoryListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduleSettingsCategoryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
