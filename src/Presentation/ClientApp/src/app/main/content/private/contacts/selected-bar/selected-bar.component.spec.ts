import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContactsSelectedBarComponent } from './selected-bar.component';

describe('ContactsSelectedBarComponent', () => {
  let component: ContactsSelectedBarComponent;
  let fixture: ComponentFixture<ContactsSelectedBarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContactsSelectedBarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContactsSelectedBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
