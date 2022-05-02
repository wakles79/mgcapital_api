import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractItemFormComponent } from './contract-item-form.component';

describe('ContractItemFormComponent', () => {
  let component: ContractItemFormComponent;
  let fixture: ComponentFixture<ContractItemFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContractItemFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractItemFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
