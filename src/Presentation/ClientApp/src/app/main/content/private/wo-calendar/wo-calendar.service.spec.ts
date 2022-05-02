import { TestBed } from '@angular/core/testing';

import { WoCalendarService } from './wo-calendar.service';

describe('WoCalendarService', () => {
  let service: WoCalendarService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(WoCalendarService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
