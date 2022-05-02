import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketFdConversationComponent } from './ticket-fd-conversation.component';

describe('TicketFdConversationComponent', () => {
  let component: TicketFdConversationComponent;
  let fixture: ComponentFixture<TicketFdConversationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TicketFdConversationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketFdConversationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
