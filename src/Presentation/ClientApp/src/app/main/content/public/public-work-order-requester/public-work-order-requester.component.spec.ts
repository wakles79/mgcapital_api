import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PublicWorkOrderRequesterComponent } from './public-work-order-requester.component';

describe('PublicWorkOrderRequesterComponent', () => {
  let component: PublicWorkOrderRequesterComponent;
  let fixture: ComponentFixture<PublicWorkOrderRequesterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PublicWorkOrderRequesterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PublicWorkOrderRequesterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
