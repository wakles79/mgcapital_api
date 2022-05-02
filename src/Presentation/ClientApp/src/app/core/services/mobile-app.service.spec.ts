import { TestBed, inject } from '@angular/core/testing';

import { MobileAppService } from './mobile-app.service';

describe('MobileAppService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MobileAppService]
    });
  });

  it('should be created', inject([MobileAppService], (service: MobileAppService) => {
    expect(service).toBeTruthy();
  }));
});
