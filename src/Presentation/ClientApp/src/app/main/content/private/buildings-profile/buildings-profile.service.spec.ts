import { TestBed } from '@angular/core/testing';

import { BuildingsProfileService } from './buildings-profile.service';

describe('BuildingsProfileService', () => {
  let service: BuildingsProfileService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BuildingsProfileService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
