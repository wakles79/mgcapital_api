import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import * as moment from 'moment';
import { fuseAnimations } from '@fuse/animations';
import { ContractTrackingDetailModel } from '@app/core/models/contract/contract-tracking-detail.model';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { RevenueGridModel } from '@app/core/models/revenue/revenue-grid.model';
import { ExpenseGridModel } from '@app/core/models/expense/expense-grid.model';
import { RevenueCSV } from '@app/core/models/revenue/revenue-csv-model';
import { ExpenseCSVModel } from '@app/core/models/expense/expense-csv.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { RevenuesFormComponent } from '../contract-balance-detail/revenues-form/revenues-form.component';
import { ExpensesFormComponent } from '../contract-balance-detail/expenses-form/expenses-form.component';
import { ImportRevenueExpenseCsvFormComponent } from '@app/core/modules/import-revenue-expense-csv-form/import-revenue-expense-csv-form/import-revenue-expense-csv-form.component';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { FormControl, FormGroup } from '@angular/forms';
import { RevenueAndExpensesCSV } from '@app/core/models/revenue/revenuesAndExpenses';
import { BudgetTrackingService } from './budget-tracking.service';
import { RevenuesService } from '../../revenues/revenues.service';
import { ExpensesService } from '../../expenses/expenses.service';
import { SettingsService } from '../../settings/settings.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CompanySettingsBaseModel } from '@app/core/models/company-settings/company-settings-base.model';
import { takeUntil } from 'rxjs/operators';
import { Location } from '@angular/common';
import { RevenueBaseModel } from '@app/core/models/revenue/revenue-base.model';
import { ExpenseBaseModel } from '@app/core/models/expense/expense-base.model';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';

