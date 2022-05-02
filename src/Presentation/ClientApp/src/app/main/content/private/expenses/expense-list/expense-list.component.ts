import { DataSource } from '@angular/cdk/table';
import { AfterViewInit, Component, Input, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { ExpenseBaseModel } from '@app/core/models/expense/expense-base.model';
import { ExpenseGridModel } from '@app/core/models/expense/expense-grid.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { merge, Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { ExpenseFormComponent } from '../expense-form/expense-form.component';
import { ExpensesService } from '../expenses.service';

@Component({
  selector: 'app-expense-list',
  templateUrl: './expense-list.component.html',
  styleUrls: ['./expense-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ExpenseListComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  expenseFormDialog: MatDialogRef<ExpenseFormComponent>;

  loading$ = this.expenseService.loadingSubject.asObservable();

  get expensesCount(): any { return this.expenseService.elementsCount; }
  expenses: ExpenseGridModel[] = [];
  expense: ExpenseBaseModel;
  dataSource: ExpenseDataSource | null;
  columnsToDisplay = ['checkbox', 'transactionNumber', 'contract', 'building', 'customer', 'description', 'date', 'amount', 'buttons'];

  onExpensesChangedSubscription: Subscription;
  onSelectedExpensesChangedSubscription: Subscription;
  onExpensesDataChangedSubscription: Subscription;

  searchInput: FormControl;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  // selected
  selectedExpenses: any[];
  checkboxes: {};

  @Input() readOnly: boolean;

  constructor(
    private expenseService: ExpensesService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar) {

    this.searchInput = new FormControl(this.expenseService.searchText);

    this.onExpensesChangedSubscription = this.expenseService.allElementsChanged
      .subscribe(expenses => {
        this.expenses = expenses;

        this.checkboxes = {};
        expenses.map(expense => {
          this.checkboxes[expense.id] = false;
        });
      });

    this.onSelectedExpensesChangedSubscription =
      this.expenseService.selectedElementsChanged.subscribe(selectedExpenses => {
        for (const id in this.checkboxes) {
          if (!this.checkboxes.hasOwnProperty(id)) {
            continue;
          }
          this.checkboxes[id] = selectedExpenses.includes(id);
        }
        this.selectedExpenses = selectedExpenses;
      });

    this.onExpensesDataChangedSubscription = this.expenseService.elementChanged
      .subscribe(expense => {
        this.expense = expense;
      });

  }

  ngOnInit(): void {
    this.dataSource = new ExpenseDataSource(this.expenseService);

    console.log(this.dataSource);
    this.searchInput.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.expenseService.searchTextChanged.next(searchText);
      });
  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.expenseService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize
        ))
      )
      .subscribe();
  }

  editExpense(expense): void {
    this.expenseFormDialog = this.dialog.open(ExpenseFormComponent, {
      panelClass: 'expense-form-dialog',
      data: {
        action: 'edit',
        expense: expense
      }
    });

    this.expenseFormDialog.afterClosed()
      .subscribe((revenueForm: FormGroup) => {
        if (!revenueForm) {
          return;
        }

        const ExpensesToUpdate = new ExpenseBaseModel(revenueForm.getRawValue());
        this.expenseService.loadingSubject.next(true);
        this.expenseService.updateElement(ExpensesToUpdate)
          .then(
            () => {
              this.expenseService.loadingSubject.next(false);
              this.snackBar.open('Inspection updated successfully!!!', 'close', { duration: 1000 });
            },
            () => {
              this.expenseService.loadingSubject.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            })
          .catch(() => {
            this.expenseService.loadingSubject.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });

      });
  }

  onSelectedChange(revenueID): void {
    this.expenseService.toggleSelectedElement(revenueID);
  }

  deleteExpense(expense): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });
    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.expenseService.delete(expense);
      }
      this.confirmDialogRef = null;
    });
  }

}

export class ExpenseDataSource extends DataSource<ExpenseGridModel>{

  constructor(private expenseService: ExpensesService) {
    super();
  }

  connect(): Observable<any[]> {
    return this.expenseService.allElementsChanged;
  }

  disconnect(): void {

  }
}

