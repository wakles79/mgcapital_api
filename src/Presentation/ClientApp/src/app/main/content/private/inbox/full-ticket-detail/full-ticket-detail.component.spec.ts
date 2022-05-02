import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FullTicketDetailComponent } from './full-ticket-detail.component';

describe('FullTicketDetailComponent', () => {
  let component: FullTicketDetailComponent;
  let fixture: ComponentFixture<FullTicketDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FullTicketDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FullTicketDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
