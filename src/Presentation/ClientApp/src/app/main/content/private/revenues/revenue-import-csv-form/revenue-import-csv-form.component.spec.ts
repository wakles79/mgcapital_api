import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RevenueImportCsvFormComponent } from './revenue-import-csv-form.component';

describe('RevenueImportCsvFormComponent', () => {
  let component: RevenueImportCsvFormComponent;
  let fixture: ComponentFixture<RevenueImportCsvFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RevenueImportCsvFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RevenueImportCsvFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
