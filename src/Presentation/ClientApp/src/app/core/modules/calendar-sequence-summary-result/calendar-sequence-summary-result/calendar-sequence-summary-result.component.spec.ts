import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CalendarSequenceSummaryResultComponent } from './calendar-sequence-summary-result.component';

describe('CalendarSequenceSummaryResultComponent', () => {
  let component: CalendarSequenceSummaryResultComponent;
  let fixture: ComponentFixture<CalendarSequenceSummaryResultComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CalendarSequenceSummaryResultComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CalendarSequenceSummaryResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
