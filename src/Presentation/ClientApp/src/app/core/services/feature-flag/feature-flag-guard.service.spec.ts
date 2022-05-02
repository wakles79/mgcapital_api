import { TestBed } from '@angular/core/testing';

import { FeatureFlagGuardService } from './feature-flag-guard.service';

describe('FeatureFlagGuardService', () => {
  let service: FeatureFlagGuardService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FeatureFlagGuardService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
