import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PublicProposalDetailComponent } from './public-proposal-detail.component';

describe('PublicProposalDetailComponent', () => {
  let component: PublicProposalDetailComponent;
  let fixture: ComponentFixture<PublicProposalDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PublicProposalDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PublicProposalDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
