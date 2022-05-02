import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VerifyFreshdeskComponent } from './verify-freshdesk.component';

describe('VerifyFreshdeskComponent', () => {
  let component: VerifyFreshdeskComponent;
  let fixture: ComponentFixture<VerifyFreshdeskComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VerifyFreshdeskComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VerifyFreshdeskComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
