import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ExpenseCSVModel } from '@app/core/models/expense/expense-csv.model';
import { ExpenseGridModel } from '@app/core/models/expense/expense-grid.model';
import { RevenueCSV } from '@app/core/models/revenue/revenue-csv-model';
import { RevenueGridModel } from '@app/core/models/revenue/revenue-grid.model';
import { ImportRevenueExpenseCsvFormComponent } from '@app/core/modules/import-revenue-expense-csv-form/import-revenue-expense-csv-form/import-revenue-expense-csv-form.component';
import { ResultImportCsvComponent } from '@app/core/modules/result-import-csv/result-import-csv/result-import-csv.component';
import { fuseAnimations } from '@fuse/animations';
import { EditContractFormComponent } from './edit-contract-form/edit-contract-form.component';
import { ContractCSVModel } from '@app/core/models/contract/contract-csv.model';
import { ContractItemCSVModel } from '@app/core/models/contract-item/contract-item-csv.model';
import { ContractExpenseCSVModel } from '@app/core/models/contract-expense/contract-expense-csv.model';
import { RevenueAndExpensesCSV } from '@app/core/models/revenue/revenuesAndExpenses';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SettingsService } from '../settings/settings.service';
import { RevenuesService } from '../revenues/revenues.service';
import { ExpensesService } from '../expenses/expenses.service';
import { ContractsService } from './contracts.service';
import { CompanySettingsBaseModel } from '@app/core/models/company-settings/company-settings-base.model';
import { ContractGridModel } from '@app/core/models/contract/contract-grid.model';
import { NewContractFormComponent } from './new-contract-form/new-contract-form.component';
import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';
import { FormGroup } from '@angular/forms';
import * as moment from 'moment';
import { DataSource } from '@angular/cdk/table';
import { Observable } from 'rxjs';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';

