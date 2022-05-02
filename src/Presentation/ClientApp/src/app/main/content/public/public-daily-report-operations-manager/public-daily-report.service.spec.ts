import { TestBed } from '@angular/core/testing';

import { PublicDailyReportService } from './public-daily-report.service';

describe('PublicDailyReportService', () => {
  let service: PublicDailyReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PublicDailyReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