@Component({
  selector: 'app-budget-tracking',
  templateUrl: './budget-tracking.component.html',
  styleUrls: ['./budget-tracking.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class BudgetTrackingComponent implements OnInit {

  showXAxisLabel: boolean;
  showYAxisLabel: boolean;

  monthNames: string[] = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
  expenseTypeNames: string[] = ['Labor', 'Equipments', 'Supplies', 'Others', 'Subcontractor'];

  yearlyDataView: any[] = [550, 700];
  data: any[] = [];

  expensePieDataView: any[] = [400, 400];
  expensePieColorScheme = {
    domain: ['#66bb6a', '#ef5350']
  };
  expensePieData: any[] = [];

  yearBalance: any[] = [
    {
      'name': 'Revenue',
      'series': [
        {
          'name': 'January',
          'value': 40
        },
        {
          'name': 'February',
          'value': 80
        },
        {
          'name': 'March',
          'value': 10
        },
        {
          'name': 'April',
          'value': 20
        },
        {
          'name': 'May',
          'value': 70
        },
        {
          'name': 'June',
          'value': 60
        },
        {
          'name': 'July',
          'value': 10
        },
        {
          'name': 'August',
          'value': 70
        },
        {
          'name': 'September',
          'value': 30
        },
        {
          'name': 'October',
          'value': 20
        },
        {
          'name': 'November',
          'value': 40
        },
        {
          'name': 'December',
          'value': 100
        }
      ]
    },
    {
      'name': 'Expense',
      'series': [
        {
          'name': 'January',
          'value': 10
        },
        {
          'name': 'February',
          'value': 20
        },
        {
          'name': 'March',
          'value': 70
        },
        {
          'name': 'April',
          'value': 40
        },
        {
          'name': 'May',
          'value': 90
        },
        {
          'name': 'June',
          'value': 20
        },
        {
          'name': 'July',
          'value': 30
        },
        {
          'name': 'August',
          'value': 90
        },
        {
          'name': 'September',
          'value': 80
        },
        {
          'name': 'October',
          'value': 70
        },
        {
          'name': 'November',
          'value': 30
        },
        {
          'name': 'December',
          'value': 40
        }
      ]
    }
  ];

  budget: ContractTrackingDetailModel;

  budgetDataChangedSubscription: Subscription;

  revenues: RevenueGridModel[] = [];
  expenses: ExpenseGridModel[] = [];

  public revenuesCSV: RevenueCSV[] = [];
  public expensesCSV: ExpenseCSVModel[] = [];

  public revenuesIsRepeated: RevenueCSV[] = [];
  public expensesISRepeated: ExpenseCSVModel[] = [];

  private today = new Date();
  availabeYears: number[] = [];
  currentYear: number;

  loading$ = new BehaviorSubject<boolean>(false);

  revenueDialog: MatDialogRef<RevenuesFormComponent>;
  expenseDialog: MatDialogRef<ExpensesFormComponent>;

  importCsvDialog: MatDialogRef<ImportRevenueExpenseCsvFormComponent>;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  dateFrom: FormControl;
  dateFromMonthFilter = 0;
  dateTo: FormControl;
  dateToMonthFilter = 12;

  yearlyProfits: { month: string, netProfit: number, profitRatio: number }[] = [];

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  items: RevenueAndExpensesCSV[] = [];
  profitMargin: number;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    private location: Location,
    private budgetTrackingService: BudgetTrackingService,
    private revenueService: RevenuesService,
    private expenseService: ExpensesService,
    private companySettings: SettingsService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private _fuseSidebarService: FuseSidebarService
  ) {

    this.dateFrom = new FormControl();
    this.dateTo = new FormControl();

    this.loading$.next(true);
    this.budgetDataChangedSubscription = this.budgetTrackingService.onBudgetChanged
      .subscribe((budgetData) => {
        this.budget = budgetData;
        this.currentYear = this.today.getFullYear();
        this.availabeYears = [];
        this.availabeYears.push(this.today.getFullYear());

        this.calculateProfitByMonths();

        if (this.budget.revenues.length > 0) {
          this.data = this.proccessMonthlyChart();
        } else {
          this.data = this.createEmptyMonthlyChart();
        }

        if (this.budget.expenses.length > 0) {
          // this.expensePieData = this.proccessExpensePieChart();
        } else {
          // this.expensePieData = this.createEmptyExpensePieChart();
        }

        this.getAvailableYears();
        this.loading$.next(false);
      }, (error) => {
        this.loading$.next(false);
        this.snackBar.open('Ops! Error when trying to get budget data', 'Close');
      });

    this.companySettings.loadPublicSettings()
      .then((settings: CompanySettingsBaseModel) => {
        this.profitMargin = settings.minimumProfitMarginPercentage;
      });
  }

  ngOnInit(): void {
    this.getExpenses();

    this.dateFrom.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {

        const dateFrom = this.dateFrom.value ? moment(this.dateFrom.value).format('YYYY-MM-DD') : null;
        const dateTo = this.dateTo.value ? moment(this.dateTo.value).format('YYYY-MM-DD') : null;

        this.dateFromMonthFilter = this.dateFrom.value ? new Date(this.dateFrom.value).getMonth() : 0;

        this.loading$.next(true);
        this.budgetTrackingService.getDetails(this.budget.id, dateFrom, dateTo);
      });

    this.dateTo.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        const dateFrom = this.dateFrom.value ? moment(this.dateFrom.value).format('YYYY-MM-DD') : null;
        const dateTo = this.dateTo.value ? moment(this.dateTo.value).format('YYYY-MM-DD') : null;

        this.dateToMonthFilter = this.dateTo.value ? new Date(this.dateTo.value).getMonth() : 0;

        this.loading$.next(true);
        this.budgetTrackingService.getDetails(this.budget.id, dateFrom, dateTo);
      });
  }

  goBack(): void {
    this.location.back();
  }

  /** Monthly Chart */
  proccessMonthlyChart(): any[] {
    const yearBalance: any[] = [
      {
        'name': 'Revenue',
        'series': []
      },
      {
        'name': 'Expense',
        'series': []
      }
    ];

    let totalRevenue = 0;
    let totalExpense = 0;

    for (let month = 0; month < 12; month++) {

      totalRevenue = this.budget.revenues
        .filter(i => new Date(i.date).getMonth() === month && new Date(i.date).getFullYear() === this.currentYear)
        .map(i => i.total)
        .reduce((acc, value) => acc + value, 0);

      totalExpense = this.budget.expenses
        .filter(e => new Date(e.date).getMonth() === month && new Date(e.date).getFullYear() === this.currentYear)
        .map(e => e.amount)
        .reduce((acc, value) => acc + value, 0);

      // Revenue
      yearBalance[0].series.push(
        {
          'name': this.monthNames[month],
          'value': totalRevenue
        }
      );

      // Expense
      yearBalance[1].series.push(
        {
          'name': this.monthNames[month],
          'value': totalExpense
        }
      );

      totalRevenue = 0;
      totalExpense = 0;
    }
    return yearBalance;
  }

  createEmptyMonthlyChart(): any[] {
    const yearBalance: any[] = [
      {
        'name': 'Revenue',
        'series': []
      },
      {
        'name': 'Expense',
        'series': []
      }
    ];

    for (let month = 0; month < 12; month++) {
      // Revenue
      yearBalance[0].series.push(
        {
          'name': this.monthNames[month],
          'value': 0
        }
      );

      // Expense
      yearBalance[1].series.push(
        {
          'name': this.monthNames[month],
          'value': 0
        }
      );
    }
    return yearBalance;
  }

  proccessExpensePieChart(): any[] {
    const result: any[] = [];
    let total = 0;
    for (let index = 0; index < this.expenseTypeNames.length; index++) {
      total = this.budget.expenses
        .filter(e => e.type === index)
        .map(e => e.amount)
        .reduce((acc, value) => acc + value, 0);

      const chartItem = { 'name': this.expenseTypeNames[index], 'value': total };
      result.push(chartItem);
      total = 0;
    }
    return result;
  }

  createEmptyExpensePieChart(): any[] {
    const result: any[] = [];
    const chartItem = { 'name': 'None', 'value': 100 };
    result.push(chartItem);
    return result;
  }

  getAvailableYears(): void {
    if (this.budget.revenues.length > 0) {
      this.budget.revenues.map(r => new Date(r.date).getFullYear()).forEach(y => {
        this.availabeYears.push(y);
      });
    }

    if (this.budget.expenses.length > 0) {
      this.budget.expenses.map(e => new Date(e.date).getFullYear()).forEach(y => {
        this.availabeYears.push(y);
      });
    }

    if (this.availabeYears.length > 0) {
      this.availabeYears.sort((a, b) => a - b);

      this.availabeYears = this.availabeYears.filter((y, i, a) => i === a.indexOf(y));
    }
  }

  changeYear(year: number): void {
    if (this.currentYear !== year) {
      this.currentYear = year;
      this.data = this.proccessMonthlyChart();
      this.calculateProfitByMonths();
    }
  }

  getExpenses(): void {
  }

  /** Buttons */

  /** Revenues */
  addRevenue(): void {
    this.revenueDialog = this.dialog.open(RevenuesFormComponent, {
      panelClass: 'revenue-form-dialog',
      data: {
        action: 'add'
      }
    });

    this.revenueDialog.afterClosed().subscribe((response: FormGroup) => {
      if (!response) {
        return;
      }

      const newRevenue = new RevenueBaseModel(response.getRawValue());
      newRevenue.buildingId = this.budget.buildingId;
      newRevenue.contractId = this.budget.id;
      newRevenue.customerId = this.budget.customerId;

      this.loading$.next(true);
      this.revenueService.createElement(newRevenue)
        .then(
          (createdProposal) => {
            this.loading$.next(false);
            this.snackBar.open('Revenue created successfully!!!', 'close', { duration: 1000 });
            this.budgetTrackingService.getDetails(this.budget.id);
          },
          () => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          })
        .catch(() => {
          this.loading$.next(false);
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });
    });
  }

  editRevenue(id: number): void {
    this.loading$.next(true);
    this.revenueService.get(id).subscribe((response: RevenueBaseModel) => {

      this.loading$.next(false);
      this.revenueDialog = this.dialog.open(RevenuesFormComponent, {
        panelClass: 'revenue-form-dialog',
        data: {
          action: 'edit',
          item: response
        }
      });

      this.revenueDialog.afterClosed().subscribe((form: FormGroup) => {
        if (!form) {
          return;
        }

        const RevenueToUpdate = new RevenueBaseModel(form.getRawValue());
        this.loading$.next(true);
        this.revenueService.updateElement(RevenueToUpdate)
          .then(
            () => {
              this.loading$.next(false);
              this.snackBar.open('Revenue Updated Successfully!!!', 'close', { duration: 1000 });
              this.budgetTrackingService.getDetails(this.budget.id);
            },
            () => {
              this.loading$.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            })
          .catch(() => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
      });

    }, (error) => {
      this.loading$.next(false);
      this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
    });
  }

  deleteRenenue(id: number): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete this revenue?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loading$.next(true);
        this.revenueService.deleteRevenue(id).then(
          () => {
            this.loading$.next(false);
            this.snackBar.open('Revenue Deleted Successfully!!!', 'close', { duration: 1000 });
            this.budgetTrackingService.getDetails(this.budget.id);
          },
          () => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          })
          .catch(() => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
      }

      this.confirmDialogRef = null;
    });
  }

  get totalRevenues(): number {
    return this.budget.revenues ? this.budget.revenues.map(i => i.total).reduce((acc, value) => acc + value, 0) : 0;
  }

  /** Expenses */
  addExpense(): void {
    this.expenseDialog = this.dialog.open(ExpensesFormComponent, {
      panelClass: 'expense-form-dialog',
      data: {
        action: 'add'
      }
    });

    this.expenseDialog.afterClosed().subscribe((response: FormGroup) => {
      if (!response) {
        return;
      }

      const newExpense = new ExpenseBaseModel(response.getRawValue());
      newExpense.buildingId = this.budget.buildingId;
      newExpense.contractId = this.budget.id;
      newExpense.customerId = this.budget.customerId;

      this.loading$.next(true);
      this.expenseService.createElement(newExpense)
        .then(
          (createdProposal) => {
            this.loading$.next(false);
            this.snackBar.open('Revenue created successfully!!!', 'close', { duration: 1000 });
            this.budgetTrackingService.getDetails(this.budget.id);
          },
          () => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          })
        .catch(() => {
          this.loading$.next(false);
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });
    });
  }

  editExpense(id: number): void {
    this.loading$.next(true);
    this.expenseService.get(id).subscribe((response: RevenueBaseModel) => {
      this.loading$.next(false);
      this.expenseDialog = this.dialog.open(ExpensesFormComponent, {
        panelClass: 'expense-form-dialog',
        data: {
          action: 'edit',
          item: response
        }
      });

      this.expenseDialog.afterClosed().subscribe((form: FormGroup) => {
        if (!form) {
          return;
        }

        this.loading$.next(true);
        const expenseToUpdate = new ExpenseBaseModel(form.getRawValue());
        expenseToUpdate.buildingId = this.budget.buildingId;
        expenseToUpdate.contractId = this.budget.id;
        expenseToUpdate.customerId = this.budget.customerId;
        this.expenseService.updateElement(expenseToUpdate)
          .then(
            () => {
              this.loading$.next(false);
              this.snackBar.open('expense updated successfully!!!', 'close', { duration: 1000 });
              this.budgetTrackingService.getDetails(this.budget.id);
            },
            () => {
              this.loading$.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            })
          .catch(() => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
      });
    }, (error) => {
      this.loading$.next(false);
      this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
    });
  }

  deleteExpense(id: number): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete this expense?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.expenseService.deleteExpense(id).then(
          () => {
            this.loading$.next(false);
            this.snackBar.open('Expense Deleted Successfully!!!', 'close', { duration: 1000 });
            this.budgetTrackingService.getDetails(this.budget.id);
          },
          () => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          })
          .catch(() => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
      }
      this.confirmDialogRef = null;
    });
  }

  get totalExpenses(): number {
    return this.budget.expenses ? this.budget.expenses.map(e => e.amount).reduce((acc, value) => acc + value, 0) : 0;
  }

  /** Profit */
  get totalProfit(): number {
    return this.totalRevenues - this.totalExpenses;
  }

  get totalProfitRatio(): number {
    return this.totalRevenues === 0 ? 0 : (this.totalProfit / this.totalRevenues);
  }

  async saveExpenses(): Promise<any> {
    await this.expenseService.createElementCSV(this.expensesCSV)
      .then(
        (createdExpenses) => {
          this.expensesISRepeated = createdExpenses['body'];
          this.snackBar.open('expenses created successfully!!!', 'close', { duration: 1000 });
          this.budgetTrackingService.getDetails(this.budget.id);
        },
        () => this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 }))
      .catch(() => this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 }));
  }

  async saveRevenues(): Promise<any> {
    await this.revenueService.createElementCSV(this.revenuesCSV)
      .then(
        (createdRevenues) => {
          this.revenuesIsRepeated = createdRevenues['body'];
          this.snackBar.open('Revenues created successfully!!!', 'close', { duration: 1000 });
          this.budgetTrackingService.getDetails(this.budget.id);
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
          this.snackBar.open('expenses created successfully!!!', 'close', { duration: 1000 });
          this.budgetTrackingService.getDetails(this.budget.id);
        },
        () => this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 }))
      .catch(() => this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 }));

    await this.revenueService.createElementCSV(this.revenuesCSV)
      .then(
        (createdRevenues) => {
          this.revenuesIsRepeated = createdRevenues['body'];
          this.snackBar.open('Revenues created successfully!!!', 'close', { duration: 1000 });
          this.budgetTrackingService.getDetails(this.budget.id);
        },
        () => this.snackBar.open('Oops, There was an error in importing revenues', 'close', { duration: 1000 }))
      .catch(() =>
        this.snackBar.open('Oops, There was an error in importing revenues', 'close', { duration: 1000 })
      );


    console.log(this.revenuesIsRepeated);
    console.log(this.expensesISRepeated);
  }

  calculateProfitByMonths(): void {
    let totalRevenue = 0;
    let totalExpense = 0;
    this.yearlyProfits = [];

    for (let month = 0; month < 12; month++) {

      totalRevenue = this.budget.revenues
        .filter(i => new Date(i.date).getMonth() === month && new Date(i.date).getFullYear() === this.currentYear)
        .map(i => i.total)
        .reduce((acc, value) => acc + value, 0);

      totalExpense = this.budget.expenses
        .filter(e => new Date(e.date).getMonth() === month && new Date(e.date).getFullYear() === this.currentYear)
        .map(e => e.amount)
        .reduce((acc, value) => acc + value, 0);

      const totalProfit = totalRevenue - totalExpense;
      this.yearlyProfits.push(
        {
          month: this.monthNames[month],
          netProfit: totalProfit,
          profitRatio: totalRevenue === 0 ? 0 : (totalProfit / totalRevenue)
        }
      );
      // this.yearlyProfits[month].netProfit = totalProfit;
      // this.yearlyProfits[month].profitRatio = totalRevenue === 0 ? 0 : (totalProfit / totalRevenue);
    }
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