@Component({
  selector: 'app-contracts',
  templateUrl: './contracts.component.html',
  styleUrls: ['./contracts.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ContractsComponent implements OnInit {

  dialogRef: any;
  editContractForm: MatDialogRef<EditContractFormComponent>;
  profitMargin: number;
  totalProfitAmount = 0;
  totalProfitRatio = 0;
  dataSource: ContractDataSource | null;

  loading$ = this.contractService.loadingSubject.asObservable();

  // csv importacion
  importCsvDialog: MatDialogRef<ImportRevenueExpenseCsvFormComponent>;
  resultCsvDialog: MatDialogRef<ResultImportCsvComponent>;
  revenues: RevenueGridModel[] = [];
  expenses: ExpenseGridModel[] = [];

  public revenuesCSV: RevenueCSV[] = [];
  public expensesCSV: ExpenseCSVModel[] = [];
  public budgetCSV: ContractCSVModel[] = [];
  public budgetItemCSV: ContractItemCSVModel[] = [];
  public budgetExpenseCSV: ContractExpenseCSVModel[] = [];

  public revenuesIsRepeated: RevenueCSV[] = [];
  public expensesISRepeated: ExpenseCSVModel[] = [];
  items: RevenueAndExpensesCSV[] = [];

  itemResult = [];
  itemResultExpenses = [];
  itemResultRevenues = [];

  resultImportacionCSV = [
    0,
    0,
    0,
    0,
    0,
    0,
    0,
  ];
  resultExpenses = [
    0,
    0,
    0,
    0,
  ];
  resultRevenues = [
    0,
    0,
    0,
    0,
  ];
  resultBudget = [
    0,
    0,
    0,
    0,
    0,
  ];
  resultBudgetItem = [
    0,
    0,
    0,
    0,
    0,
  ];

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private companySettings: SettingsService,
    private revenueService: RevenuesService,
    private expenseService: ExpensesService,
    private contractService: ContractsService,
    private _fuseSidebarService: FuseSidebarService
  ) {
    this.companySettings.loadPublicSettings()
      .then((settings: CompanySettingsBaseModel) => {
        this.profitMargin = settings.minimumProfitMarginPercentage;
      });

    this.contractService.allElementsChanged.subscribe(
      (contractData: ContractGridModel[]) => {
        try {
          this.totalProfitAmount = contractData.map(b => b.monthlyProfit).reduce((acc, value) => acc + value, 0);

          const revenue = contractData.map(b => b.totalMonthlyRevenue).reduce((acc, value) => acc + value, 0);
          const laborExpenses = contractData.map(b => b.totalMonthlyLaborExpense).reduce((acc, value) => acc + value, 0);
          const expenses = contractData.map(b => b.totalMonthlyExpense).reduce((acc, value) => acc + value, 0);
          const totalExpenses = expenses + (laborExpenses * 0.14);
          const monthlyProfit = revenue - totalExpenses;
          this.totalProfitRatio = (revenue === 0 ? 0 : (monthlyProfit / revenue)) * 100;
        } catch (ex) {
          console.log('error');
        }
      }
    );
  }

  ngOnInit(): void {
  }

  newContract(): void {
    this.dialogRef = this.dialog.open(NewContractFormComponent, {
      panelClass: 'new-contract-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.dialogRef.afterClosed()
      .subscribe((response: { message: string, id: number }) => {
        if (!response) {
          return;
        }

        if (response.message === 'success') {
          this.snackBar.open('Contract created successfully!!!', 'Close', { duration: 1000 });

          this.contractService.loadingSubject.next(true);
          this.contractService.get(response.id).subscribe(
            (contract: any) => {
              this.contractService.loadingSubject.next(false);

              if (contract) {
                const contractUpdate = new ContractBaseModel(contract);
                this.editContractForm = this.dialog.open(EditContractFormComponent, {
                  panelClass: 'edit-contract-form-dialog',
                  data: {
                    contract: contractUpdate
                  }
                });

                this.editContractForm.afterClosed().subscribe((result: FormGroup) => {
                  if (!result) {
                    return;
                  }

                  this.contractService.loadingSubject.next(true);
                  const contractToUpdate = new ContractBaseModel(result.getRawValue());
                  this.contractService.updateElement(contractToUpdate)
                    .then(
                      () => {
                        this.contractService.loadingSubject.next(false);
                        this.snackBar.open('Contract updated successfully!!!', 'Close', { duration: 1000 });
                      },
                      () => {
                        this.contractService.loadingSubject.next(false);
                        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                      })
                    .catch(() => {
                      this.contractService.loadingSubject.next(false);
                      this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                    });
                });

              } else {
                this.contractService.loadingSubject.next(false);
                this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
              }
            }, (error) => {
              this.contractService.loadingSubject.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            });

        } else {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      });
  }

  importCsv(): void {
    this.importCsvDialog = this.dialog.open(ImportRevenueExpenseCsvFormComponent, {
      panelClass: 'import-csv-rev-exp-form-dialog',
      data: {
      }
    });

    this.importCsvDialog.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        const TypeDocument = response.get('TypeDocument').value;
        const item = response.get('item').value;
        switch (TypeDocument) {
          case 0:
            this.saveBudget(item);
            break;
          case 1:
            this.saveRevenuesEstimated(item);
            break;
          case 2:
            this.saveExpensesEstimated(item);
            break;
          case 3:
            this.saveRevenuesAndExpensesReal(item);
            break;
          default:
            break;
        }
      });
  }

  async saveExpenses(): Promise<any> {

    this.contractService.loadingSubject.next(true);

    this.resultExpenses[0] = 0;
    this.resultExpenses[1] = 0;
    this.resultExpenses[2] = 0;
    this.resultExpenses[3] = 0;

    if (this.expensesCSV.length === 0) {
      this.saveRevenues();
    } else {
      this.expensesCSV.forEach(async element => {
        await this.expenseService.createElementCSV(element)
          .then(
            async (data) => {
              const key = data['body'];
              switch (key) {
                case '0':
                  this.resultExpenses[0] = this.resultExpenses[0] + 1;
                  break;
                case '1':
                  this.resultExpenses[1] = this.resultExpenses[1] + 1;
                  break;
                case '2':
                  this.resultExpenses[2] = this.resultExpenses[2] + 1;
                  break;
                case '3':
                  this.resultExpenses[3] = this.resultExpenses[3] + 1;
                  break;
                default:
                  break;
              }
              const sum = this.resultExpenses[0] + this.resultExpenses[1] + this.resultExpenses[2] + this.resultExpenses[3];
              if (sum === this.expensesCSV.length) {
                this.contractService.loadingSubject.next(false);
                if (this.revenuesCSV.length === 0) {
                  this.resultCsvDialog = this.dialog.open(ResultImportCsvComponent, {
                    panelClass: 'result-import-csv-form-dialog',
                    data: {
                      result: this.resultExpenses,
                      type: 'budget'
                    }
                  });
                } else {
                  this.saveRevenues();
                }
              }
            },
            () => {
              this.contractService.loadingSubject.next(false);
              this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 });
            })
          .catch(() => {
            this.contractService.loadingSubject.next(false);
            this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 });
          });
      });
    }
  }

  async saveRevenues(): Promise<any> {
    this.contractService.loadingSubject.next(true);

    this.resultRevenues[0] = 0;
    this.resultRevenues[1] = 0;
    this.resultRevenues[2] = 0;
    this.resultRevenues[3] = 0;

    this.revenuesCSV.forEach(async element => {
      await this.revenueService.createElementCSV(element)
        .then(
          async (data) => {
            const key = data['body'];
            switch (key) {
              case '0':
                this.resultRevenues[0] = this.resultRevenues[0] + 1;
                break;
              case '1':
                this.resultRevenues[1] = this.resultRevenues[1] + 1;
                break;
              case '2':
                this.resultRevenues[2] = this.resultRevenues[2] + 1;
                break;
              case '3':
                this.resultRevenues[3] = this.resultRevenues[3] + 1;
                break;
              default:
                break;
            }
            const sum = this.resultRevenues[0] + this.resultRevenues[1] + this.resultRevenues[2] + this.resultRevenues[3];
            if (sum === this.revenuesCSV.length) {

              this.contractService.loadingSubject.next(false);

              if (this.expensesCSV.length === 0) {
                this.resultCsvDialog = this.dialog.open(ResultImportCsvComponent, {
                  panelClass: 'result-import-csv-form-dialog',
                  data: {
                    result: this.resultRevenues,
                    type: 'Revenues&Expenses'
                  }
                });
              } else {
                this.resultRevenues[0] = this.resultRevenues[0] + this.resultExpenses[0];
                this.resultRevenues[1] = this.resultRevenues[1] + this.resultExpenses[1];
                this.resultRevenues[2] = this.resultRevenues[2] + this.resultExpenses[2];
                this.resultRevenues[3] = this.resultRevenues[3] + this.resultExpenses[3];

                this.resultCsvDialog = this.dialog.open(ResultImportCsvComponent, {
                  panelClass: 'result-import-csv-form-dialog',
                  data: {
                    result: this.resultRevenues,
                    type: 'Revenues&Expenses'
                  }
                });
              }

            }
          },
          () => {
            this.contractService.loadingSubject.next(false);
            this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 });
          })
        .catch(() => {
          this.contractService.loadingSubject.next(false);
          this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 });
        });
    });
  }

  saveRevenuesAndExpensesReal(item: any): void {

    this.beginImportCsv();

    for (let index = 0; index < item.length; index++) {

      this.items.push(item[index]);
      let amount;
      if (item[index].DebitAmount.length === 0) {
        amount = 0;
      } else {
        amount = +item[index].DebitAmount;
      }

      const dateString = item[index].Date.toString();
      const mm = dateString.slice(0, 2);
      const dd = dateString.slice(3, 5);
      const yyyy = dateString.slice(6, 10);
      item[index].date = yyyy + '-' + mm + '-' + dd + 'T00:00:00';

      if (amount === 0) {
        const itemRevenue: RevenueCSV = new RevenueCSV();
        item[index].CreditAmount = item[index].CreditAmount.replace(/,/gi, '');
        itemRevenue.date = new Date(item[index].Date);
        itemRevenue.subTotal = item[index].CreditAmount;
        itemRevenue.tax = 0;
        itemRevenue.total = item[index].CreditAmount;
        itemRevenue.description = item[index].Description;
        itemRevenue.reference = item[index].Reference;
        itemRevenue.transactionNumber = item[index].TransactionNumber;
        itemRevenue.contractNumber = item[index].ContractNumber;
        this.revenuesCSV.push(itemRevenue);
      } else {
        const itemExpenses: ExpenseCSVModel = new ExpenseCSVModel();
        item[index].DebitAmount = item[index].DebitAmount.replace(/,/gi, '');
        itemExpenses.date = new Date(item[index].Date);
        itemExpenses.reference = item[index].Reference;
        itemExpenses.vendor = item[index].Vendor;
        itemExpenses.description = item[index].Description;
        itemExpenses.amount = item[index].DebitAmount;
        itemExpenses.contractNumber = item[index].ContractNumber;
        this.expensesCSV.push(itemExpenses);
      }
    }

    this.saveExpenses();
  }

  saveBudget(item: any): void {
    this.contractService.loadingSubject.next(true);
    this.beginImportCsv();

    for (let index = 0; index < item.length; index++) {

      const itemBudget: ContractCSVModel = new ContractCSVModel();
      itemBudget.ContractNumber = item[index].ContractNumber;
      itemBudget.BuildingCode = item[index].BuildingCode;
      itemBudget.CustomerCode = item[index].CustomerCode;
      itemBudget.Area = (item[index].Area.length > 0) ? item[index].Area : 0;
      itemBudget.NumberOfPeople = (item[index].NumberOfPeople.length > 0) ? item[index].NumberOfPeople : 0;
      itemBudget.Status = item[index].Status;
      itemBudget.DaysPerMonth = (item[index].DaysPerMonth.length > 0) ? item[index].DaysPerMonth : 0;

      itemBudget.Description = item[index].Description;


      if (item[index].ExpirationDate.length === 10) {
        const dateString = item[index].ExpirationDate.toString();
        const mm = dateString.slice(0, 2);
        const dd = dateString.slice(3, 5);
        const yyyy = dateString.slice(6, 10);
        item[index].date = yyyy + '-' + mm + '-' + dd + 'T00:00:00';
        itemBudget.ExpirationDate = item.date = new Date(item[index].date);
      } else {
        itemBudget.ExpirationDate = null;
      }

      itemBudget.NumberOfRestrooms = (item[index].NumberOfRestrooms.length > 0) ? item[index].NumberOfRestrooms : 0;
      itemBudget.UnoccupiedSquareFeets = (item[index].UnoccupiedSquareFeets.length > 0) ? item[index].UnoccupiedSquareFeets : 0;
      itemBudget.ProductionRate = (item[index].ProductionRate.length > 0) ? item[index].ProductionRate : 0;
      this.budgetCSV.push(itemBudget);
    }

    this.budgetCSV.forEach(async element => {
      await this.contractService.createElementCSV(element)
        .then(
          async (data) => {
            const response = data['body'];
            const sum = this.validateImportResults(response, element);
            if (sum === this.budgetCSV.length) {
              this.contractService.loadingSubject.next(false);
              this.contractService.getElements();
              this.resultCsvDialog = this.dialog.open(ResultImportCsvComponent, {
                panelClass: 'result-import-csv-form-dialog',
                data: {
                  result: this.resultImportacionCSV,
                  itemResult: this.itemResult,
                  type: 'budget'
                }
              });
            }
          },
          () => {
            this.contractService.loadingSubject.next(false);
            this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 });
          })
        .catch(() => {
          this.contractService.loadingSubject.next(false);
          this.snackBar.open('Oops, There was an error in importing expenses', 'close', { duration: 1000 });
        });
    });
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

  async saveRevenuesEstimated(item: any): Promise<any> {

    this.beginImportCsv();
    for (let index = 0; index < item.length; index++) {

      const contractItem: ContractItemCSVModel = new ContractItemCSVModel();

      contractItem.ContractNumber = item[index].ContractNumber;
      contractItem.Quantity = (item[index].Quantity.length > 0) ? item[index].Quantity : 'null';
      contractItem.Description = item[index].Description;

      contractItem.OfficeServiceTypeName = item[index].OfficeServiceTypeName;

      item[index].Rate = item[index].Rate.replace(/,/gi, '');
      contractItem.Rate = +item[index].Rate;

      contractItem.Value = (item[index].Value.length > 0) ? item[index].Value : 'null';

      this.budgetItemCSV.push(contractItem);
    }

    const itemsCount = this.budgetItemCSV.length;
    for (let i = 0; i < itemsCount; i++) {

      const result = await this.contractService.addContractItemCsv(this.budgetItemCSV[i]).toPromise();
      const sum = this.validateImportResults(result, this.budgetItemCSV[i]);

    }

    this.contractService.loadingSubject.next(false);
    this.resultCsvDialog = this.dialog.open(ResultImportCsvComponent, {
      panelClass: 'result-import-csv-form-dialog',
      data: {
        result: this.resultImportacionCSV,
        itemResult: this.itemResult,
        type: 'budgetItem'
      }
    });
  }

  async saveExpensesEstimated(item: any): Promise<any> {
    this.beginImportCsv();

    for (let index = 0; index < item.length; index++) {
      const expenseBudget: ContractExpenseCSVModel = new ContractExpenseCSVModel();
      expenseBudget.ContractNumber = item[index].ContractNumber;
      expenseBudget.Quantity = (item[index].Quantity.length > 0) ? item[index].Quantity : 'null';
      expenseBudget.Description = item[index].Description;
      expenseBudget.ExpenseType = item[index].ExpenseType;
      expenseBudget.ExpenseSubcategory = item[index].ExpenseSubcategory;

      expenseBudget.Rate = (item[index].Rate.length > 0) ? item[index].Rate : 'null';

      expenseBudget.Value = (item[index].Value.length > 0) ? item[index].Value : 'null';

      expenseBudget.TaxPercent = (item[index].TaxPercent.length > 0) ? item[index].TaxPercent : 'null';

      this.budgetExpenseCSV.push(expenseBudget);
    }

    const expensesCount = this.budgetExpenseCSV.length;
    for (let i = 0; i < expensesCount; i++) {

      const response = await this.contractService.addContractExpenseCsv(this.budgetExpenseCSV[i]).toPromise();
      const sum = this.validateImportResults(response, this.budgetItemCSV[i]);

    }

    this.contractService.loadingSubject.next(false);
    this.resultCsvDialog = this.dialog.open(ResultImportCsvComponent, {
      panelClass: 'result-import-csv-form-dialog',
      data: {
        result: this.resultImportacionCSV,
        itemResult: this.itemResult,
        type: 'budgetExpense'
      }
    });
  }

  validateImportResults(response, item): number {
    switch (response.errorCode) {
      case 0:
        this.resultImportacionCSV[0] = this.resultImportacionCSV[0] + 1;
        break;
      case 1:
        this.resultImportacionCSV[1] = this.resultImportacionCSV[1] + 1;
        item.error = response.message;
        this.itemResult.push(item);
        break;
      case 2:
        this.resultImportacionCSV[2] = this.resultImportacionCSV[2] + 1;
        item.error = response.message;
        this.itemResult.push(item);
        break;
      case 3:
        this.resultImportacionCSV[3] = this.resultImportacionCSV[3] + 1;
        item.error = response.message;
        this.itemResult.push(item);
        break;
      case 4:
        this.resultImportacionCSV[4] = this.resultImportacionCSV[4] + 1;
        break;
      case 5:
        // Update
        this.resultImportacionCSV[5] = this.resultImportacionCSV[5] + 1;
        break;
      case 6:
        this.resultImportacionCSV[6] = this.resultImportacionCSV[6] + 1;
        item.error = response.message;
        this.itemResult.push(item);
        break;
      default:
        break;
    }
    const sum = this.resultImportacionCSV[0] + this.resultImportacionCSV[1]
      + this.resultImportacionCSV[2] + this.resultImportacionCSV[3]
      + this.resultImportacionCSV[4] + this.resultImportacionCSV[5]
      + this.resultImportacionCSV[6];
    return sum;
  }

  beginImportCsv(): void {
    this.contractService.loadingSubject.next(true);
    this.budgetCSV = [];
    this.budgetItemCSV = [];
    this.budgetExpenseCSV = [];
    this.revenuesCSV = [];
    this.expensesCSV = [];

    this.itemResult = [];

    this.resultImportacionCSV[0] = 0;
    this.resultImportacionCSV[1] = 0;
    this.resultImportacionCSV[2] = 0;
    this.resultImportacionCSV[3] = 0;
    this.resultImportacionCSV[4] = 0;
    this.resultImportacionCSV[5] = 0;
    this.resultImportacionCSV[6] = 0;
  }

  // Export CSV
  exportToCsv(): void {
    this.contractService.exportCsv()
      .then((csvFile) => {
        this.downloadCsvfile(csvFile);
      })
      .catch((error) => {
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

  downloadCsvfile(data: any): void {
    const csvData = data;
    const a = document.createElement('a');
    a.setAttribute('style', 'display:none;');
    document.body.appendChild(a);
    const blob = new Blob([csvData.body], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    a.href = url;
    a.download = 'BudgetReport_' + this.dateReportString + '.csv';
    a.click();
  }

  get dateReportString(): string {
    return moment(new Date()).format('YYYY-MM-DD');
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

export class ContractDataSource extends DataSource<any>
{
  constructor(private contractService: ContractsService) {
    super();
  }

  connect(): Observable<any[]> {
    return this.contractService.allElementsChanged;
  }

  disconnect(): void {

  }
}
