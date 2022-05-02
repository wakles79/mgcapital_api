import { TestBed } from '@angular/core/testing';

import { CleaningReportDetailsService } from './cleaning-report-details.service';

describe('CleaningReportDetailsService', () => {
  let service: CleaningReportDetailsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CleaningReportDetailsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
