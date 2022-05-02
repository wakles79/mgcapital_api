import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketReplyFormComponent } from './ticket-reply-form.component';

describe('TicketReplyFormComponent', () => {
  let component: TicketReplyFormComponent;
  let fixture: ComponentFixture<TicketReplyFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TicketReplyFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketReplyFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
