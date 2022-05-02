import { TestBed } from '@angular/core/testing';

import { WorkOrderServicesService } from './work-order-services.service';

describe('WorkOrderServicesService', () => {
  let service: WorkOrderServicesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(WorkOrderServicesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
