import { TestBed } from '@angular/core/testing';

import { ScheduleSettingsCategoryService } from './schedule-settings-category.service';

describe('ScheduleSettingsCategoryService', () => {
  let service: ScheduleSettingsCategoryService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ScheduleSettingsCategoryService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
