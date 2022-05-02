import { TestBed } from '@angular/core/testing';

import { ExpensesTypesService } from './expenses-types.service';

describe('ExpensesTypesService', () => {
  let service: ExpensesTypesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ExpensesTypesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
