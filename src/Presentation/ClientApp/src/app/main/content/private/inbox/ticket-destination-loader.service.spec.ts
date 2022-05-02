import { TestBed } from '@angular/core/testing';

import { TicketDestinationLoaderService } from './ticket-destination-loader.service';

describe('TicketDestinationLoaderService', () => {
  let service: TicketDestinationLoaderService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TicketDestinationLoaderService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
