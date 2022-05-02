import { TestBed } from '@angular/core/testing';

import { InspectionDetailService } from './inspection-detail.service';

describe('InspectionDetailService', () => {
  let service: InspectionDetailService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(InspectionDetailService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
