import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PublicWorkOrderComponent } from './public-work-order.component';

describe('PublicWorkOrderComponent', () => {
  let component: PublicWorkOrderComponent;
  let fixture: ComponentFixture<PublicWorkOrderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PublicWorkOrderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PublicWorkOrderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
