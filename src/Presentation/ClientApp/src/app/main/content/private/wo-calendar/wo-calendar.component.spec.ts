import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WoCalendarComponent } from './wo-calendar.component';

describe('WoCalendarComponent', () => {
  let component: WoCalendarComponent;
  let fixture: ComponentFixture<WoCalendarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WoCalendarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WoCalendarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
