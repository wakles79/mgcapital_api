import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ExpenseSubcategoryBaseModel } from '@app/core/models/expense-subcategory/expense-subcategory-base.model';

@Component({
  selector: 'app-expense-subcategory-form',
  templateUrl: './expense-subcategory-form.component.html',
  styleUrls: ['./expense-subcategory-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ExpenseSubcategoryFormComponent implements OnInit {

  expenseSubcategory: ExpenseSubcategoryBaseModel;
  subcategoryForm: FormGroup;

  action: string;
  dialogTitle: string;

  types: any[] = [
    { 'id': 0, 'name': 'Hour' },
    // { 'id': 1, 'name': 'Amount' },
    { 'id': 2, 'name': 'Room' },
    { 'id': 3, 'name': 'Square Feet' },
    { 'id': 4, 'name': 'Unit' },
    { 'id': 5, 'name': 'Device' }
  ];

  periodicities: any[] = [
    { 'id': 0, 'name': 'Daily' },
    { 'id': 0, 'name': 'Monthly' },
    { 'id': 0, 'name': 'Yearly' }
  ];

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    public dialogRef: MatDialogRef<ExpenseSubcategoryFormComponent>,
    private formBuilder: FormBuilder
  ) {
    this.action = data.action;

    if (this.action === 'new') {
      this.dialogTitle = 'New Subcategory';
      this.subcategoryForm = this.createSubcategoryForm();
    } else {
      this.dialogTitle = 'Update Subcategory';
      this.expenseSubcategory = data.expenseSubcategory;
      this.subcategoryForm = this.updateSubcategoryForm();
    }
  }

  ngOnInit(): void {
  }

  createSubcategoryForm(): FormGroup {
    return this.formBuilder.group({
      name: ['', Validators.required],
      rate: ['', Validators.required],
      rateType: [0, Validators.required],
      periodicity: ['', Validators.required],
      status: [true]
    });
  }

  updateSubcategoryForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.expenseSubcategory.id],
      name: [this.expenseSubcategory.name, Validators.required],
      rate: [this.expenseSubcategory.rate, Validators.required],
      rateType: [this.expenseSubcategory.rateType, Validators.required],
      periodicity: [this.expenseSubcategory.periodicity, Validators.required],
      expenseTypeId: [this.expenseSubcategory.expenseTypeId],
      status: [this.expenseSubcategory.status]
    });
  }

}
