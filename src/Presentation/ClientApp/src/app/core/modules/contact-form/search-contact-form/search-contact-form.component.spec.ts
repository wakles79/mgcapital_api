import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchContactFormComponent } from './search-contact-form.component';

describe('SearchContactFormComponent', () => {
  let component: SearchContactFormComponent;
  let fixture: ComponentFixture<SearchContactFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SearchContactFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SearchContactFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
