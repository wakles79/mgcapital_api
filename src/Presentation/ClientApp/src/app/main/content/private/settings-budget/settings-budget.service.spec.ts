import { TestBed } from '@angular/core/testing';

import { SettingsBudgetService } from './settings-budget.service';

describe('SettingsBudgetService', () => {
  let service: SettingsBudgetService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SettingsBudgetService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
