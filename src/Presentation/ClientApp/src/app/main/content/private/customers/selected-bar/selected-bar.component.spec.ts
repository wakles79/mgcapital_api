import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomersSelectedBarComponent } from './selected-bar.component';

describe('CustomersSelectedBarComponent', () => {
  let component: CustomersSelectedBarComponent;
  let fixture: ComponentFixture<CustomersSelectedBarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomersSelectedBarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomersSelectedBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
