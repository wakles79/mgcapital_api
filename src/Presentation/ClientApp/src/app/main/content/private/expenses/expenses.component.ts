import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ImportRevenueExpenseCsvFormComponent } from '@app/core/modules/import-revenue-expense-csv-form/import-revenue-expense-csv-form/import-revenue-expense-csv-form.component';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';
import { ExpenseFormComponent } from './expense-form/expense-form.component';
import { RevenueGridModel } from '@app/core/models/revenue/revenue-grid.model';
import { ExpenseGridModel } from '@app/core/models/expense/expense-grid.model';
import { RevenueCSV } from '@app/core/models/revenue/revenue-csv-model';
import { ExpenseCSVModel } from '@app/core/models/expense/expense-csv.model';
import { RevenueAndExpensesCSV } from '@app/core/models/revenue/revenuesAndExpenses';
import { Subscription } from 'rxjs';
import { ExpensesService } from './expenses.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RevenuesService } from '../revenues/revenues.service';
import { FormGroup } from '@angular/forms';
import { ExpenseBaseModel } from '@app/core/models/expense/expense-base.model';
import * as moment from 'moment';

@Component({
  selector: 'app-expenses',
  templateUrl: './expenses.component.html',
  styleUrls: ['./expenses.component.scss']
})
export class ExpensesComponent implements OnInit {

  expenseFormDialog: MatDialogRef<ExpenseFormComponent>;
  // importCsvDialog: MatDialogRef<ExpensesImportCsvComponent>;

  // csv importacion
  importCsvDialog: MatDialogRef<ImportRevenueExpenseCsvFormComponent>;

  revenues: RevenueGridModel[] = [];
  expenses: ExpenseGridModel[] = [];

  public revenuesCSV: RevenueCSV[] = [];
  public expensesCSV: ExpenseCSVModel[] = [];

  public revenuesIsRepeated: RevenueCSV[] = [];
  public expensesISRepeated: ExpenseCSVModel[] = [];
  items: RevenueAndExpensesCSV[] = [];


