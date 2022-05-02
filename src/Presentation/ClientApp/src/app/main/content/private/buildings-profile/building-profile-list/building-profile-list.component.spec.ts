import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuildingProfileListComponent } from './building-profile-list.component';

describe('BuildingProfileListComponent', () => {
  let component: BuildingProfileListComponent;
  let fixture: ComponentFixture<BuildingProfileListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuildingProfileListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuildingProfileListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
