import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ExpenseBaseModel, ExpenseCategory } from '@app/core/models/expense/expense-base.model';
import { fuseAnimations } from '@fuse/animations';

@Component({
  selector: 'app-expenses-form',
  templateUrl: './expenses-form.component.html',
  styleUrls: ['./expenses-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ExpensesFormComponent implements OnInit {

  expenseForm: FormGroup;
  private expense: ExpenseBaseModel;
  private today = new Date();
  categories: { id: number, name: string }[] = [];
  action: any;
  dialogTitle: string;

  constructor(
    public dialogRef: MatDialogRef<ExpensesFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private snackBar: MatSnackBar
  ) {
    this.action = data.action;
    if (this.action === 'add') {
      this.expenseForm = this.createExpenseForm();
      this.dialogTitle = 'New Expense';
    } else if (this.action === 'edit') {
      this.expense = data.item;
      this.dialogTitle = 'Edit Expense';
      this.expenseForm = this.updateExpenseForm();
      console.log(this.expense);
    }
  }
  ngOnInit(): void {
    this.getCategoriesToArray();
  }

  createExpenseForm(): FormGroup {
    return this.formBuilder.group({
      type: [null, [Validators.required]],
      description: [''],
      reference: [''],
      amount: [null, [Validators.required]],
      vendor: [''],
      date: [new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate(), 18, 0, 0), [Validators.required]]
    });
  }

  updateExpenseForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.expense.id],
      isDirect: [this.expense.isDirect],
      contractId: [this.expense.contractId],
      buildingId: [this.expense.buildingId],
      customerId: [this.expense.customerId],
      type: [this.expense.type, [Validators.required]],
      date: [this.expense.date, [Validators.required]],
      reference: [this.expense.reference],
      amount: [this.expense.amount, [Validators.required]],
      vendor: [this.expense.vendor],
      description: [this.expense.description],
      buildingFilter: ['']
    });
  }

  getCategoriesToArray(): void {
    for (const category in ExpenseCategory) {
      if (typeof ExpenseCategory[category] === 'number') {
        this.categories.push({ id: ExpenseCategory[category] as any, name: category });
      }
    }
  }

}
