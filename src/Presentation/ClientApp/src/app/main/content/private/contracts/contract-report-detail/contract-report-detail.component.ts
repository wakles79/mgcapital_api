import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ContractActivityLogGridModel } from '@app/core/models/contract/contract-activity-log-grid.model';
import { ContractNoteGridModel } from '@app/core/models/contract/contract-note-grid.model';
import { ContractReportDetailsModel } from '@app/core/models/contract/contract-report-detail.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { BehaviorSubject, Subscription } from 'rxjs';
import { SettingsService } from '../../settings/settings.service';
import { ContractsService } from '../contracts.service';
import { DeleteItemConfirmDialogComponent } from '../delete-item-confirm-dialog/delete-item-confirm-dialog.component';
import { EditContractFormComponent } from '../edit-contract-form/edit-contract-form.component';
import { OfficeSpaceFormComponent } from '../office-space-form/office-space-form.component';
import { ContractReportDetailService } from './contract-report-detail.service';
import { Location } from '@angular/common';
import { CompanySettingsBaseModel } from '@app/core/models/company-settings/company-settings-base.model';
import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';
import { FormGroup } from '@angular/forms';
import { ContractOfficeSpaceModel } from '@app/core/models/contract/contract-office-space.model';
import { ContractItemFormComponent } from '../contract-form/contract-item-form/contract-item-form.component';
import { ContractItemGridModel } from '@app/core/models/contract-item/contract-item-grid.model';
import { ContractExpenseFormComponent } from '../contract-form/contract-expense-form/contract-expense-form.component';
import { ContractExpenseGridModel } from '@app/core/models/contract-expense/contract-expense-grid.model';
import { ShareUrlDialogComponent } from '@app/core/modules/share-url-dialog/share-url-dialog/share-url-dialog.component';
import { ContractNoteBaseModel } from '@app/core/models/contract/contract-note-base.model';

