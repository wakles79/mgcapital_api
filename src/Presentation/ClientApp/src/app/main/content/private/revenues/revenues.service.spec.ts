import { TestBed } from '@angular/core/testing';

import { RevenuesService } from './revenues.service';

describe('RevenuesService', () => {
  let service: RevenuesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RevenuesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
