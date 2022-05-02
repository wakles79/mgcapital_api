import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ExpenseCSVModel } from '@app/core/models/expense/expense-csv.model';
import { ExpenseGridModel } from '@app/core/models/expense/expense-grid.model';
import { RevenueCSV } from '@app/core/models/revenue/revenue-csv-model';
import { RevenueGridModel } from '@app/core/models/revenue/revenue-grid.model';
import { RevenueAndExpensesCSV } from '@app/core/models/revenue/revenuesAndExpenses';
import { ImportRevenueExpenseCsvFormComponent } from '@app/core/modules/import-revenue-expense-csv-form/import-revenue-expense-csv-form/import-revenue-expense-csv-form.component';
import { fuseAnimations } from '@fuse/animations';
import { Subscription } from 'rxjs';
import { ExpensesService } from '../expenses/expenses.service';
import { RevenueFormComponent } from './revenue-form/revenue-form.component';
import { RevenueImportCsvFormComponent } from './revenue-import-csv-form/revenue-import-csv-form.component';
import { RevenuesService } from './revenues.service';
import * as moment from 'moment';
import { RevenueBaseModel } from '@app/core/models/revenue/revenue-base.model';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';

@Component({
  selector: 'app-revenues',
  templateUrl: './revenues.component.html',
  styleUrls: ['./revenues.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class RevenuesComponent implements OnInit {

  revenueFormDialog: MatDialogRef<RevenueFormComponent>;
  revenueImportCsvFormDialog: MatDialogRef<RevenueImportCsvFormComponent>;

  // Selected Revenues
  hasSelectedRevenues: boolean;
  onSelectedRevenuesChangedSubscription: Subscription;

  // csv importacion
  importCsvDialog: MatDialogRef<ImportRevenueExpenseCsvFormComponent>;

  revenues: RevenueGridModel[] = [];
  expenses: ExpenseGridModel[] = [];

  public revenuesCSV: RevenueCSV[] = [];
  public expensesCSV: ExpenseCSVModel[] = [];

  public revenuesIsRepeated: RevenueCSV[] = [];
  public expensesISRepeated: ExpenseCSVModel[] = [];
  items: RevenueAndExpensesCSV[] = [];

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private revenueService: RevenuesService,
    private expenseService: ExpensesService,
    private _fuseSidebarService: FuseSidebarService

  ) { }

  ngOnInit(): void {
    this.onSelectedRevenuesChangedSubscription =
      this.revenueService.selectedElementsChanged
        .subscribe(selectedRevenues => {
          this.hasSelectedRevenues = selectedRevenues.length > 0;
        });
  }

  newRevenue(): void {
    this.revenueFormDialog = this.dialog.open(RevenueFormComponent, {
      panelClass: 'revenue-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.revenueFormDialog.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        const revenue = new RevenueBaseModel(response.getRawValue());
        revenue.transactionNumber = ((revenue.buildingId + moment(revenue.date).format('MMDDYYYY').toString()).toString());

        this.revenueService.createElement(revenue)
          .then(
            (createdProposal) => {
              this.snackBar.open('Revenue created successfully!!!', 'close', { duration: 1000 });
            },
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });
  }

  newImportRevenue(): void {
    this.revenueImportCsvFormDialog = this.dialog.open(RevenueImportCsvFormComponent, {
      panelClass: 'revenue-import-csv-form-dialog',
      data: {
      }
    });
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
  //           item.item[index].CreditAmount = item.item[index].CreditAmount.replace(/,/gi, '');
  //           const itemRevenue: RevenueCSV = new RevenueCSV();
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
  //           itemExpenses.vendor = item.item[index].Vendor;
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
      .catch(() => this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 }));

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
