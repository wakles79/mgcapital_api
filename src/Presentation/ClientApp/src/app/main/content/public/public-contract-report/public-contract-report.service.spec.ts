import { TestBed } from '@angular/core/testing';

import { PublicContractReportService } from './public-contract-report.service';

describe('PublicContractReportService', () => {
  let service: PublicContractReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PublicContractReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