  // Selected Revenues
  hasSelectedExpenses: boolean;
  onSelectedExpensesChangedSubscription: Subscription;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    private expenseService: ExpensesService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private revenueService: RevenuesService,
    private _fuseSidebarService: FuseSidebarService
  ) { }

  ngOnInit(): void {
    this.onSelectedExpensesChangedSubscription =
      this.expenseService.selectedElementsChanged
        .subscribe(selectedExpenses => {
          this.hasSelectedExpenses = selectedExpenses.length > 0;
        });
  }

  importCsvExpenses(): void {
    // this.importCsvDialog = this.dialog.open(ExpensesImportCsvComponent, {
    //   panelClass: 'expense-import-csv-dialog'
    // });
  }

  newExpense(): void {
    this.expenseFormDialog = this.dialog.open(ExpenseFormComponent, {
      panelClass: 'expense-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.expenseFormDialog.afterClosed()
      .subscribe(
        (expenseForm: FormGroup) => {
          if (!expenseForm) {
            return;
          }

          this.expenseService.loadingSubject.next(true);
          const expense = new ExpenseBaseModel(expenseForm.getRawValue());
          expense.transactionNumber = ((expense.buildingId + moment(expense.date).format('MMDDYYYY').toString()).toString());
          this.expenseService.createElement(expense)
            .then(
              response => {
                this.expenseService.loadingSubject.next(false);
                this.snackBar.open('Expense created successfully!!!', 'close', { duration: 1000 });
              },
              () => {
                this.expenseService.loadingSubject.next(false);
                this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
              }
            )
            .catch(
              () => {
                this.expenseService.loadingSubject.next(false);
                this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
              }
            );
        }
      );
  }

  // importCsv() {
  //   this.importCsvDialog = this.dialog.open(ImportRevenueExpenseCsvFormComponent, {
  //     panelClass: 'import-csv-rev-exp-form-dialog',
  //     data: {
  //     }
  //   });

  //   this.importCsvDialog.afterClosed()
  //     .subscribe((response: FormGroup) => {
  //       if (!response) {
  //         return;
  //       }
  //       this.revenuesCSV = [];
  //       this.expensesCSV = [];

  //       // this.loading$.next(true);
  //       const item = response.getRawValue();

  //       for (let index = 0; index < item.item.length; index++) {

  //         this.items.push(item.item[index]);
  //         let amount;
  //         if (item.item[index].DebitAmount.length === 0) {
  //           amount = 0;
  //         } else {
  //           amount = +item.item[index].DebitAmount;
  //         }

  //         const dateString = item.item[index].Date.toString();
  //         const mm = dateString.slice(0, 2);
  //         const dd = dateString.slice(3, 5);
  //         const yyyy = dateString.slice(6, 10);
  //         item.item[index].date = yyyy + '-' + mm + '-' + dd + 'T00:00:00';



  //         if (amount === 0) {
  //           const itemRevenue: RevenueCSV = new RevenueCSV();
  //           item.item[index].CreditAmount = item.item[index].CreditAmount.replace(/,/gi, '');
  //           itemRevenue.date = new Date(item.item[index].date);
  //           itemRevenue.subTotal = +item.item[index].CreditAmount;
  //           itemRevenue.tax = 0;
  //           itemRevenue.total = +item.item[index].CreditAmount;
  //           itemRevenue.description = item.item[index].Description;
  //           itemRevenue.reference = item.item[index].Reference;
  //           itemRevenue.transactionNumber = item.item[index].TransactionNumber;
  //           itemRevenue.buildingId = item.item[index].BuildingCode;
  //           itemRevenue.customerId = item.item[index].CustomerCode;
  //           itemRevenue.contractId = item.item[index].ContractNumber;
  //           itemRevenue.transactionNumber = ((item.item[index].BuildingCode + moment(itemRevenue.date).format('MMDDYYYY').toString()).toString());
  //           this.revenuesCSV.push(itemRevenue);
  //         } else {
  //           const itemExpenses: ExpenseCSVModel = new ExpenseCSVModel();
  //           item.item[index].DebitAmount = item.item[index].DebitAmount.replace(/,/gi, '');
  //           itemExpenses.date = new Date(item.item[index].Date);
  //           itemExpenses.reference = item.item[index].Reference;
  //           itemExpenses.vendor = '';
  //           itemExpenses.description = item.item[index].Description;
  //           itemExpenses.amount = +item.item[index].DebitAmount;
  //           itemExpenses.transactionNumber = ((item.item[index].BuildingCode + moment(itemExpenses.date).format('MMDDYYYY').toString()).toString());

  //           itemExpenses.buildingId = item.item[index].BuildingCode;
  //           itemExpenses.customerId = item.item[index].CustomerCode;
  //           itemExpenses.contractId = item.item[index].ContractNumber;
  //           this.expensesCSV.push(itemExpenses);
  //         }
  //       }



  //       if (this.expensesCSV.length !== 0 && this.revenuesCSV.length !== 0) {
  //         this.saveRevenuesAndExpenses();
  //       } else {
  //         if (this.expensesCSV.length !== 0) {
  //           this.saveExpenses();
  //         }

  //         if (this.revenuesCSV.length !== 0) {
  //           this.saveRevenues();
  //         }
  //       }
  //       // this.loading$.next(false);
  //     });
  // }


  async saveExpenses(): Promise<any> {
    await this.expenseService.createElementCSV(this.expensesCSV)
      .then(
        (createdExpenses) => {
          this.expensesISRepeated = createdExpenses['body'];
        },
        () => this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 }))
      .catch(() => this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 }));
  }

  async saveRevenues(): Promise<any> {
    await this.revenueService.createElementCSV(this.revenuesCSV)
      .then(
        (createdRevenues) => {
          this.revenuesIsRepeated = createdRevenues['body'];
        },
        () => this.snackBar.open('Oops, There was an error in importing revenues', 'close', { duration: 1000 }))
      .catch(() =>
        this.snackBar.open('Oops, There was an error in importing revenues', 'close', { duration: 1000 })
      );
  }

  async saveRevenuesAndExpenses(): Promise<any> {
    await this.expenseService.createElementCSV(this.expensesCSV)
      .then(
        (createdExpenses) => {
          this.expensesISRepeated = createdExpenses['body'];
        },
        () => this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 }))
      .catch(() =>
        this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 }));

    await this.revenueService.createElementCSV(this.revenuesCSV)
      .then(
        (createdRevenues) => {
          this.revenuesIsRepeated = createdRevenues['body'];
        },
        () => this.snackBar.open('Oops, There was an error in importing revenues', 'close', { duration: 1000 }))
      .catch(() =>
        this.snackBar.open('Oops, There was an error in importing revenues', 'close', { duration: 1000 })
      );

    this.openDialog();
  }

  openDialog(): void {
    const revenuesImported = (this.revenuesCSV.length - this.revenuesIsRepeated.length);
    const expensesImproted = (this.expensesCSV.length - this.expensesISRepeated.length);

    this.importCsvDialog = this.dialog.open(ImportRevenueExpenseCsvFormComponent, {
      panelClass: 'import-csv-rev-exp-form-dialog',
      data: {
        action: 'Result',
        revenuesImported: revenuesImported,
        expensesImproted: expensesImproted,
        revenueRepeated: this.revenuesIsRepeated.length,
        expensesRepeated: this.expensesISRepeated.length,
        total: this.items.length
      }
    });

    this.items = [];
  }

  /**
   * Toggle sidebar
   *
   * @param name
   */
  toggleSidebar(name): void {
    this._fuseSidebarService.getSidebar(name).toggleOpen();
  }

}
