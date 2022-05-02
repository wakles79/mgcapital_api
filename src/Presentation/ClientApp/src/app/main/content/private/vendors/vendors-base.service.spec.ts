import { TestBed } from '@angular/core/testing';

import { VendorsBaseService } from './vendors-base.service';

describe('VendorsBaseService', () => {
  let service: VendorsBaseService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(VendorsBaseService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
