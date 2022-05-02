import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { ExpenseCategory } from '@app/core/models/expense/expense-base.model';
import { ExpenseCSVModel } from '@app/core/models/expense/expense-csv.model';
import { RevenueCSV } from '@app/core/models/revenue/revenue-csv-model';
import { fuseAnimations } from '@fuse/animations';
import { BehaviorSubject } from 'rxjs';
import { RevenueExpenseCSVModel } from '@app/core/models/contract/revenue-expense-csv.model';
import * as Papa from 'papaparse';

@Component({
  selector: 'app-import-revenue-expense-csv-form',
  templateUrl: './import-revenue-expense-csv-form.component.html',
  styleUrls: ['./import-revenue-expense-csv-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ImportRevenueExpenseCsvFormComponent implements OnInit {


  public revenues: RevenueCSV[] = [];
  public expenses: ExpenseCSVModel[] = [];

  dialogTitle: string;
  action: string;

  isResult: any;
  quantityElements = 0;

  items: RevenueExpenseCSVModel[] = [];
  dataSource = new MatTableDataSource(this.items);
  displayedColumns = ['ExcelRow', 'TransactionId', 'description', 'date', 'reference', 'debitAmount', 'creditAmount', 'options'];

  types: { id: number, name: string }[] = [];

  importCsvForm: FormGroup;
  loading$ = new BehaviorSubject<boolean>(false);

  public records: any[] = [];
  public expesesAndRevenues: RevenueExpenseCSVModel[] = [];

  revenuesImported: any;
  expensesImproted: any;
  revenueRepeated: any;
  expensesRepeated: any;
  error: any;
  total: any;

  // Selected Type

  TypeDocument: any[] = [
    { 'id': 0, 'name': 'Budget' },
    { 'id': 1, 'name': 'Estimated Revenues' },
    { 'id': 2, 'name': 'Estimated Expenses' },
    { 'id': 3, 'name': 'Real Revenues and Expenses' }
  ];

  columns = '';

  array = [];

  arrayBudget =
    [
      {
        ContractNumber: '',
        BuildingCode: '',
        CustomerCode: '',
        Area: '',
        NumberOfPeople: '',
        Status: '',
        DaysPerMonth: '',
        Description: '',
        ExpirationDate: '',
        NumberOfRestrooms: '',
        UnoccupiedSquareFeets: '',
        ProductionRate: ''
      }
    ];

  arrayRevenuesEstimated =
    [
      {
        ContractNumber: '',
        Quantity: '',
        Description: '',
        OfficeServiceTypeName: '',
        Rate: '',
        Value: '',
      }
    ];

  arrayExpensesEstimated =
    [
      {
        ContractNumber: '',
        Quantity: '',
        Description: '',
        ExpenseType: '',
        ExpenseSubcategory: '',
        Rate: '',
        Value: '',
        TaxPercent: ''
      }
    ];

  arrayExpenseAndRevenue =
    [
      {
        Date: '',
        Reference: '',
        Description: '',
        Vendor: '',
        DebitAmount: '',
        CreditAmount: '',
        ContractNumber: ''
      }
    ];


  constructor(
    public dialogRef: MatDialogRef<ImportRevenueExpenseCsvFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private snackBar: MatSnackBar,
    private formBuilder: FormBuilder
  ) {
    this.data = data;
    this.action = data.action;
    this.dialogTitle = 'Import Revenue and Expenses';
    if (this.action === 'Result') {
      this.revenuesImported = data.revenuesImported;
      this.expensesImproted = data.expensesImproted;
      this.revenueRepeated = data.revenueRepeated;
      this.expensesRepeated = data.expensesRepeated;
      this.total = data.total;
    }
  }

  ngOnInit(): void {
    if (this.action === 'Result') {
      this.isResult = false;
    } else {
      this.isResult = true;
      this.importCsvForm = this.createForm();
      this.changeTypeDocument(0);
      this.getTypesToArray();
    }
  }

  createForm(): FormGroup {
    return this.formBuilder.group({
      item: this.formBuilder.array([]),
      TypeDocument: [0, [Validators.required]]
    });
  }

  /** Types */
  getTypesToArray(): void {
    for (const category in ExpenseCategory) {
      if (typeof ExpenseCategory[category] === 'number') {
        this.types.push({ id: ExpenseCategory[category] as any, name: category });
      }
    }
  }

  isValidCSVFile(file: any): any {
    return file.name.endsWith('.csv');
  }

  fileReset(): void {
    this.records = [];
  }


  ClickSave(): void {
    if (this.quantityElements > 0) {
      this.dialogRef.close(this.importCsvForm);
    } else {
      this.snackBar.open('Oops,there are only 0 items to import', 'close', { duration: 1000 });
    }
  }

  onChangeCSV($event: any): void {

    this.loading$.next(true);

    const text = [];
    const files = $event.srcElement.files;

    if (this.isValidCSVFile(files[0])) {

      Papa.parse(files[0], {
        header: true,
        skipEmptyLines: true,
        complete: (result, file) => {

          this.records = result.data;

          this.ValidateRow();

          const taskFormArray = this.formBuilder.array(this.records);
          this.importCsvForm.setControl('item', taskFormArray);
          this.quantityElements = this.records.length;
        }
      });
    } else {
      this.loading$.next(false);
      alert('Please import valid .csv file.');
      this.fileReset();
    }
  }

  changeTypeDocument(id: number): void {
    switch (id) {
      case 0:
        this.array = this.arrayBudget;
        break;
      case 1:
        this.array = this.arrayExpensesEstimated;
        break;
      case 2:
        this.array = this.arrayRevenuesEstimated;
        break;
      case 3:
        this.array = this.arrayExpenseAndRevenue;
        break;
      default:
        break;
    }
    this.quantityElements = 0;
    this.records = [];
  }

  downloadCSV(): void {
    const csv = Papa.unparse(this.array);

    const csvData = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    let csvURL = null;
    if (navigator.msSaveBlob) {
      csvURL = navigator.msSaveBlob(csvData, 'download.csv');
    }
    else {
      csvURL = window.URL.createObjectURL(csvData);
    }

    const tempLink = document.createElement('a');
    tempLink.href = csvURL;
    tempLink.setAttribute('download', 'download.csv');
    tempLink.click();
  }

  ValidateRow(): void {
    const id = this.importCsvForm.get('TypeDocument').value;
    // { 'id': 0, 'name': 'Budget' },
    // { 'id': 1, 'name': 'Estimated Revenues' },
    // { 'id': 2, 'name': 'Estimated Expenses' },
    // { 'id': 3, 'name': 'Real Revenues and Expenses' }
    switch (id) {
      case 0:
        if (this.records[0].ContractNumber && this.records[0].BuildingCode && this.records[0].CustomerCode) {
          const items = this.records.filter(i => i.ContractNumber !== '' && i.BuildingCode !== '' && i.CustomerCode !== '');
          this.records = items;
        } else {
          this.snackBar.open('Oops, This document does not comply with the appropriate format', 'close', { duration: 5000 });
          this.records = [];
        }
        break;
      case 1:
        if (this.records[0].ContractNumber && this.records[0].Description) {
          const items = this.records.filter(i => i.ContractNumber !== '');
          this.records = items;
        } else {
          this.snackBar.open('Oops, This document does not comply with the appropriate format', 'close', { duration: 5000 });
          this.records = [];
        }
        break;
      case 2:
        if (this.records[0].ContractNumber && this.records[0].ExpenseType) {
          const items = this.records.filter(i => i.ContractNumber !== '' && i.ExpenseType !== '');
          this.records = items;
        } else {
          this.snackBar.open('Oops, This document does not comply with the appropriate format', 'close', { duration: 5000 });
          this.records = [];
        }
        break;
      case 3:
        if (this.records[0].ContractNumber && this.records[0].Date && this.records[0].Reference) {
          const items = this.records.filter(i => i.ContractNumber !== '' && i.BuildingCode !== '' && i.CustomerCode !== '');
          this.records = items;
        } else {
          this.snackBar.open('Oops, This document does not comply with the appropriate format', 'close', { duration: 5000 });
          this.records = [];
        }
        break;
      default:
        break;
    }
  }

}
