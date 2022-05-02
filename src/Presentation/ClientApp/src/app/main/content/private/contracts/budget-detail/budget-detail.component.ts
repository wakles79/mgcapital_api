import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ContractExpenseGridModel } from '@app/core/models/contract-expense/contract-expense-grid.model';
import { ContractItemGridModel } from '@app/core/models/contract-item/contract-item-grid.model';
import { ContractActivityLogGridModel } from '@app/core/models/contract/contract-activity-log-grid.model';
import { ContractDetailModel } from '@app/core/models/contract/contract-detail.model';
import { ContractNoteGridModel } from '@app/core/models/contract/contract-note-grid.model';
import { ContractOfficeSpaceModel } from '@app/core/models/contract/contract-office-space.model';
import { fuseAnimations } from '@fuse/animations';
import { BehaviorSubject, Subscription } from 'rxjs';
import { EditContractFormComponent } from '../edit-contract-form/edit-contract-form.component';
import { ContractExpenseFormComponent } from '../contract-form/contract-expense-form/contract-expense-form.component';
import { ContractItemFormComponent } from '../contract-form/contract-item-form/contract-item-form.component';
import { DeleteItemConfirmDialogComponent } from '../delete-item-confirm-dialog/delete-item-confirm-dialog.component';
import { BudgetActivityLogNotesDialogComponent } from '@app/core/modules/budget-activity-log-notes-dialog/budget-activity-log-notes-dialog/budget-activity-log-notes-dialog.component';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { ShareUrlDialogComponent } from '@app/core/modules/share-url-dialog/share-url-dialog/share-url-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BudgetDetailService } from './budget-detail.service';
import { ContractsService } from '../contracts.service';
import { SettingsService } from '../../settings/settings.service';
import { CompanySettingsBaseModel } from '@app/core/models/company-settings/company-settings-base.model';
import { Location } from '@angular/common';
import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';
import { FormGroup } from '@angular/forms';
import { ContractItemBaseModel } from '@app/core/models/contract-item/contract-item-base.model';
import { ContractNoteBaseModel } from '@app/core/models/contract/contract-note-base.model';

