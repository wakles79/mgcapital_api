import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CalendarItemFormComponent } from './calendar-item-form.component';

describe('CalendarItemFormComponent', () => {
  let component: CalendarItemFormComponent;
  let fixture: ComponentFixture<CalendarItemFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CalendarItemFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CalendarItemFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
