import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResultImportCsvComponent } from './result-import-csv.component';

describe('ResultImportCsvComponent', () => {
  let component: ResultImportCsvComponent;
  let fixture: ComponentFixture<ResultImportCsvComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResultImportCsvComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResultImportCsvComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
