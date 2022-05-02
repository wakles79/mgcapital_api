import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuildingProfileDetailComponent } from './building-profile-detail.component';

describe('BuildingProfileDetailComponent', () => {
  let component: BuildingProfileDetailComponent;
  let fixture: ComponentFixture<BuildingProfileDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuildingProfileDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuildingProfileDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
