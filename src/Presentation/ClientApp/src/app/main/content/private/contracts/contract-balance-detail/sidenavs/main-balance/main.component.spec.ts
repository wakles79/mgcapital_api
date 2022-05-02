import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MainBalanceComponent } from './main.component';

describe('MainBalanceComponent', () => {
  let component: MainBalanceComponent;
  let fixture: ComponentFixture<MainBalanceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MainBalanceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MainBalanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
