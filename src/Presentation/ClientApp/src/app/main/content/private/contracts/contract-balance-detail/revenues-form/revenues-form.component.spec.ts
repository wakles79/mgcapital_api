import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RevenuesFormComponent } from './revenues-form.component';

describe('RevenuesFormComponent', () => {
  let component: RevenuesFormComponent;
  let fixture: ComponentFixture<RevenuesFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RevenuesFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RevenuesFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
