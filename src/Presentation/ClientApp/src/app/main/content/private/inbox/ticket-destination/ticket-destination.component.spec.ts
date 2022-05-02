import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketDestinationComponent } from './ticket-destination.component';

describe('TicketDestinationComponent', () => {
  let component: TicketDestinationComponent;
  let fixture: ComponentFixture<TicketDestinationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TicketDestinationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketDestinationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
