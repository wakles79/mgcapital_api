import { TestBed } from '@angular/core/testing';

import { ContractReportDetailService } from './contract-report-detail.service';

describe('ContractReportDetailService', () => {
  let service: ContractReportDetailService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ContractReportDetailService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
