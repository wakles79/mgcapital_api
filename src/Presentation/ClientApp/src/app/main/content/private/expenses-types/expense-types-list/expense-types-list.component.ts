import { AfterViewInit, Component, OnInit, ViewEncapsulation, OnDestroy, ViewChild, TemplateRef, Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fuseAnimations } from '@fuse/animations';
import { ExpenseTypeGridModel } from '@app/core/models/expense-type/expense-type-grid.model';
import { ExpenseTypeBaseModel } from '@app/core/models/expense-type/expense-type-base.model';
import { merge, Observable, Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { ExpensesTypesService } from '../expenses-types.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { ExpenseTypeFormComponent } from '@app/core/modules/expenses-form/expense-type-form/expense-type-form.component';
import { DataSource } from '@angular/cdk/table';

@Component({
  selector: 'app-expense-types-list',
  templateUrl: './expense-types-list.component.html',
  styleUrls: ['./expense-types-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ExpenseTypesListComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  expenseTypes: ExpenseTypeGridModel[];
  get expenseTypesCount(): any { return this.expenseTypeService.elementsCount; }
  expenseType: ExpenseTypeBaseModel;
  dataSource: ExpenseTypeDataSource | null;
  displayedColumns = ['description', 'subcategories', 'status', 'buttons'];
  get listCount(): any { return this.expenseTypeService.elementsCount; }

  onExpenseTypeChangedSubscription: Subscription;
  onExpenseTypeDataChangedSubscription: Subscription;

  loading$ = this.expenseTypeService.loadingSubject.asObservable();

  dialogRef: any;

  searchInput: FormControl;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  @Input() readOnly: boolean;

  constructor(
    private expenseTypeService: ExpensesTypesService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {
    this.searchInput = new FormControl(this.expenseTypeService.searchText);

    this.onExpenseTypeChangedSubscription =
      this.expenseTypeService.allElementsChanged.subscribe(officetypes => {
        this.expenseTypes = officetypes;
      });

    this.onExpenseTypeDataChangedSubscription =
      this.expenseTypeService.elementChanged.subscribe(officetype => {
        this.expenseTypes = officetype;
      });
  }

  ngOnInit(): void {
    this.dataSource = new ExpenseTypeDataSource(this.expenseTypeService);

    this.searchInput.valueChanges
      .pipe(debounceTime(300),
        distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.expenseTypeService.searchTextChanged.next(searchText);
      });
  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.expenseTypeService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize
        ))
      )
      .subscribe();

  }

  ngOnDestroy(): void {
    this.onExpenseTypeChangedSubscription.unsubscribe();
    this.onExpenseTypeDataChangedSubscription.unsubscribe();
  }

  activeStatus(status: boolean): any {
    return status ? 'Active' : 'Inactive';
  }

  activeOptions(status: boolean): any {
    return status ? 'Disable' : 'Enable';
  }

  editExpenseType(expenseTypeId: any): void {
    this.expenseTypeService.get(expenseTypeId)
      .subscribe(
        (expenseTypeData: any) => {
          if (!expenseTypeData) {
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            return;
          }

          const expenseType = new ExpenseTypeBaseModel(expenseTypeData);
          this.dialogRef = this.dialog.open(ExpenseTypeFormComponent, {
            panelClass: 'expense-type-form-dialog',
            data: {
              expenseType: expenseType,
              action: 'edit'
            }
          });

          this.dialogRef.afterClosed()
            .subscribe((response: string) => {
              if (!response) {
                return;
              }

              if (response === 'success') {
                this.snackBar.open('Expense type updated successfully!!!', 'close', { duration: 1000 });
              } else {
                this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
              }
            });
        },
        (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 })
      );
  }

  changeStatusExpenseType(expenseTypeId: any): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to change the status of the expense type?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.expenseTypeService.delete(expenseTypeId);
      }
      this.confirmDialogRef = null;
    });
  }

}

export class ExpenseTypeDataSource extends DataSource<any>{
  constructor(private expenseTypeService: ExpensesTypesService) {
    super();
  }

  connect(): Observable<any[]> {
    return this.expenseTypeService.allElementsChanged;
  }

  disconnect(): void {
  }
}
