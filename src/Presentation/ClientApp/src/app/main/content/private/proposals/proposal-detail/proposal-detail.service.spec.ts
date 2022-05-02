import { TestBed } from '@angular/core/testing';

import { ProposalDetailService } from './proposal-detail.service';

describe('ProposalDetailService', () => {
  let service: ProposalDetailService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProposalDetailService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
