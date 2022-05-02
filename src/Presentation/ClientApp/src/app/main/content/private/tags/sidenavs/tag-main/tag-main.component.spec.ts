import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TagMainComponent } from './tag-main.component';

describe('TagMainComponent', () => {
  let component: TagMainComponent;
  let fixture: ComponentFixture<TagMainComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TagMainComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TagMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
