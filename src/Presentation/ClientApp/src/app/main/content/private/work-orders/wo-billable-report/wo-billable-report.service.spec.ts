import { TestBed } from '@angular/core/testing';

import { WoBillableReportService } from './wo-billable-report.service';

describe('WoBillableReportService', () => {
  let service: WoBillableReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(WoBillableReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
