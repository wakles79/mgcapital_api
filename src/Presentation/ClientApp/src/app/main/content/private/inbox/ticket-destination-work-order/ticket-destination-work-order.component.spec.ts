import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketDestinationWorkOrderComponent } from './ticket-destination-work-order.component';

describe('TicketDestinationWorkOrderComponent', () => {
  let component: TicketDestinationWorkOrderComponent;
  let fixture: ComponentFixture<TicketDestinationWorkOrderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TicketDestinationWorkOrderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketDestinationWorkOrderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
