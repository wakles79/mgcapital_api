import { TestBed } from '@angular/core/testing';

import { ContractBalanceDetailService } from './contract-balance-detail.service';

describe('ContractBalanceDetailService', () => {
  let service: ContractBalanceDetailService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ContractBalanceDetailService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
