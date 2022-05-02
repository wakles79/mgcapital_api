import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProposalServiceFormComponent } from './proposal-service-form.component';

describe('ProposalServiceFormComponent', () => {
  let component: ProposalServiceFormComponent;
  let fixture: ComponentFixture<ProposalServiceFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProposalServiceFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProposalServiceFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
