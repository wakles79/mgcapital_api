import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ExpenseTypeFormComponent } from '@app/core/modules/expenses-form/expense-type-form/expense-type-form.component';
import { fuseAnimations } from '@fuse/animations';

@Component({
  selector: 'app-expenses-types',
  templateUrl: './expenses-types.component.html',
  styleUrls: ['./expenses-types.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ExpensesTypesComponent implements OnInit, OnDestroy {

  expenseTypeFormRef: any;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {

  }

  newExpenseType(): void {
    this.expenseTypeFormRef = this.dialog.open(ExpenseTypeFormComponent, {
      panelClass: 'expense-type-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.expenseTypeFormRef.afterClosed()
      .subscribe((response: string) => {
        if (!response) {
          return;
        }

        if (response === 'success') {
          this.snackBar.open('Expense type created successfully!!!', 'close', { duration: 1000 });
        } else {
          this.snackBar.open(response, 'close', { duration: 1000 });
        }
      });
  }

}
