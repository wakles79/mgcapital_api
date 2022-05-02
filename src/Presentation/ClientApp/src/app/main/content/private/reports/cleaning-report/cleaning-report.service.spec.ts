import { TestBed } from '@angular/core/testing';

import { CleaningReportService } from './cleaning-report.service';

describe('CleaningReportService', () => {
  let service: CleaningReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CleaningReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
