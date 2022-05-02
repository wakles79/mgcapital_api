import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuildingsProfileComponent } from './buildings-profile.component';

describe('BuildingsProfileComponent', () => {
  let component: BuildingsProfileComponent;
  let fixture: ComponentFixture<BuildingsProfileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuildingsProfileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuildingsProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
