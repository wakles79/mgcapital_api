import { TestBed } from '@angular/core/testing';

import { UsersBaseService } from './users-base.service';

describe('UsersBaseService', () => {
  let service: UsersBaseService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UsersBaseService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
