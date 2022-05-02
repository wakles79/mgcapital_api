import { TestBed } from '@angular/core/testing';

import { BudgetTrackingService } from './budget-tracking.service';

describe('BudgetTrackingService', () => {
  let service: BudgetTrackingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BudgetTrackingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
