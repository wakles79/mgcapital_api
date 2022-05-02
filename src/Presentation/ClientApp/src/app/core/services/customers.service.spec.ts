import { TestBed, inject } from '@angular/core/testing';

import { CustomersBaseService } from './customers.service';

describe('CustomersBaseService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CustomersBaseService]
    });
  });

  it('should be created', inject([CustomersBaseService], (service: CustomersBaseService) => {
    expect(service).toBeTruthy();
  }));
});
