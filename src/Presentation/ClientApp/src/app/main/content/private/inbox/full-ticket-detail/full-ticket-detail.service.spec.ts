import { TestBed } from '@angular/core/testing';

import { FullTicketDetailService } from './full-ticket-detail.service';

describe('FullTicketDetailService', () => {
  let service: FullTicketDetailService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FullTicketDetailService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
