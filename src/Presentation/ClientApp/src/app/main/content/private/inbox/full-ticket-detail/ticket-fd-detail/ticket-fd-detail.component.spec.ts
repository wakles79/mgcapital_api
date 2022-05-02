import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketFdDetailComponent } from './ticket-fd-detail.component';

describe('TicketFdDetailComponent', () => {
  let component: TicketFdDetailComponent;
  let fixture: ComponentFixture<TicketFdDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TicketFdDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketFdDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