@Component({
  selector: 'app-budget-detail',
  templateUrl: './budget-detail.component.html',
  styleUrls: ['./budget-detail.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class BudgetDetailComponent implements OnInit {

  loading$ = new BehaviorSubject<boolean>(false);
  profitMargin = 0;
  occupiedSquareFeet = 0;

  // Bugdet
  budgetDetail: ContractDetailModel;

  dailyProfitFormula: string;
  monthlyProfitFormula: string;
  yearlyProfitFormula: string;

  dailyProfitRatioFormula: string;
  monthlyProfitRatioFormula: string;
  yearlyProfitRatioFormula: string;

  // Revenue
  estimatedRevenue: ContractItemGridModel[] = [];
  totalDailyRevenue = 0;
  totalMonthlyRevenue = 0;
  totalYearlyRevenue = 0;

  // Expenses
  estimatedExpenses: ContractExpenseGridModel[] = [];

  estimatedLaborExpenses: ContractExpenseGridModel[] = [];
  estimatedEquipmentsExpenses: ContractExpenseGridModel[] = [];
  estimatedSuppliesExpenses: ContractExpenseGridModel[] = [];
  estimatedOtherExpenses: ContractExpenseGridModel[] = [];

  totalDailyExpense = 0;
  totalMonthlyExpense = 0;
  totalYearlyExpense = 0;

  totalDailyOverheadExpense = 0;
  totalMonthlyOverheadExpense = 0;
  totalYearlyOverheadExpense = 0;

  totalExpensesOverheadDailyFormula: string;
  totalExpensesOverheadMonthlyFormula: string;
  totalExpensesOverheadYearlyFormula: string;

  notes: ContractNoteGridModel[] = [];
  officeSpaces: ContractOfficeSpaceModel[];
  activityLog: ContractActivityLogGridModel[] = [];
  private _budgetChangedSubscription: Subscription;

  private _budgetEditFormDialog: MatDialogRef<EditContractFormComponent>;
  private _contractExpenseDialog: MatDialogRef<ContractExpenseFormComponent>;
  private _contractRevenueDialog: MatDialogRef<ContractItemFormComponent>;
  private _deleteEstimatedRevenueDialogRef: MatDialogRef<DeleteItemConfirmDialogComponent>;
  private _activityLogNotesDialog: MatDialogRef<BudgetActivityLogNotesDialogComponent>;

  private _confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;
  private _sharePublicUrlDialog: MatDialogRef<ShareUrlDialogComponent>;

  // Notes
  note = '';

  // Sorting
  revenueSortReverse = false;
  expenseSortReverse = false;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    private _location: Location,
    private _dialog: MatDialog,
    private _snackBar: MatSnackBar,
    private _budgetDetailService: BudgetDetailService,
    private _budgetService: ContractsService,
    private _companySettingsService: SettingsService
  ) {

    this.loading$.next(true);
    this._budgetChangedSubscription = this._budgetDetailService.onbudgetDetailChanged
      .subscribe((data) => {
        this.budgetDetail = new ContractDetailModel(data);
        this.loading$.next(false);
      }, (error) => {
        this.loading$.next(false);
        this._snackBar.open('Ops! Error when trying to get budget data', 'Close');
      });

    this._budgetDetailService.onEstimatedRevenueChanged
      .subscribe((data) => {
        this.estimatedRevenue = data;
        this.getOfficeSpaces();
        this.calculateRevenueRate();
      }, (error) => {
        this._snackBar.open('Ops! Error when trying to get estimated revenue', 'Close');
      });

    this._budgetDetailService.onEstimatedExpensesChanged
      .subscribe((data) => {
        this.estimatedExpenses = data;
        this.calculateExpenseRate();
      }, (error) => {
        this._snackBar.open('Ops! Error when trying to get estimated expenses', 'Close');
      });

    this._budgetDetailService.onNotesChanged
      .subscribe((data) => {
        this.notes = data;
      }, (error) => {
        this._snackBar.open('Ops! Error when trying to get notes', 'Close');
      });


    this._budgetDetailService.onActivityLogChanged
      .subscribe((data) => {
        this.activityLog = data;
      }, (error) => {
        this._snackBar.open('Ops! Error when trying to get notes', 'Close');
      });

    this._companySettingsService.loadSettings()
      .then((settings: CompanySettingsBaseModel) => {
        this.profitMargin = settings.minimumProfitMarginPercentage;
      }, () => {
        this.profitMargin = 0;
      });
  }

  ngOnInit(): void {
  }

  // Navigation
  goBack(): void {
    this._location.back();
  }

  // Budget
  editBudget(): void {
    this.loading$.next(true);
    this._budgetDetailService.get(this.budgetDetail.id)
      .subscribe((budget: any) => {
        this.loading$.next(false);
        if (!budget) {
          this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          return;
        }

        const contractUpdate = new ContractBaseModel(budget);
        this._budgetEditFormDialog = this._dialog.open(EditContractFormComponent, {
          panelClass: 'edit-contract-form-dialog',
          data: {
            contract: contractUpdate,
            action: 'edit'
          }
        });

        this._budgetEditFormDialog.afterClosed()
          .subscribe((result: FormGroup) => {
            if (!result) {
              return;
            }

            this.loading$.next(true);
            const contractToUpdate = new ContractBaseModel(result.getRawValue());
            this._budgetService.updateElement(contractToUpdate)
              .then(
                () => {
                  this._snackBar.open('Contract updated successfully!!!', 'Close', { duration: 1000 });
                  this._budgetDetailService.getDetails(this.budgetDetail.id);
                },
                () => {
                  this.loading$.next(false);
                  this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                })
              .catch(() => {
                this.loading$.next(false);
                this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
              });
          });

      });
  }
  get dailyProfit(): number {
    this.dailyProfitFormula = `${this.totalDailyRevenue.toFixed(2)}(Total Daily Revenue) - ${this.totalDailyExpense.toFixed(2)}(Total Daily Expenses)`;
    return this.totalDailyRevenue - this.totalDailyExpense;
  }
  get monthlyProfit(): number {
    this.monthlyProfitFormula = `${this.totalMonthlyRevenue.toFixed(2)}(Total Monthly Revenue) - ${this.totalMonthlyExpense.toFixed(2)}(Total Monthly Expenses)`;
    return this.totalMonthlyRevenue - this.totalMonthlyExpense;
  }
  get yearlyProfit(): number {
    this.yearlyProfitFormula = `${this.totalYearlyRevenue.toFixed(2)}(Total Yearly Revenue) - ${this.totalYearlyExpense.toFixed(2)}(Total Yearly Expenses)`;
    return this.totalYearlyRevenue - this.totalYearlyExpense;
  }
  get dailyProfitRatio(): number {
    this.dailyProfitRatioFormula = `${this.dailyProfit.toFixed(2)}(Total Daily Profit) / ${this.totalDailyRevenue.toFixed(2)}(Total Daily Revenue)`;
    return this.totalDailyRevenue === 0 ? 0 : (this.dailyProfit / this.totalDailyRevenue);
  }
  get monthlyProfitRatio(): number {
    this.monthlyProfitRatioFormula = `${this.monthlyProfit.toFixed(2)}(Total Monthly Profit) / ${this.totalMonthlyRevenue.toFixed(2)}(Total Monthly Revenue)`;
    return this.totalMonthlyRevenue === 0 ? 0 : (this.monthlyProfit / this.totalMonthlyRevenue);
  }
  get yearlyProfitRatio(): number {
    this.yearlyProfitRatioFormula = `${this.yearlyProfit.toFixed(2)}(Total Yearly Profit) / ${this.totalYearlyRevenue.toFixed(2)}(Total Yearly Revenue)`;
    return this.totalYearlyRevenue === 0 ? 0 : (this.yearlyProfit / this.totalYearlyRevenue);
  }
  get urlToCopy(): string {
    return window.location.protocol + '//' + window.location.host + '/contracts/contract-report/' + this.budgetDetail.guid;
  }
  getDocumentUrl(): void {
    this.loading$.next(true);
    this._budgetService.get(this.budgetDetail.id, 'GetBudgetPDFDocumentUrl')
      .subscribe((response: string) => {
        this.loading$.next(false);
        window.open(response, '_blank');
      }, (error) => {
        this.loading$.next(false);
        this._snackBar.open('Oops, there was an error: ' + error, 'close', { duration: 1000 });
      });
  }
  shareBudgetDocument(): void {
    this._sharePublicUrlDialog = this._dialog.open(ShareUrlDialogComponent, {
      panelClass: 'share-url-form-dialog',
      data: {
        urlToCopy: this.urlToCopy
      }
    });
  }
  openPublicBudgetViewNewTab(): void {
    window.open(this.urlToCopy, '_blank');
  }

  // Revenues
  newRevenue(): void {
    this._contractRevenueDialog = this._dialog.open(ContractItemFormComponent, {
      panelClass: 'contract-item-form-dialog',
      data: {
        action: 'new',
        daysPerMonth: Number(this.budgetDetail.daysPerMonth)
      }
    });

    this._contractRevenueDialog.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        this.loading$.next(true);
        const contractItem = new ContractItemGridModel(response.getRawValue());
        contractItem.id = 0;
        contractItem.contractId = this.budgetDetail.id;
        this._budgetService.saveContractItem(contractItem)
          .subscribe(
            () => {
              this._snackBar.open('Contract item created successfully!!!', 'Close', { duration: 1000 });
              this._budgetDetailService.getDetails(this.budgetDetail.id);
            },
            (error) => { this._snackBar.open('Oops, there was an error', 'Close', { duration: 1000 }); }
          );
      });
  }
  updateRevenue(id: number): void {
    this.loading$.next(true);
    this._budgetService.getContractItem(id)
      .subscribe((result: any) => {
        this.loading$.next(false);
        if (!result) {
          return;
        }

        const revenue = new ContractItemBaseModel(result);
        this._contractRevenueDialog = this._dialog.open(ContractItemFormComponent, {
          panelClass: 'contract-item-form-dialog',
          data: {
            action: 'edit',
            contractItem: revenue,
            daysPerMonth: (Number(this.budgetDetail.daysPerMonth))
          }
        });


        this._contractRevenueDialog.afterClosed()
          .subscribe((response: FormGroup) => {
            if (!response) {
              return;
            }
            this.loading$.next(true);
            const contractItem = new ContractItemGridModel(response.getRawValue());
            contractItem.contractId = this.budgetDetail.id;
            this._budgetService.updateContractItem(contractItem)
              .subscribe(
                () => {
                  this._snackBar.open('Contract item created successfully!!!', 'Close', { duration: 1000 });
                  this._budgetDetailService.getDetails(this.budgetDetail.id);
                },
                (error) => {
                  this.loading$.next(false);
                  this._snackBar.open('Oops, there was an error', 'Close', { duration: 1000 });
                });
          });
      }, () => {
        this.loading$.next(false);
        this._snackBar.open('Oops, there was an error', 'Close', { duration: 1000 });
      });
  }
  deleteRevenue(id: number): void {
    this._deleteEstimatedRevenueDialogRef = this._dialog.open(DeleteItemConfirmDialogComponent, {
      disableClose: false
    });

    this._deleteEstimatedRevenueDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete this row?';

    this._deleteEstimatedRevenueDialogRef.afterClosed().subscribe((result: { response: boolean, updatePrepopulatedItems: boolean }) => {
      if (result.response) {
        this._budgetService.deleteContractItem(id, result.updatePrepopulatedItems)
          .then(() => {
            this._snackBar.open('Contract item deleted successfully!!!', 'Close', { duration: 1000 });
            this._budgetDetailService.getDetails(this.budgetDetail.id);
          })
          .catch(() => {
            this._snackBar.open('Oops, there was an error', 'Close', { duration: 1000 });
          });
      }
      this._deleteEstimatedRevenueDialogRef = null;
    });
  }
  sortRevenue(column: string): void {
    this.estimatedRevenue.sort(this.sortArray(column, this.revenueSortReverse === true ? 'asc' : 'desc'));
    this.revenueSortReverse = !this.revenueSortReverse;
  }
  calculateRevenueRate(): void {
    let value = 0;
    const daysMonth = this.budgetDetail.daysPerMonth;
    this.totalDailyRevenue = 0;
    this.totalMonthlyRevenue = 0;
    this.totalYearlyRevenue = 0;

    let position = 1;
    this.estimatedRevenue.forEach(revenue => {

      revenue.order = revenue.order === 0 ? position : revenue.order;

      if (revenue.rateType === 0) {
        value = revenue.hours;
      }
      else if (revenue.rateType === 1) {
        value = 1;
      }
      else if (revenue.rateType === 2) {
        value = revenue.rooms;
      }
      else if (revenue.rateType === 3) {
        value = revenue.squareFeet;
      }

      switch (revenue.ratePeriodicity) {
        case 'Daily':
          revenue.dailyRate = (value * revenue.rate) * revenue.quantity;
          revenue.dailyRateFormula = '((' + value.toFixed(2) + '(Value) * ' + revenue.rate + '(Rate)) * ' + revenue.quantity + '(Quantity))';

          revenue.monthlyRate = (revenue.dailyRate * daysMonth);
          revenue.monthlyRateFormula = '(' + revenue.dailyRate + '(Daily Rate) * ' + daysMonth + '(Days per Month))';

          revenue.yearlyRate = (revenue.monthlyRate * 12);
          revenue.yearlyRateFormula = '(' + revenue.monthlyRate.toFixed(2) + '(Monthly Rate) * 12)';
          break;

        case 'Monthly':
          revenue.monthlyRate = (value * revenue.rate) * revenue.quantity;
          revenue.monthlyRateFormula = '((' + value.toFixed(2) + '(Value) * ' + revenue.rate + '(Rate)) * ' + revenue.quantity + '(Quantity))';

          revenue.dailyRate = (revenue.monthlyRate / daysMonth);
          revenue.dailyRateFormula = '(' + revenue.monthlyRate.toFixed(2) + '(Monthly Rate) / ' + daysMonth + '(Days per Month))';


          revenue.yearlyRate = (revenue.monthlyRate * 12);
          revenue.yearlyRateFormula = '(' + revenue.monthlyRate.toFixed(2) + '(Monthly Rate) * 12)';
          break;

        case 'Bi-Monthly':
          revenue.biMonthlyRate = (value * revenue.rate) * revenue.quantity;
          revenue.biMonthlyRateFormula = '(((Value) * (Rate)) * (Quantity))';

          revenue.monthlyRate = (revenue.biMonthlyRate / 2);
          revenue.monthlyRateFormula = '(' + revenue.biMonthlyRate + '(Bi-Monthly Rate) / 2)';

          revenue.quarterly = (revenue.monthlyRate * 3);
          revenue.quarterlyFormula = '(' + revenue.monthlyRate + '(Monthly Rate) * 3)';

          revenue.biAnnually = (revenue.monthlyRate * 6);
          revenue.biAnnuallyFormula = '(' + revenue.monthlyRate + '(Monthly Rate) * 6)';

          revenue.dailyRate = (revenue.monthlyRate / daysMonth);
          revenue.dailyRateFormula = '(' + revenue.monthlyRate + '(Monthly Rate) / ' + daysMonth + ' (Days per Month))';

          revenue.yearlyRate = (revenue.monthlyRate * 12);
          revenue.yearlyRateFormula = '(' + revenue.monthlyRate + '(Monthly Rate) * 12)';
          break;

        case 'Quarterly':
          revenue.quarterly = (value * revenue.rate) * revenue.quantity;
          revenue.quarterlyFormula = '(((Value) * (Rate)) * (Quantity))';

          revenue.monthlyRate = (revenue.quarterly / 3);
          revenue.monthlyRateFormula = '(' + revenue.quarterly + '(Quarterly Rate) / 3)';

          revenue.biMonthlyRate = (revenue.monthlyRate * 2);
          revenue.biMonthlyRateFormula = '(' + revenue.monthlyRate + '(Monthly Rate) * 2)';

          revenue.biAnnually = (revenue.monthlyRate * 6);
          revenue.biAnnuallyFormula = '(' + revenue.monthlyRate + '(Monthly Rate) * 6)';

          revenue.dailyRate = (revenue.monthlyRate / daysMonth);
          revenue.dailyRateFormula = '(' + revenue.monthlyRate + '(Monthly Rate) / ' + daysMonth + '(Days per Month))';

          revenue.yearlyRate = (revenue.monthlyRate * 12);
          revenue.yearlyRateFormula = '(' + revenue.monthlyRate + '(Monthly Rate) / 12';
          break;

        case 'Bi-Annually':
          revenue.biAnnually = (value * revenue.rate) * revenue.quantity;
          revenue.biAnnuallyFormula = '(((Value) * (Rate)) * (Quantity))';

          revenue.monthlyRate = (revenue.biAnnually / 6);
          revenue.monthlyRateFormula = '(' + revenue.biAnnually + '(Bi-Anually Rate) / 6)';

          revenue.biMonthlyRate = (revenue.monthlyRate * 2);
          revenue.biMonthlyRateFormula = '(' + revenue.monthlyRate + '(Monthly Rate) * 2)';

          revenue.quarterly = (revenue.monthlyRate * 3);
          revenue.quarterlyFormula = '(' + revenue.monthlyRate + '(Monthly Rate) * 3)';

          revenue.dailyRate = (revenue.monthlyRate / daysMonth);
          revenue.dailyRateFormula = '(' + revenue.monthlyRate + '(Monthly Rate) / ' + daysMonth + '(Days per Month))';

          revenue.yearlyRate = (revenue.monthlyRate * 12);
          revenue.yearlyRateFormula = '(' + revenue.monthlyRate + '(Monthly Rate) * 12)';
          break;

        case 'Yearly':
          revenue.yearlyRate = (value * revenue.rate) * revenue.quantity;
          revenue.yearlyRateFormula = '((' + value.toFixed(2) + '(Value) * ' + revenue.rate + '(Rate)) * ' + revenue.quantity + '(Quantity))';

          revenue.monthlyRate = (revenue.yearlyRate / 12);
          revenue.monthlyRateFormula = '((' + revenue.yearlyRate.toFixed(2) + '(Yearly Rate) / 12))';

          revenue.dailyRate = (revenue.monthlyRate / daysMonth);
          revenue.dailyRateFormula = '(' + revenue.monthlyRate.toFixed(2) + '(Monthly Rate) / ' + daysMonth + '(Days per Month))';
          break;
      }

      this.totalDailyRevenue += revenue.dailyRate;
      this.totalMonthlyRevenue += revenue.monthlyRate;
      this.totalYearlyRevenue += revenue.yearlyRate;

      position++;
    });
  }
  moveRevenueUp(id: number, position: number): void {
    const index = this.estimatedRevenue.findIndex(r => r.id === id);
    const previous = this.estimatedRevenue.findIndex(r => r.order === (position - 1));
    if (index >= 0 && previous >= 0) {
      this._budgetService.updateContractRevenueOrder(
        this.estimatedRevenue[index].id,
        position - 1,
        this.estimatedRevenue[previous].id,
        position)
        .subscribe(() => {
          this.loading$.next(true);
          this._budgetDetailService.getDetails(this.budgetDetail.id);
        }, () => {
          this._snackBar.open('Oops, there was an error', 'Close', { duration: 1000 });
        });
    }
  }
  moveRevenueDown(id: number, position: number): void {
    const index = this.estimatedRevenue.findIndex(r => r.id === id);
    const next = this.estimatedRevenue.findIndex(r => r.order === (position + 1));
    if (index >= 0 && next >= 0) {
      this._budgetService.updateContractRevenueOrder(
        this.estimatedRevenue[index].id,
        position + 1,
        this.estimatedRevenue[next].id,
        position)
        .subscribe(() => {
          this.loading$.next(true);
          this._budgetDetailService.getDetails(this.budgetDetail.id);
        }, () => {
          this._snackBar.open('Oops, there was an error', 'Close', { duration: 1000 });
        });
    }
  }

  // Expenses
  newExpense(): void {
    this._contractExpenseDialog = this._dialog.open(ContractExpenseFormComponent, {
      panelClass: 'contract-expense-form-dialog',
      data: {
        action: 'new',
        daysPerMonth: (Number(this.budgetDetail.daysPerMonth))
      }
    });

    this._contractExpenseDialog.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        this.loading$.next(true);
        const expense = new ContractExpenseGridModel(response.getRawValue());
        expense.id = 0;
        expense.contractId = this.budgetDetail.id;
        this._budgetService.saveContractExpense(expense)
          .subscribe(
            () => {
              this._snackBar.open('Contract expense created successfully!!!', 'Close', { duration: 1000 });
              this._budgetDetailService.getDetails(this.budgetDetail.id);
            },
            (error) => { this._snackBar.open('Oops, there was an error', 'Close', { duration: 1000 }); }
          );

        this._contractExpenseDialog = null;
      });
  }
  updateExpense(expense): void {
    this.loading$.next(true);
    this._budgetService.getContractExpense(expense.id)
      .subscribe((result: any) => {
        this.loading$.next(false);
        if (!result) {
          this._snackBar.open('Oops, there was an error', 'Close', { duration: 1000 });
          return;
        }

        this._contractExpenseDialog = this._dialog.open(ContractExpenseFormComponent, {
          panelClass: 'contract-expense-form-dialog',
          data: {
            action: 'edit',
            contractExpense: expense,
            daysPerMonth: (Number(this.budgetDetail.daysPerMonth))
          }
        });

        this._contractExpenseDialog.afterClosed()
          .subscribe((response: FormGroup) => {
            if (!response) {
              return;
            }

            this.loading$.next(true);
            const expenseUpdate = new ContractExpenseGridModel(response.getRawValue());
            expenseUpdate.contractId = this.budgetDetail.id;
            this._budgetService.updateContractExpense(expenseUpdate)
              .subscribe(
                () => {
                  this._snackBar.open('Contract expense updated successfully!!!', 'Close', { duration: 1000 });
                  this._budgetDetailService.getDetails(this.budgetDetail.id);
                },
                (error) => {
                  this.loading$.next(false);
                  this._snackBar.open('Oops, there was an error', 'Close', { duration: 1000 });
                });
          });

      }, (error) => {
        this.loading$.next(false);
        this._snackBar.open('Oops, there was an error', 'Close', { duration: 1000 });
      });
  }
  deleteExpense(id: number): void {
    this._confirmDialogRef = this._dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this._confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete this row?';

    this._confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this._budgetService.deleteContractExpense(id)
          .then(() => {
            this._snackBar.open('Contract expense deleted successfully!!!', 'Close', { duration: 1000 });
            this._budgetDetailService.getDetails(this.budgetDetail.id);
          })
          .catch(() => {
            this._snackBar.open('Oops, there was an error', 'Close', { duration: 1000 });
          });
      }
      this._confirmDialogRef = null;
    }
    );
  }
  sortExpense(column: string): void {

    this.estimatedLaborExpenses.sort(this.sortArray(column, this.expenseSortReverse === true ? 'asc' : 'desc'));
    this.estimatedEquipmentsExpenses.sort(this.sortArray(column, this.expenseSortReverse === true ? 'asc' : 'desc'));
    this.estimatedSuppliesExpenses.sort(this.sortArray(column, this.expenseSortReverse === true ? 'asc' : 'desc'));
    this.estimatedOtherExpenses.sort(this.sortArray(column, this.expenseSortReverse === true ? 'asc' : 'desc'));

    this.expenseSortReverse = !this.expenseSortReverse;

  }
  calculateExpenseRate(): void {
    const daysMonth = this.budgetDetail.daysPerMonth;
    let value = 0;
    let rate = 0;
    let cleanRate = 0;
    let rateText: string;

    this.estimatedLaborExpenses = [];
    this.estimatedEquipmentsExpenses = [];
    this.estimatedSuppliesExpenses = [];
    this.estimatedOtherExpenses = [];

    this.totalDailyExpense = 0;
    this.totalMonthlyExpense = 0;
    this.totalYearlyExpense = 0;

    let totalDailyLaborOverheadExpense = 0;
    let totalMonthlyLaborOverheadExpense = 0;
    let totalYearlyLaborOverheadExpense = 0;

    for (const expense of this.estimatedExpenses) {
      value = expense.value;
      rate = expense.rate;
      cleanRate = expense.rate;
      rateText = '';

      expense.taxesAndInsurance = expense.overheadPercent === 0 ? 0 : (expense.overheadPercent / 100) * expense.rate;

      if (expense.expenseCategory === 0) {
        rate = rate + (rate * (expense.overheadPercent / 100));
        rateText = ` + ${expense.overheadPercent}%`;
      }

      switch (expense.ratePeriodicity) {
        case 'Daily':
          expense.dailyRate = (value * rate) * expense.quantity;
          expense.dailyRateFormula = `((${value.toFixed(2)}(Value) * ${rate.toFixed(2)}(Rate${rateText})) * ${expense.quantity}(Quantity)`;
          // expense.dailyRateFormula = '(((Value)  * (Rate)) * (Quantity))';

          expense.monthlyRate = (expense.dailyRate * daysMonth);
          expense.monthlyRateFormula = `(${expense.dailyRate.toFixed(2)}(Daily Rate) * ${daysMonth}(Days per Month))`;
          // expense.monthlyRateFormula = '((Daily Rate) * (Days per Month))';

          expense.yearlyRate = (expense.monthlyRate * 12);
          expense.yearlyRateFormula = `${expense.monthlyRate.toFixed(2)}(Monthly Rate) * 12`;
          // expense.yearlyRateFormula = '((Monthly Rate) * 12)';

          if (expense.expenseCategory === 0) {
            expense.dailyTaxRate = (value * cleanRate) * expense.quantity;
          }
          break;

        case 'Monthly':
          expense.monthlyRate = (value * rate) * expense.quantity;
          expense.monthlyRateFormula = `(${value.toFixed(2)}(Value) * ${expense.rate.toFixed(2)}(Rate${rateText}) * ${expense.quantity}(Quantity))`;
          // expense.monthlyRateFormula = '(((Value) * (Rate)) * (Quantity))';

          expense.dailyRate = (expense.monthlyRate / daysMonth);
          expense.dailyRateFormula = `(${expense.monthlyRate.toFixed(2)}(Monthly Rate) / ${daysMonth}(Days per Month))`;
          // expense.dailyRateFormula = '((Monthly Rate) / (Days per Month))';

          expense.yearlyRate = (expense.monthlyRate * 12);
          expense.yearlyRateFormula = `(${expense.monthlyRate.toFixed(2)}(Monthly Rate) * 12)`;
          // expense.yearlyRateFormula = '((Monthly Rate) * 12)';

          if (expense.expenseCategory === 0) {
            expense.dailyTaxRate = ((value * cleanRate) * expense.quantity) / daysMonth;
          }
          break;

        case 'Yearly':
          expense.yearlyRate = (value * rate) * expense.quantity;
          expense.yearlyRateFormula = `(${value.toFixed(2)}(Value) * ${expense.rate.toFixed(2)}(Rate${rateText}) * ${expense.quantity}(Quantity))`;
          // expense.yearlyRateFormula = '(((Value) * (Rate)) * (Quantity))';

          expense.monthlyRate = (expense.yearlyRate / 12);
          expense.monthlyRateFormula = `(${expense.yearlyRate.toFixed(2)}(Yearly Rate) / 12)`;
          // expense.monthlyRateFormula = '(((Yearly Rate) / 12))';

          expense.dailyRate = (expense.monthlyRate / daysMonth);
          expense.dailyRateFormula = `(${expense.monthlyRate.toFixed(2)}(Monthly Rate) / ${daysMonth}(Days per Month))`;
          // expense.dailyRateFormula = '((Monthly Rate) / (Days per Month))';

          if (expense.expenseCategory === 0) {
            expense.dailyTaxRate = (((value * cleanRate) * expense.quantity) / 12) / daysMonth;
          }
          break;
      }

      if (expense.expenseCategory === 0) {
        this.estimatedLaborExpenses.push(expense);
        totalDailyLaborOverheadExpense += expense.dailyRate * 0.14;
        totalMonthlyLaborOverheadExpense += expense.monthlyRate * 0.14;
        totalYearlyLaborOverheadExpense += expense.yearlyRate * 0.14;
      }
      else if (expense.expenseCategory === 1) {
        this.estimatedEquipmentsExpenses.push(expense);
      }
      else if (expense.expenseCategory === 2) {
        this.estimatedSuppliesExpenses.push(expense);
      }
      else if (expense.expenseCategory === 3) {
        this.estimatedOtherExpenses.push(expense);
      }

      this.totalDailyExpense += expense.dailyRate;
      this.totalMonthlyExpense += expense.monthlyRate;
      this.totalYearlyExpense += expense.yearlyRate;
    }

    this.totalExpensesOverheadDailyFormula = this.totalDailyExpense.toFixed(2) + ' (Total Daily Labor Expenses) * 14% (Percentage)';
    this.totalDailyOverheadExpense = totalDailyLaborOverheadExpense;
    this.totalDailyExpense += this.totalDailyOverheadExpense;

    this.totalExpensesOverheadMonthlyFormula = this.totalMonthlyExpense.toFixed(2) + ' (Total Monthly Labor Expenses) * 14% (Percentage)';
    this.totalMonthlyOverheadExpense = totalMonthlyLaborOverheadExpense;
    this.totalMonthlyExpense += this.totalMonthlyOverheadExpense;

    this.totalExpensesOverheadYearlyFormula = this.totalYearlyExpense.toFixed(2) + ' (Total Yearly Labor Expenses) * 14% (Percentage)';
    this.totalYearlyOverheadExpense = totalYearlyLaborOverheadExpense;
    this.totalYearlyExpense += this.totalYearlyOverheadExpense;
  }

  // Office Space
  getOfficeSpaces(): void {
    try {
      const os: ContractOfficeSpaceModel[] = [];
      let occupiedSquareFeet = 0;

      this.estimatedRevenue.filter(i => i.rateType === 3).forEach(item => {
        occupiedSquareFeet += item.squareFeet;
        const officeSpaceIndex = os.findIndex(i => i.officeTypeName === item.officeServiceTypeName);
        if (officeSpaceIndex >= 0) {
          os[officeSpaceIndex].squareFeet = item.squareFeet + os[officeSpaceIndex].squareFeet;
        } else {
          os.push({
            id: item.officeServiceTypeId,
            officeTypeName: item.officeServiceTypeName,
            squareFeet: item.squareFeet,
            contractId: 0,
            officeTypeId: 0
          });
        }
      });

      this.occupiedSquareFeet = occupiedSquareFeet;
      this.officeSpaces = os;
    } catch (e) {
      console.log(e);
    }
  }

  // Activity Log
  itemTypeName(type: number): string {
    let name = '';

    switch (type) {
      case 0:
        name = 'Estimated Revenue';
        break;
      case 1:
        name = 'Estimated Expense';
        break;
      case 2:
        name = 'Real Revenue';
        break;
      case 3:
        name = 'Real Expense';
        break;
      default:
        break;
    }

    return name;
  }
  displayActivityLogNotes(id: number): void {
    console.log(id);
    this._activityLogNotesDialog = this._dialog.open(BudgetActivityLogNotesDialogComponent, {
      panelClass: 'budget-activity-log-notes-dialog',
      data: {
        activityLogId: id
      }
    });

    this._activityLogNotesDialog.afterClosed().subscribe(() => {
      this.loading$.next(true);
      this._budgetDetailService.getDetails(this.budgetDetail.id);
      this._activityLogNotesDialog = null;
    });
  }

  // Notes
  addNote(): void {
    if (this.note.trim()) {
      const newNote = new ContractNoteBaseModel(
        {
          id: 0,
          note: this.note.trim(),
          contractId: this.budgetDetail.id,
          employeeId: 0,
          epochCreatedDate: 0
        });

      this.loading$.next(true);
      this._budgetDetailService.addContractNote(newNote)
        .subscribe(() => {
          this._snackBar.open('Note Added Successfully', 'close', { duration: 1000 });
          this.getNotes();
        }, (error) => {
          this.loading$.next(false);
          this._snackBar.open('Oops, there was an error: ' + error, 'close', { duration: 1000 });
        });

    } else {
      this._snackBar.open('Enter valid text', 'close', { duration: 1000 });
    }
  }
  getNotes(): void {
    this._budgetDetailService.getContractNotes(this.budgetDetail.id)
      .subscribe((result: ContractNoteGridModel[]) => {
        this.notes = result;
        this.loading$.next(false);
      }, (error) => {
        this.loading$.next(false);
        this._snackBar.open('Oops, there was an error: ' + error, 'close', { duration: 1000 });
      });
  }

  // Aux
  sortArray(key, order = 'asc'): any {
    const sortOrder = order === 'asc' ? 1 : -1;

    return function (a, b) {
      const result = a[key] < b[key] ? -1 : a[key] > b[key] ? 1 : 0;
      return result * sortOrder;
    };

  }

}
