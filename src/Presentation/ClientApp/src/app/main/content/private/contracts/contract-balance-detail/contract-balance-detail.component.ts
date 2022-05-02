import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ContractReportDetailsModel } from '@app/core/models/contract/contract-report-detail.model';
import { ExpenseGridModel } from '@app/core/models/expense/expense-grid.model';
import * as moment from 'moment';
import { BehaviorSubject, Subscription } from 'rxjs';
import { ContractsService } from '../contracts.service';
import { ContractBalanceDetailService } from './contract-balance-detail.service';
import { ExpensesFormComponent } from './expenses-form/expenses-form.component';
import { RevenuesFormComponent } from './revenues-form/revenues-form.component';
import { Location } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RevenuesService } from '../../revenues/revenues.service';
import { ExpensesService } from '../../expenses/expenses.service';
import { RevenueBaseModel } from '@app/core/models/revenue/revenue-base.model';
import { ExpenseBaseModel } from '@app/core/models/expense/expense-base.model';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';

@Component({
  selector: 'app-contract-balance-detail',
  templateUrl: './contract-balance-detail.component.html',
  styleUrls: ['./contract-balance-detail.component.scss']
})
export class ContractBalanceDetailComponent implements OnInit {

  contractReportDetail: ContractReportDetailsModel;
  revenueFormDialog: MatDialogRef<RevenuesFormComponent>;
  expenseFormDialog: MatDialogRef<ExpensesFormComponent>;
  contractDataChangedSubscription: Subscription;
  loading$ = new BehaviorSubject<boolean>(false);

  private today = new Date();
  dateBegin = Date;
  dateEnd = Date;
  // expenses

  dsContractExpenses: ExpenseGridModel[] = [];

  hasLaborExpenses = false;
  laborExpenses: ExpenseGridModel[] = [];

  hasEquipmentsExpenses = false;
  EquipmentsExpenses: ExpenseGridModel[] = [];

  hasSuppliesExpenses = false;
  SuppliesExpenses: ExpenseGridModel[] = [];

  hasOthersExpenses = false;
  OthersExpenses: ExpenseGridModel[] = [];

  hasSubcontractorExpenses = false;
  SubcontractorExpenses: ExpenseGridModel[] = [];

  filterBy: { [key: string]: any } = {
    'dateFrom': '',
    'dateTo': '',
  };

  // filter
  dateFrom = moment(moment().add(-1, 'd').toDate()).format('YYYY-MM-DD');
  dateTo = moment(moment().add().toDate()).format('Y  YYY-MM-DD');
  dateFromCtrl = new FormControl('', Validators.required);
  dateToCtrl = new FormControl('', Validators.required);

  constructor(
    private contractService: ContractsService,
    private contractReportBalanceService: ContractBalanceDetailService,
    private location: Location,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private revenueService: RevenuesService,
    private expenseService: ExpensesService,
    private _fuseSidebarService: FuseSidebarService
  ) {
    this.loading$.next(true);
    this.filterBy = this.contractReportBalanceService.filterBy;
    this.dateFromCtrl.setValue(this.contractReportBalanceService.dateFrom);
    this.dateToCtrl.setValue(this.contractReportBalanceService.dateTo);

    this.contractDataChangedSubscription =
      this.contractReportBalanceService.onContractReportDetailChanged.subscribe(
        (contractData: any) => {
          this.loading$.next(false);
          this.contractReportDetail = contractData;
          this.dsContractExpenses = this.contractReportDetail.expenses;
        }
      );
  }

  ngOnInit(): void {
    this.cargarLista();
  }

  goBack(): void {
    this.location.back();
  }

  cargarLista(): void {
    if (this.dsContractExpenses) {
      this.getLaborExpenses();
      this.getEquipmentsExpenses();
      this.getSuppliesExpenses();
      this.getOthersExpenses();
      this.getSubcontractorExpenses();
    }
  }

  getStatus(status: number): any {
    if (status === 0) {
      return 'Pending';
    } else if (status === 1) {
      return 'Active';
    } else if (status === 2) {
      return 'Finished';
    } else if (status === 3) {
      return 'Declined';
    }
  }

  get totalExpenses(): number {
    if (this.contractReportDetail.expenses !== null) {
      return this.contractReportDetail ? this.contractReportDetail.expenses.map(i => i.amount).reduce((acc, value) => acc + value, 0) : 0;
    } else {
      return 0;
    }

  }

  get totalRevenues(): number {
    if (this.contractReportDetail.revenues !== null) {
      return this.contractReportDetail ? this.contractReportDetail.revenues.map(i => i.total).reduce((acc, value) => acc + value, 0) : 0;
    } else {
      return 0;
    }
  }

  get Profit(): number {
    return this.contractReportDetail ? (this.totalRevenues - this.totalExpenses) : 0;
  }

  get ProfitRatio(): number {
    return this.totalRevenues === 0 ? 0 : (this.Profit / this.totalRevenues);
  }