@Component({
  selector: 'app-contract-report-detail',
  templateUrl: './contract-report-detail.component.html',
  styleUrls: ['./contract-report-detail.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ContractReportDetailComponent implements OnInit {

  contractReportDetail: ContractReportDetailsModel;
  contractActivityLog: ContractActivityLogGridModel[] = [];
  contractDataChangedSubscription: Subscription;

  loading$ = new BehaviorSubject<boolean>(false);

  hasLaborExpenses = false;
  hasEquipmentsExpenses = false;
  hasSuppliesExpenses = false;
  hasOtherExpenses = false;

  dialogRef: any;

  contractItemDialog: any;
  contractExpenseDialog: any;

  officeSpaceFormDialog: MatDialogRef<OfficeSpaceFormComponent>;

  dailyProfitFormula: string;
  monthlyProfitFormula: string;
  yearlyProfitFormula: string;

  dailyProfitRatioFormula: string;
  monthlyProfitRatioFormula: string;
  yearlyProfitRatioFormula: string;

  totalExpensesOverheadDailyFormula: string;
  totalExpensesOverheadMonthlyFormula: string;
  totalExpensesOverheadYearlyFormula: string;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;
  deleteRevenueDialogRef: MatDialogRef<DeleteItemConfirmDialogComponent>;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  profitMargin = 0;

  budgetEditFormDialog: MatDialogRef<EditContractFormComponent>;

  note = '';
  notes: ContractNoteGridModel[] = [];

  constructor(
    private contractService: ContractsService,
    private contractReportService: ContractReportDetailService,
    private _companySettingsService: SettingsService,
    private location: Location,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.loading$.next(true);
    this.contractDataChangedSubscription =
      this.contractReportService.onContractReportDetailChanged.subscribe(
        (contractData: any) => {
          this.loading$.next(false);
          this.contractReportDetail = contractData;
          this.getActivityLog(this.contractReportDetail.id);
          this.getNotes();

          if (this.contractReportDetail.contractItems.length > 0) {
            this.calculateRateContractItems(this.contractReportDetail.daysPerMonth === 0 ? 1 : this.contractReportDetail.daysPerMonth);
            this.groupItemType();
          }

          if (this.contractReportDetail.contractExpenses.length > 0) {
            this.calculateRateExpenses(this.contractReportDetail.daysPerMonth === 0 ? 1 : this.contractReportDetail.daysPerMonth);
          }
        }
      );

    this._companySettingsService.loadSettings()
      .then((settings: CompanySettingsBaseModel) => {
        this.profitMargin = settings.minimumProfitMarginPercentage;
      });
  }

  ngOnInit(): void {

    /*
    this.route.queryParamMap.subscribe((map: any) => {
      const action = map.params['action'];

      if (action === 'add') {
        // IN THE FUTURE add an item :)
      }
      else if (action === 'edit') {
        const cleaningReportItemId = map.params['itemId'];
        if (cleaningReportItemId) {
          console.log(cleaningReportItemId);
        }
      }
    });
    */
  }

  goBack(): void {
    this.location.back();
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

  get urlToCopy(): string {
    return window.location.protocol + '//' + window.location.host + '/contracts/contract-report/' + this.contractReportDetail.guid;
  }

  getActivityLog(contractId: number): void {
    this.contractService.getAll('GetActivityLog', null, '', '', 0, 99, { 'id': contractId.toString() })
      .subscribe((response: { count: number, payload: ContractActivityLogGridModel[] }) => {
        this.contractActivityLog = response.payload;
      }, (error) => {
        this.snackBar.open('Oops, there was an error loading activity log.', 'Close', { duration: 1000 });
      });
  }

  editBudget(): void {
    this.loading$.next(true);
    this.contractService.get(this.contractReportDetail.id)
      .subscribe((budget: any) => {
        this.loading$.next(false);

        if (!budget) {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          return;
        }

        const contractUpdate = new ContractBaseModel(budget);
        this.budgetEditFormDialog = this.dialog.open(EditContractFormComponent, {
          panelClass: 'edit-contract-form-dialog',
          data: {
            contract: contractUpdate,
            action: 'edit'
          }
        });

        this.budgetEditFormDialog.afterClosed()
          .subscribe((result: FormGroup) => {
            if (!result) {
              return;
            }

            this.loading$.next(true);
            const contractToUpdate = new ContractBaseModel(result.getRawValue());
            this.contractService.updateElement(contractToUpdate)
              .then(
                () => {
                  this.loading$.next(false);
                  this.snackBar.open('Contract updated successfully!!!', 'Close', { duration: 1000 });
                  this.contractReportService.getDetails(this.contractReportDetail.id);
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

  /** OFFICE SPACES */
  editOfficeSpace(officeSpace): void {
    let addedTypes = '';
    this.contractReportDetail.officeSpaces.forEach((i: ContractOfficeSpaceModel) => {

      if (i.officeTypeId !== officeSpace.officeTypeId) {
        addedTypes += (addedTypes ? ',' + i.officeTypeId : i.officeTypeId.toString());
      }

    });

    this.loading$.next(true);
    this.contractService.getOfficeSpace(officeSpace.id).subscribe(
      (response: ContractOfficeSpaceModel) => {

        this.loading$.next(false);
        this.officeSpaceFormDialog = this.dialog.open(OfficeSpaceFormComponent, {
          panelClass: 'office-space-form-dialog',
          data: {
            action: 'edit',
            addedTypes: addedTypes,
            officeSpace: response
          }
        });

        this.officeSpaceFormDialog.afterClosed()
          .subscribe((responseForm: FormGroup) => {
            if (!response) {
              return;
            }

            this.loading$.next(true);
            const officeSpaceUpdated = new ContractOfficeSpaceModel(responseForm.getRawValue());
            officeSpaceUpdated.contractId = this.contractReportDetail.id;
            this.contractService.updateOfficeSpace(officeSpaceUpdated)
              .subscribe(() => {
                this.contractReportService.getDetails(this.contractReportDetail.id);
                this.loading$.next(false);
              }, (error) => {
                this.loading$.next(false);
                this.snackBar.open('Ops! Error when trying to update office space', 'Close');
              });

          });
      }, (error) => {
        this.loading$.next(false);
        this.snackBar.open('Ops! Error when trying to get office space', 'Close');
      });
  }

  get occupiedSquareFeet(): number {
    let total = 0;
    this.contractReportDetail.officeSpaces.forEach((item: ContractOfficeSpaceModel) => {
      total += item.squareFeet;
    });
    return total;
  }

  get totalSquareFeet(): number {
    return this.occupiedSquareFeet + this.contractReportDetail.unoccupiedSquareFeets;
  }

  /** CONTRACT ITEMS */
  newContractItem(): void {
    this.contractItemDialog = this.dialog.open(ContractItemFormComponent, {
      panelClass: 'contract-item-form-dialog',
      data: {
        action: 'new',
        daysPerMonth: (Number(this.contractReportDetail.daysPerMonth))
      }
    });

    this.contractItemDialog.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        this.loading$.next(true);
        const contractItem = new ContractItemGridModel(response.getRawValue());
        contractItem.id = 0;
        contractItem.contractId = this.contractReportDetail.id;
        this.contractService.saveContractItem(contractItem)
          .subscribe(
            () => {
              this.snackBar.open('Contract item created successfully!!!', 'Close', { duration: 1000 });
              this.contractReportService.getDetails(this.contractReportDetail.id);
            },
            (error) => { this.snackBar.open('Oops, there was an error', 'Close', { duration: 1000 }); }
          );
      });
  }

  updateContractItem(item): void {
    this.contractItemDialog = this.dialog.open(ContractItemFormComponent, {
      panelClass: 'contract-item-form-dialog',
      data: {
        action: 'edit',
        contractItem: item,
        daysPerMonth: (Number(this.contractReportDetail.daysPerMonth))
      }
    });


    this.contractItemDialog.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        this.loading$.next(true);
        const contractItem = new ContractItemGridModel(response.getRawValue());
        contractItem.contractId = this.contractReportDetail.id;
        this.contractService.updateContractItem(contractItem)
          .subscribe(
            () => {
              this.snackBar.open('Contract item created successfully!!!', 'Close', { duration: 1000 });
              this.contractReportService.getDetails(this.contractReportDetail.id);
            },
            (error) => { this.snackBar.open('Oops, there was an error', 'Close', { duration: 1000 }); }
          );
      });
    this.loading$.next(false);
  }

  deleteContractItem(id: number): void {
    this.deleteRevenueDialogRef = this.dialog.open(DeleteItemConfirmDialogComponent, {
      disableClose: false
    });

    this.deleteRevenueDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete this row?';

    this.deleteRevenueDialogRef.afterClosed().subscribe((result: { response: boolean, updatePrepopulatedItems: boolean }) => {
      if (result.response) {
        this.contractService.deleteContractItem(id, result.updatePrepopulatedItems)
          .then(() => {
            this.snackBar.open('Contract item deleted successfully!!!', 'Close', { duration: 1000 });
            this.contractReportService.getDetails(this.contractReportDetail.id);
          })
          .catch(() => {
            this.snackBar.open('Oops, there was an error', 'Close', { duration: 1000 });
          });
      }
      this.deleteRevenueDialogRef = null;
    }
    );
  }

  get totalDailyRevenue(): number {
    return this.contractReportDetail ? this.contractReportDetail.contractItems.map(i => i.dailyRate).reduce((acc, value) => acc + value, 0) : 0;
  }

  get totalMonthlyRevenue(): number {
    return this.contractReportDetail ? this.contractReportDetail.contractItems.map(i => i.monthlyRate).reduce((acc, value) => acc + value, 0) : 0;
  }

  get totalYearlyRevenue(): number {
    return this.contractReportDetail ? this.contractReportDetail.contractItems.map(i => i.yearlyRate).reduce((acc, value) => acc + value, 0) : 0;
  }

  groupItemType(): void {
    try {
      const os: ContractOfficeSpaceModel[] = [];

      this.contractReportDetail.contractItems.filter(i => i.rateType === 3).forEach(item => {
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

      this.contractReportDetail.officeSpaces = os;
    } catch (e) {
      console.log(e);
    }
  }

  /** EXPENSES */
  newContractExpense(): void {
    this.contractExpenseDialog = this.dialog.open(ContractExpenseFormComponent, {
      panelClass: 'contract-expense-form-dialog',
      data: {
        action: 'new',
        daysPerMonth: (Number(this.contractReportDetail.daysPerMonth))
      }
    });

    this.contractExpenseDialog.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        this.loading$.next(true);
        const expense = new ContractExpenseGridModel(response.getRawValue());
        expense.id = 0;
        expense.contractId = this.contractReportDetail.id;
        this.contractService.saveContractExpense(expense)
          .subscribe(
            () => {
              this.snackBar.open('Contract expense created successfully!!!', 'Close', { duration: 1000 });
              this.contractReportService.getDetails(this.contractReportDetail.id);
            },
            (error) => { this.snackBar.open('Oops, there was an error', 'Close', { duration: 1000 }); }
          );
      });
  }

  updateContractExpenses(item): void {
    this.contractExpenseDialog = this.dialog.open(ContractExpenseFormComponent, {
      panelClass: 'contract-expense-form-dialog',
      data: {
        action: 'edit',
        contractExpense: item,
        daysPerMonth: (Number(this.contractReportDetail.daysPerMonth))
      }
    });

    this.contractExpenseDialog.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        this.loading$.next(true);
        const expense = new ContractExpenseGridModel(response.getRawValue());
        expense.contractId = this.contractReportDetail.id;
        this.contractService.updateContractExpense(expense)
          .subscribe(
            () => {
              this.snackBar.open('Contract expense created successfully!!!', 'Close', { duration: 1000 });
              this.contractReportService.getDetails(this.contractReportDetail.id);
            },
            (error) => { this.snackBar.open('Oops, there was an error', 'Close', { duration: 1000 }); }
          );
      });

  }

  deleteContractExpense(id: number): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete this row?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.contractService.deleteContractExpense(id)
          .then(() => {
            this.snackBar.open('Contract expense deleted successfully!!!', 'Close', { duration: 1000 });
            this.contractReportService.getDetails(this.contractReportDetail.id);
          })
          .catch(() => {
            this.snackBar.open('Oops, there was an error', 'Close', { duration: 1000 });
          });
      }
      this.confirmDialogRef = null;
    }
    );
  }

  get subTotalDailyExpenses(): number {
    return this.contractReportDetail ? this.contractReportDetail.contractExpenses.map(i => i.dailyRate).reduce((acc, value) => acc + value, 0) : 0;
  }
  get totalDailyExpenses(): number {
    return this.subTotalDailyExpenses + this.totalExpensesOverheadDaily;
  }

  get subTotalMonthlyExpenses(): number {
    return this.contractReportDetail ? this.contractReportDetail.contractExpenses.map(i => i.monthlyRate).reduce((acc, value) => acc + value, 0) : 0;
  }
  get totalMonthlyExpenses(): number {
    return this.subTotalMonthlyExpenses + this.totalExpensesOverheadMonthly;
  }

  get subTotalYearlyExpenses(): number {
    return this.contractReportDetail ? this.contractReportDetail.contractExpenses.map(i => i.yearlyRate).reduce((acc, value) => acc + value, 0) : 0;
  }
  get totalYearlyExpenses(): number {
    return this.subTotalYearlyExpenses + this.totalExpensesOverheadYearly;
  }

  getLaborExpenses(): ContractExpenseGridModel[] {
    const laborExpenses = this.contractReportDetail.contractExpenses.filter(e => e.expenseCategory === 0);
    this.hasLaborExpenses = laborExpenses.length > 0 ? true : false;
    return laborExpenses;
  }

  getEquipmentsExpenses(): ContractExpenseGridModel[] {
    const equipmentExpenses = this.contractReportDetail.contractExpenses.filter(e => e.expenseCategory === 1);
    this.hasEquipmentsExpenses = equipmentExpenses.length > 0 ? true : false;
    return equipmentExpenses;
  }

  getSuppliesExpenses(): ContractExpenseGridModel[] {
    const suppliesExpenses = this.contractReportDetail.contractExpenses.filter(e => e.expenseCategory === 2);
    this.hasSuppliesExpenses = suppliesExpenses.length > 0 ? true : false;
    return suppliesExpenses;
  }

  getOtherExpenses(): ContractExpenseGridModel[] {
    const otherExpenses = this.contractReportDetail.contractExpenses.filter(e => e.expenseCategory === 3);
    this.hasOtherExpenses = otherExpenses.length > 0 ? true : false;
    return otherExpenses;
  }

  /** BUTTONS */
  sendContractByEmailContract(): void {

  }

  shareContractDocument(): void {
    this.dialogRef = this.dialog.open(ShareUrlDialogComponent, {
      panelClass: 'share-url-form-dialog',
      data: {
        urlToCopy: this.urlToCopy
      }
    });
  }

  openPublicContractViewNewTab(): void {
    window.open(this.urlToCopy, '_blank');
  }

  /** MATH */
  calculateRateContractItems(daysMonth: number): void {
    let value = 0;
    for (const cItem of this.contractReportDetail.contractItems) {
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
          cItem.dailyRateFormula = '((' + value.toFixed(2) + '(Value) * ' + cItem.rate + '(Rate)) * ' + cItem.quantity + '(Quantity))';

          cItem.monthlyRate = (cItem.dailyRate * daysMonth);
          cItem.monthlyRateFormula = '(' + cItem.dailyRate + '(Daily Rate) * ' + daysMonth + '(Days per Month))';

          cItem.yearlyRate = (cItem.monthlyRate * 12);
          cItem.yearlyRateFormula = '(' + cItem.monthlyRate.toFixed(2) + '(Monthly Rate) * 12)';
          break;

        case 'Monthly':
          cItem.monthlyRate = (value * cItem.rate) * cItem.quantity;
          cItem.monthlyRateFormula = '((' + value.toFixed(2) + '(Value) * ' + cItem.rate + '(Rate)) * ' + cItem.quantity + '(Quantity))';

          cItem.dailyRate = (cItem.monthlyRate / daysMonth);
          cItem.dailyRateFormula = '(' + cItem.monthlyRate.toFixed(2) + '(Monthly Rate) / ' + daysMonth + '(Days per Month))';


          cItem.yearlyRate = (cItem.monthlyRate * 12);
          cItem.yearlyRateFormula = '(' + cItem.monthlyRate.toFixed(2) + '(Monthly Rate) * 12)';
          break;

        case 'Bi-Monthly':
          cItem.biMonthlyRate = (value * cItem.rate) * cItem.quantity;
          cItem.biMonthlyRateFormula = '(((Value) * (Rate)) * (Quantity))';

          cItem.monthlyRate = (cItem.biMonthlyRate / 2);
          cItem.monthlyRateFormula = '(' + cItem.biMonthlyRate + '(Bi-Monthly Rate) / 2)';

          cItem.quarterly = (cItem.monthlyRate * 3);
          cItem.quarterlyFormula = '(' + cItem.monthlyRate + '(Monthly Rate) * 3)';

          cItem.biAnnually = (cItem.monthlyRate * 6);
          cItem.biAnnuallyFormula = '(' + cItem.monthlyRate + '(Monthly Rate) * 6)';

          cItem.dailyRate = (cItem.monthlyRate / daysMonth);
          cItem.dailyRateFormula = '(' + cItem.monthlyRate + '(Monthly Rate) / ' + daysMonth + ' (Days per Month))';

          cItem.yearlyRate = (cItem.monthlyRate * 12);
          cItem.yearlyRateFormula = '(' + cItem.monthlyRate + '(Monthly Rate) * 12)';
          break;

        case 'Quarterly':
          cItem.quarterly = (value * cItem.rate) * cItem.quantity;
          cItem.quarterlyFormula = '(((Value) * (Rate)) * (Quantity))';

          cItem.monthlyRate = (cItem.quarterly / 3);
          cItem.monthlyRateFormula = '(' + cItem.quarterly + '(Quarterly Rate) / 3)';

          cItem.biMonthlyRate = (cItem.monthlyRate * 2);
          cItem.biMonthlyRateFormula = '(' + cItem.monthlyRate + '(Monthly Rate) * 2)';

          cItem.biAnnually = (cItem.monthlyRate * 6);
          cItem.biAnnuallyFormula = '(' + cItem.monthlyRate + '(Monthly Rate) * 6)';

          cItem.dailyRate = (cItem.monthlyRate / daysMonth);
          cItem.dailyRateFormula = '(' + cItem.monthlyRate + '(Monthly Rate) / ' + daysMonth + '(Days per Month))';

          cItem.yearlyRate = (cItem.monthlyRate * 12);
          cItem.yearlyRateFormula = '(' + cItem.monthlyRate + '(Monthly Rate) / 12';
          break;

        case 'Bi-Annually':
          cItem.biAnnually = (value * cItem.rate) * cItem.quantity;
          cItem.biAnnuallyFormula = '(((Value) * (Rate)) * (Quantity))';

          cItem.monthlyRate = (cItem.biAnnually / 6);
          cItem.monthlyRateFormula = '(' + cItem.biAnnually + '(Bi-Anually Rate) / 6)';

          cItem.biMonthlyRate = (cItem.monthlyRate * 2);
          cItem.biMonthlyRateFormula = '(' + cItem.monthlyRate + '(Monthly Rate) * 2)';

          cItem.quarterly = (cItem.monthlyRate * 3);
          cItem.quarterlyFormula = '(' + cItem.monthlyRate + '(Monthly Rate) * 3)';

          cItem.dailyRate = (cItem.monthlyRate / daysMonth);
          cItem.dailyRateFormula = '(' + cItem.monthlyRate + '(Monthly Rate) / ' + daysMonth + '(Days per Month))';

          cItem.yearlyRate = (cItem.monthlyRate * 12);
          cItem.yearlyRateFormula = '(' + cItem.monthlyRate + '(Monthly Rate) * 12)';
          break;

        case 'Yearly':
          cItem.yearlyRate = (value * cItem.rate) * cItem.quantity;
          cItem.yearlyRateFormula = '((' + value.toFixed(2) + '(Value) * ' + cItem.rate + '(Rate)) * ' + cItem.quantity + '(Quantity))';

          cItem.monthlyRate = (cItem.yearlyRate / 12);
          cItem.monthlyRateFormula = '((' + cItem.yearlyRate.toFixed(2) + '(Yearly Rate) / 12))';

          cItem.dailyRate = (cItem.monthlyRate / daysMonth);
          cItem.dailyRateFormula = '(' + cItem.monthlyRate.toFixed(2) + '(Monthly Rate) / ' + daysMonth + '(Days per Month))';

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
    let cleanRate = 0;
    let rateText: string;

    for (const cExpense of this.contractReportDetail.contractExpenses) {
      value = cExpense.value;
      rate = cExpense.rate;
      cleanRate = cExpense.rate;
      rateText = '';

      cExpense.taxesAndInsurance = cExpense.overheadPercent === 0 ? 0 : (cExpense.overheadPercent / 100) * cExpense.rate;

      if (cExpense.expenseCategory === 0) {
        rate = rate + (rate * (cExpense.overheadPercent / 100));
        rateText = ` + ${cExpense.overheadPercent}%`;
      }

      switch (cExpense.ratePeriodicity) {
        case 'Daily':
          cExpense.dailyRate = (value * rate) * cExpense.quantity;
          cExpense.dailyRateFormula = `((${value.toFixed(2)}(Value) * ${rate.toFixed(2)}(Rate${rateText})) * ${cExpense.quantity}(Quantity)`;
          // cExpense.dailyRateFormula = '(((Value)  * (Rate)) * (Quantity))';

          cExpense.monthlyRate = (cExpense.dailyRate * daysMonth);
          cExpense.monthlyRateFormula = `(${cExpense.dailyRate.toFixed(2)}(Daily Rate) * ${daysMonth}(Days per Month))`;
          // cExpense.monthlyRateFormula = '((Daily Rate) * (Days per Month))';

          cExpense.yearlyRate = (cExpense.monthlyRate * 12);
          cExpense.yearlyRateFormula = `${cExpense.monthlyRate.toFixed(2)}(Monthly Rate) * 12`;
          // cExpense.yearlyRateFormula = '((Monthly Rate) * 12)';

          if (cExpense.expenseCategory === 0) {
            cExpense.dailyTaxRate = (value * cleanRate) * cExpense.quantity;
          }
          break;

        case 'Monthly':
          cExpense.monthlyRate = (value * rate) * cExpense.quantity;
          cExpense.monthlyRateFormula = `(${value.toFixed(2)}(Value) * ${cExpense.rate.toFixed(2)}(Rate${rateText}) * ${cExpense.quantity}(Quantity))`;
          // cExpense.monthlyRateFormula = '(((Value) * (Rate)) * (Quantity))';

          cExpense.dailyRate = (cExpense.monthlyRate / daysMonth);
          cExpense.dailyRateFormula = `(${cExpense.monthlyRate.toFixed(2)}(Monthly Rate) / ${daysMonth}(Days per Month))`;
          // cExpense.dailyRateFormula = '((Monthly Rate) / (Days per Month))';

          cExpense.yearlyRate = (cExpense.monthlyRate * 12);
          cExpense.yearlyRateFormula = `(${cExpense.monthlyRate.toFixed(2)}(Monthly Rate) * 12)`;
          // cExpense.yearlyRateFormula = '((Monthly Rate) * 12)';

          if (cExpense.expenseCategory === 0) {
            cExpense.dailyTaxRate = ((value * cleanRate) * cExpense.quantity) / daysMonth;
          }
          break;

        case 'Yearly':
          cExpense.yearlyRate = (value * rate) * cExpense.quantity;
          cExpense.yearlyRateFormula = `(${value.toFixed(2)}(Value) * ${cExpense.rate.toFixed(2)}(Rate${rateText}) * ${cExpense.quantity}(Quantity))`;
          // cExpense.yearlyRateFormula = '(((Value) * (Rate)) * (Quantity))';

          cExpense.monthlyRate = (cExpense.yearlyRate / 12);
          cExpense.monthlyRateFormula = `(${cExpense.yearlyRate.toFixed(2)}(Yearly Rate) / 12)`;
          // cExpense.monthlyRateFormula = '(((Yearly Rate) / 12))';

          cExpense.dailyRate = (cExpense.monthlyRate / daysMonth);
          cExpense.dailyRateFormula = `(${cExpense.monthlyRate.toFixed(2)}(Monthly Rate) / ${daysMonth}(Days per Month))`;
          // cExpense.dailyRateFormula = '((Monthly Rate) / (Days per Month))';

          if (cExpense.expenseCategory === 0) {
            cExpense.dailyTaxRate = (((value * cleanRate) * cExpense.quantity) / 12) / daysMonth;
          }
          break;
      }
    }
  }

  get dailyProfit(): number {
    this.dailyProfitFormula = this.totalDailyRevenue.toFixed(2) + '(Total Daily Revenue) - ' + this.totalDailyExpenses.toFixed(2) + '(Total Daily Expenses)';
    return this.contractReportDetail ? (this.totalDailyRevenue - this.totalDailyExpenses) : 0;
  }

  get monthlyProfit(): number {
    this.monthlyProfitFormula = this.totalMonthlyRevenue.toFixed(2) + '(Total Monthly Revenue) - ' + this.totalMonthlyExpenses.toFixed(2) + '(Total Monthly Expenses)';
    return this.contractReportDetail ? (this.totalMonthlyRevenue - this.totalMonthlyExpenses) : 0;
  }

  get yearlyProfit(): number {
    this.yearlyProfitFormula = this.totalYearlyRevenue.toFixed(2) + '(Total Yearly Revenue) - ' + this.totalYearlyExpenses.toFixed(2) + '(Total Yearly Expenses)';
    return this.contractReportDetail ? (this.totalYearlyRevenue - this.totalYearlyExpenses) : 0;
  }

  get dailyProfitRatio(): number {
    this.dailyProfitRatioFormula = this.dailyProfit.toFixed(2) + '(Total Daily Profit) / ' + this.totalDailyRevenue.toFixed(2) + '(Total Daily Revenue)';
    return this.totalDailyRevenue === 0 ? 0 : (this.dailyProfit / this.totalDailyRevenue);
  }

  get monthlyProfitRatio(): number {
    this.monthlyProfitRatioFormula = this.monthlyProfit.toFixed(2) + '(Total Monthly Profit) / ' + this.totalMonthlyRevenue.toFixed(2) + '(Total Monthly Revenue)';
    return this.totalMonthlyRevenue === 0 ? 0 : (this.monthlyProfit / this.totalMonthlyRevenue);
  }

  get yearlyProfitRatio(): number {
    this.yearlyProfitRatioFormula = this.yearlyProfit + '(Total Yearly Profit) / ' + this.totalYearlyRevenue + '(Total Yearly Revenue)';
    return this.totalYearlyRevenue === 0 ? 0 : (this.yearlyProfit / this.totalYearlyRevenue);
  }

  get totalExpensesOverheadDaily(): number {
    const total = this.getLaborExpenses().map(i => i.dailyRate).reduce((acc, value) => acc + value, 0);
    this.totalExpensesOverheadDailyFormula = total.toFixed(2) + ' (Total Daily Labor Expenses) * 14% (Percentage)';
    return total * 0.14;
  }

  get totalExpensesOverheadMonthly(): number {
    const total = this.getLaborExpenses().map(i => i.monthlyRate).reduce((acc, value) => acc + value, 0);
    this.totalExpensesOverheadMonthlyFormula = total.toFixed(2) + ' (Total Monthly Labor Expenses) * 14% (Percentage)';
    return total * 0.14;
  }

  get totalExpensesOverheadYearly(): number {
    const total = this.getLaborExpenses().map(i => i.yearlyRate).reduce((acc, value) => acc + value, 0);
    this.totalExpensesOverheadYearlyFormula = total.toFixed(2) + ' (Total Yearly Labor Expenses) * 14% (Percentage)';
    return total * 0.14;
  }

  /** LOG */
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

  // PDF
  getDocumentUrl(): void {
    this.loading$.next(true);
    this.contractService.get(this.contractReportDetail.id, 'GetBudgetPDFDocumentUrl')
      .subscribe((response: string) => {
        this.loading$.next(false);
        window.open(response, '_blank');
      }, (error) => {
        this.loading$.next(false);
        this.snackBar.open('Oops, there was an error: ' + error, 'close', { duration: 1000 });
      });
  }

  // Notes
  addNote(): void {
    if (this.note.trim()) {
      const newNote = new ContractNoteBaseModel(
        {
          id: 0,
          note: this.note.trim(),
          contractId: this.contractReportDetail.id,
          employeeId: 0,
          epochCreatedDate: 0
        });

      this.loading$.next(true);
      this.contractReportService.addContractNote(newNote)
        .subscribe(() => {
          this.snackBar.open('Note Added Successfully', 'close', { duration: 1000 });
          this.getNotes();
        }, (error) => {
          this.loading$.next(false);
          this.snackBar.open('Oops, there was an error: ' + error, 'close', { duration: 1000 });
        });

    } else {
      this.snackBar.open('Enter valid text', 'close', { duration: 1000 });
    }
  }

  getNotes(): void {
    this.contractReportService.getContractNotes(this.contractReportDetail.id)
      .subscribe((result: ContractNoteGridModel[]) => {
        this.notes = result;
        this.loading$.next(false);
      }, (error) => {
        this.loading$.next(false);
        this.snackBar.open('Oops, there was an error: ' + error, 'close', { duration: 1000 });
      });
  }

}
