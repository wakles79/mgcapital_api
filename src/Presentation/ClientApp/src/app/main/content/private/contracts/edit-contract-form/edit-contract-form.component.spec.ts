import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditContractFormComponent } from './edit-contract-form.component';

describe('EditContractFormComponent', () => {
  let component: EditContractFormComponent;
  let fixture: ComponentFixture<EditContractFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditContractFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditContractFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
