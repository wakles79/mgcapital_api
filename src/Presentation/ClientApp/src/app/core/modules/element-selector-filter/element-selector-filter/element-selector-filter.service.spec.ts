import { TestBed } from '@angular/core/testing';

import { ElementSelectorFilterService } from './element-selector-filter.service';

describe('ElementSelectorFilterService', () => {
  let service: ElementSelectorFilterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ElementSelectorFilterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
