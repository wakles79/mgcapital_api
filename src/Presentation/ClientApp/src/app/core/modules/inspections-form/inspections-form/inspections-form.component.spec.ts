import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InspectionsFormComponent } from './inspections-form.component';

describe('InspectionsFormComponent', () => {
  let component: InspectionsFormComponent;
  let fixture: ComponentFixture<InspectionsFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InspectionsFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InspectionsFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
