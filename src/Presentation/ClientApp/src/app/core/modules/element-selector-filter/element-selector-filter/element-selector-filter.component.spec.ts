import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ElementSelectorFilterComponent } from './element-selector-filter.component';

describe('ElementSelectorFilterComponent', () => {
  let component: ElementSelectorFilterComponent;
  let fixture: ComponentFixture<ElementSelectorFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ElementSelectorFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ElementSelectorFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
