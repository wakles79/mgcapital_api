import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ContractReportDetailsModel } from '@app/core/models/contract/contract-report-detail.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfigService } from '@fuse/services/config.service';
import { BehaviorSubject, Subscription } from 'rxjs';
import { ContractReportDetailService } from '../../private/contracts/contract-report-detail/contract-report-detail.service';
import { ContractExpenseGridModel } from '@app/core/models/contract-expense/contract-expense-grid.model';
import { ContractOfficeSpaceModel } from '@app/core/models/contract/contract-office-space.model';

@Component({
  selector: 'app-public-contract-report',
  templateUrl: './public-contract-report.component.html',
  styleUrls: ['./public-contract-report.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class PublicContractReportComponent implements OnInit {

  contractDataChangedSubscription: Subscription;
  contractDetail: ContractReportDetailsModel;

  loading$ = new BehaviorSubject<boolean>(true);

  hasLaborExpenses = false;
  hasEquipmentsExpenses = false;
  hasSuppliesExpenses = false;
  hasOtherExpenses = false;

  constructor(
    private fuseConfig: FuseConfigService,
    private contractService: ContractReportDetailService
  ) {

    this.fuseConfig.config = {
      layout: {
        navbar: {
          hidden: true
        },
        toolbar: {
          hidden: true
        },
        footer: {
          hidden: true
        },
        sidepanel: {
          hidden: true
        }
      }
    };

    this.contractDataChangedSubscription = this.contractService.onContractReportDetailChanged
      .subscribe((contractData: any) => {
        this.loading$.next(false);
        this.contractDetail = contractData;

        if (this.contractDetail.contractItems.length > 0) {
          this.calculateRateContractItems(this.contractDetail.daysPerMonth);
        }

        if (this.contractDetail.contractExpenses.length > 0) {
          this.calculateRateExpenses(this.contractDetail.daysPerMonth);
        }

      });
  }

  ngOnInit(): void {
  }

  /** CONTRACT */
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

  /** REVENUE */
  get totalDailyRevenue(): number {
    return this.contractDetail ? this.contractDetail.contractItems.map(i => i.dailyRate).reduce((acc, value) => acc + value, 0) : 0;
  }

  get totalMonthlyRevenue(): number {
    return this.contractDetail ? this.contractDetail.contractItems.map(i => i.monthlyRate).reduce((acc, value) => acc + value, 0) : 0;
  }

  get totalYearlyRevenue(): number {
    return this.contractDetail ? this.contractDetail.contractItems.map(i => i.yearlyRate).reduce((acc, value) => acc + value, 0) : 0;
  }

  /** EXPENSES */
  get subTotalDailyExpenses(): number {
    return this.contractDetail ? this.contractDetail.contractExpenses.map(i => i.dailyRate).reduce((acc, value) => acc + value, 0) : 0;
  }
  get totalDailyExpenses(): number {
    return this.subTotalDailyExpenses + this.totalExpensesOverheadDaily;
  }

  get subTotalMonthlyExpenses(): number {
    return this.contractDetail ? this.contractDetail.contractExpenses.map(i => i.monthlyRate).reduce((acc, value) => acc + value, 0) : 0;
  }
  get totalMonthlyExpenses(): number {
    return this.subTotalMonthlyExpenses + this.totalExpensesOverheadMonthly;
  }

  get subTotalYearlyExpenses(): number {
    return this.contractDetail ? this.contractDetail.contractExpenses.map(i => i.yearlyRate).reduce((acc, value) => acc + value, 0) : 0;
  }
  get totalYearlyExpenses(): number {
    return this.subTotalYearlyExpenses + this.totalExpensesOverheadYearly;
  }

  getLaborExpenses(): ContractExpenseGridModel[] {
    const laborExpenses = this.contractDetail.contractExpenses.filter(e => e.expenseCategory === 0);
    this.hasLaborExpenses = laborExpenses.length > 0 ? true : false;
    return laborExpenses;
  }

  getEquipmentsExpenses(): ContractExpenseGridModel[] {
    const equipmentExpenses = this.contractDetail.contractExpenses.filter(e => e.expenseCategory === 1);
    this.hasEquipmentsExpenses = equipmentExpenses.length > 0 ? true : false;
    return equipmentExpenses;
  }

  getSuppliesExpenses(): ContractExpenseGridModel[] {
    const suppliesExpenses = this.contractDetail.contractExpenses.filter(e => e.expenseCategory === 2);
    this.hasSuppliesExpenses = suppliesExpenses.length > 0 ? true : false;
    return suppliesExpenses;
  }

  getOtherExpenses(): ContractExpenseGridModel[] {
    const otherExpenses = this.contractDetail.contractExpenses.filter(e => e.expenseCategory === 3);
    this.hasOtherExpenses = otherExpenses.length > 0 ? true : false;
    return otherExpenses;
  }

  /** OFFICE TYPE */
  get occupiedSquareFeet(): number {
    let total = 0;
    this.contractDetail.officeSpaces.forEach((item: ContractOfficeSpaceModel) => {
      total += item.squareFeet;
    });
    return total;
  }
  get totalSquareFeet(): number {
    return this.occupiedSquareFeet + this.contractDetail.unoccupiedSquareFeets;
  }

  /** MATH */
  calculateRateContractItems(daysMonth: number): void {
    let value = 0;
    for (const cItem of this.contractDetail.contractItems) {
      if (cItem.rateType === 0) {
        value = cItem.hours;
      }
      else if (cItem.rateType === 1) {
        value = 1;
      }
      else if (cItem.rateType === 2) {
        value = cItem.rooms;
      }
      else if (cItem.rateType === 3) {
        value = cItem.squareFeet;
      }

      switch (cItem.ratePeriodicity) {
        case 'Daily':
          cItem.dailyRate = (value * cItem.rate) * cItem.quantity;
          cItem.monthlyRate = (cItem.dailyRate * daysMonth);
          cItem.yearlyRate = (cItem.monthlyRate * 12);
          break;

        case 'Monthly':
          cItem.monthlyRate = (value * cItem.rate) * cItem.quantity;
          cItem.dailyRate = (cItem.monthlyRate / daysMonth);
          cItem.yearlyRate = (cItem.monthlyRate * 12);
          break;

        case 'Yearly':
          cItem.yearlyRate = (value * cItem.rate) * cItem.quantity;
          cItem.monthlyRate = (cItem.yearlyRate / 12);
          cItem.dailyRate = (cItem.monthlyRate / daysMonth);
          break;
      }
    }
  }

  calculateRateExpenses(daysMonth: number): void {
    if (!Number(daysMonth)) {
      return;
    }

    let value = 0;
    let rate = 0;
    for (const cExpense of this.contractDetail.contractExpenses) {
      value = cExpense.value;
      rate = cExpense.rate;

      cExpense.taxesAndInsurance = cExpense.overheadPercent === 0 ? 0 : (cExpense.overheadPercent / 100) * cExpense.rate;

      if (cExpense.expenseCategory === 0) {
        rate = rate + (rate * (cExpense.overheadPercent / 100));
      }

      switch (cExpense.ratePeriodicity) {
        case 'Daily':
          cExpense.dailyRate = (value * rate) * cExpense.quantity;
          cExpense.monthlyRate = (cExpense.dailyRate * daysMonth);
          cExpense.yearlyRate = (cExpense.monthlyRate * 12);
          break;

        case 'Monthly':
          cExpense.monthlyRate = (value * rate) * cExpense.quantity;
          cExpense.dailyRate = (cExpense.monthlyRate / daysMonth);
          cExpense.yearlyRate = (cExpense.monthlyRate * 12);
          break;

        case 'Yearly':
          cExpense.yearlyRate = (value * rate) * cExpense.quantity;
          cExpense.monthlyRate = (cExpense.yearlyRate / 12);
          cExpense.dailyRate = (cExpense.monthlyRate / daysMonth);
          break;
      }
    }
  }

  get dailyProfit(): number {
    return this.contractDetail ? (this.totalDailyRevenue - this.totalDailyExpenses) : 0;
  }

  get monthlyProfit(): number {
    return this.contractDetail ? (this.totalMonthlyRevenue - this.totalMonthlyExpenses) : 0;
  }

  get yearlyProfit(): number {
    return this.contractDetail ? (this.totalYearlyRevenue - this.totalYearlyExpenses) : 0;
  }

  get dailyProfitRatio(): number {
    return this.totalDailyRevenue === 0 ? 0 : (this.dailyProfit / this.totalDailyRevenue);
  }

  get monthlyProfitRatio(): number {
    return this.totalMonthlyRevenue === 0 ? 0 : (this.monthlyProfit / this.totalMonthlyRevenue);
  }

  get yearlyProfitRatio(): number {
    return this.totalYearlyRevenue === 0 ? 0 : (this.yearlyProfit / this.totalYearlyRevenue);
  }

  get totalExpensesOverheadDaily(): number {
    const total = this.getLaborExpenses().map(i => i.dailyRate).reduce((acc, value) => acc + value, 0);
    total.toFixed(2);
    return total * 0.14;
  }
  get totalExpensesOverheadMonthly(): number {
    const total = this.getLaborExpenses().map(i => i.monthlyRate).reduce((acc, value) => acc + value, 0);
    total.toFixed(2);
    return total * 0.14;
  }
  get totalExpensesOverheadYearly(): number {
    const total = this.getLaborExpenses().map(i => i.yearlyRate).reduce((acc, value) => acc + value, 0);
    total.toFixed(2);
    return total * 0.14;
  }

}
