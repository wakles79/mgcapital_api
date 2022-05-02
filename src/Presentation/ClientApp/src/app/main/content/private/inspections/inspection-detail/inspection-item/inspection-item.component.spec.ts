import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InspectionItemComponent } from './inspection-item.component';

describe('InspectionItemComponent', () => {
  let component: InspectionItemComponent;
  let fixture: ComponentFixture<InspectionItemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InspectionItemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InspectionItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