  newRevenue(): void {
    this.revenueFormDialog = this.dialog.open(RevenuesFormComponent, {
      panelClass: 'revenue-form-dialog',
      data: {
        action: 'add'
      }
    });

    this.revenueFormDialog.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        const revenue = new RevenueBaseModel(response.getRawValue());
        revenue.buildingId = this.contractReportDetail.buildingId;
        revenue.contractId = this.contractReportDetail.id;
        revenue.customerId = this.contractReportDetail.customerId;
        this.revenueService.createElement(revenue)
          .then(
            (createdProposal) => {
              this.snackBar.open('Revenue created successfully!!!', 'close', { duration: 1000 });
              this.contractReportBalanceService.getDetails(this.contractReportDetail.id);
              this.dsContractExpenses = this.contractReportDetail.expenses;
              this.cargarLista();
            },
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });
  }

  newExpense(): void {
    this.expenseFormDialog = this.dialog.open(ExpensesFormComponent, {
      panelClass: 'expense-form-dialog',
      data: {
        action: 'add'
      }
    });

    this.expenseFormDialog.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        const expense = new ExpenseBaseModel(response.getRawValue());
        expense.buildingId = this.contractReportDetail.buildingId;
        expense.contractId = this.contractReportDetail.id;
        expense.customerId = this.contractReportDetail.customerId;
        this.expenseService.createElement(expense)
          .then(
            (createdProposal) => {
              this.snackBar.open('Revenue created successfully!!!', 'close', { duration: 1000 });
              this.contractReportBalanceService.getDetails(this.contractReportDetail.id);
              this.dsContractExpenses = this.contractReportDetail.expenses;
              this.cargarLista();
            },
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });
  }

  updateRevenueItem(item): void {
    this.revenueFormDialog = this.dialog.open(RevenuesFormComponent, {
      panelClass: 'revenue-form-dialog',
      data: {
        action: 'edit',
        item: item
      }
    });


    this.revenueFormDialog.afterClosed()
      .subscribe((revenueForm: FormGroup) => {
        if (!revenueForm) {
          return;
        }
        const RevenueToUpdate = new RevenueBaseModel(revenueForm.getRawValue());
        this.revenueService.loadingSubject.next(true);
        this.revenueService.updateElement(RevenueToUpdate)
          .then(
            () => {
              this.revenueService.loadingSubject.next(false);
              this.contractReportBalanceService.getDetails(this.contractReportDetail.id);
              this.dsContractExpenses = this.contractReportDetail.expenses;
              this.cargarLista();
              this.snackBar.open('revenue updated successfully!!!', 'close', { duration: 1000 });
            },
            () => {
              this.revenueService.loadingSubject.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            })
          .catch(() => {
            this.revenueService.loadingSubject.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });

      });
  }

  updateExpenseItem(item): void {
    this.expenseFormDialog = this.dialog.open(ExpensesFormComponent, {
      panelClass: 'expense-form-dialog',
      data: {
        action: 'edit',
        item: item
      }
    });

    this.expenseFormDialog.afterClosed()
      .subscribe((expenseForm: FormGroup) => {
        if (!expenseForm) {
          return;
        }
        const expenseToUpdate = new ExpenseBaseModel(expenseForm.getRawValue());
        expenseToUpdate.buildingId = this.contractReportDetail.buildingId;
        expenseToUpdate.contractId = this.contractReportDetail.id;
        expenseToUpdate.customerId = this.contractReportDetail.customerId;
        this.expenseService.loadingSubject.next(true);
        this.expenseService.updateElement(expenseToUpdate)
          .then(
            () => {
              this.expenseService.loadingSubject.next(false);
              this.contractReportBalanceService.getDetails(expenseToUpdate.contractId);
              this.dsContractExpenses = this.contractReportDetail.expenses;
              this.cargarLista();
              this.snackBar.open('expense updated successfully!!!', 'close', { duration: 1000 });
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

  getLaborExpenses(): void {
    this.laborExpenses = this.dsContractExpenses.filter(e => e.type === 0);
    this.hasLaborExpenses = this.laborExpenses.length > 0 ? true : false;
  }

  getEquipmentsExpenses(): void {
    this.EquipmentsExpenses = this.dsContractExpenses.filter(e => e.type === 1);
    this.hasEquipmentsExpenses = this.EquipmentsExpenses.length > 0 ? true : false;
  }

  getSuppliesExpenses(): void {
    this.SuppliesExpenses = this.dsContractExpenses.filter(e => e.type === 2);
    this.hasSuppliesExpenses = this.SuppliesExpenses.length > 0 ? true : false;
  }

  getOthersExpenses(): void {
    this.OthersExpenses = this.dsContractExpenses.filter(e => e.type === 3);
    this.hasOthersExpenses = this.OthersExpenses.length > 0 ? true : false;
  }

  getSubcontractorExpenses(): void {
    this.SubcontractorExpenses = this.dsContractExpenses.filter(e => e.type === 4);
    this.hasSubcontractorExpenses = this.SubcontractorExpenses.length > 0 ? true : false;
  }

  findByDates(): void {
    this.contractReportBalanceService.getDetails(this.contractReportDetail.id);
  }

    /**
     * Toggle sidebar
     *
     * @param name
     */
    toggleSidebar(name): void
    {
        this._fuseSidebarService.getSidebar(name).toggleOpen();
    }

}
