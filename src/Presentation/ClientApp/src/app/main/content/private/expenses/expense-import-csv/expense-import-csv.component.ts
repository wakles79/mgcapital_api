import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormGroup, AbstractControl, FormBuilder, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';
import { ExpenseBaseModel } from '@app/core/models/expense/expense-base.model';
import { ExpenseCSVModel } from '@app/core/models/expense/expense-csv.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BuildingsService } from '../../buildings/buildings.service';
import { ContractsService } from '../../contracts/contracts.service';
import { ExpensesService } from '../expenses.service';
import * as moment from 'moment';
import * as Papa from 'papaparse';

@Component({
  selector: 'app-expense-import-csv',
  templateUrl: './expense-import-csv.component.html',
  styleUrls: ['./expense-import-csv.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ExpenseImportCsvComponent implements OnInit {


  expenses: ExpenseCSVModel[] = [];
  recordCSV: ExpenseCSVModel[] = [];
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  revenueForm: FormGroup;


  csvReader: any;
  private _onDestroy = new Subject<void>();

  // Building
  buildings: ListItemModel[] = [];
  public records: ExpenseBaseModel[] = [];
  filteredBuildings: Subject<any[]> = new Subject<any[]>();
  get buildingFilter(): AbstractControl { return this.revenueForm.get('buildingFilter'); }

  // Cotracts
  contracts: ContractBaseModel[] = [];
  filteredContracts: Subject<any[]> = new Subject<any[]>();

  // Data CSV
  displayedColumns = ['description'];
  dataSource: any | null;
  action: any;

  // Active/Disactive boton Guardar
  isDone: boolean;

  payload: any[] = [];
  constructor(
    public dialogRef: MatDialogRef<ExpenseImportCsvComponent>,
    private snackBar: MatSnackBar,
    private formBuilder: FormBuilder,
    private buildingService: BuildingsService,
    private contractService: ContractsService,
    private expenseService: ExpensesService,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.revenueForm = this.createRevenueForm();
    this.isDone = false;
    this.getBuildings();
    this.buildingFilter.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.buildingFilters();
      });
  }

  createRevenueForm(): FormGroup {
    return this.formBuilder.group({
      buildingFilter: [''],
      filteredContracts: [''],
      contractFilter: [''],
      custumer: [''],
      buildingId: ['', [Validators.required]],
      customerId: ['', [Validators.required]],
      contractId: ['', [Validators.required]]
    });
  }

  // BUILDING
  getBuildings(): void {
    this.buildingService.getAllAsList('ReadAllCbo', '', 0, 999, null, null)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings.next(this.buildings);
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get buildings', 'Close');
      });
  }

  buildingFilters(): void {
    if (!this.buildings) {
      return;
    }
    let search = this.buildingFilter.value;
    if (!search) {
      this.filteredBuildings.next(this.buildings.slice());
      return;
    } else {
      search = search.toLowerCase();
    }

    this.filteredBuildings.next(
      this.buildings.filter(customer => customer.name.toLowerCase().indexOf(search) > -1)
    );
  }

  buildingChanged(buildingId: number): void {
    this.contractService.getAllAsListByBuilding(buildingId)
      .subscribe((response: { count: number, payload: ContractBaseModel[] }) => {
        this.contracts = response.payload;
        this.filteredContracts.next(this.contracts);
        if (this.contracts.length === 1) {
          const x = this.contracts.find(t => t.buildingId === buildingId);
          this.revenueForm.patchValue({
            customerId: x.customerId
          });
        } else {
          this.revenueForm.patchValue({
            customerId: ''
          });
        }
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get buildings', 'Close');
      });
  }

  contractChanged(contractId: number): void {
    for (let index = 0; index < this.contracts.length; index++) {
      if (contractId === this.contracts[index].id) {
        this.revenueForm.patchValue({
          customerId: this.contracts[index].customerId
        });
      }
    }
  }

  // IMPORT DOCUMENT CSV
  uploadListener($event: any): void {

    const text = [];
    const files = $event.srcElement.files;

    if (this.isValidCSVFile(files[0])) {

      const input = $event.target;
      const reader = new FileReader();
      reader.readAsText(input.files[0]);

      reader.onload = () => {
        const csvData = reader.result;
        const csvRecordsArray = (csvData as string).split(/\r\n|\n/);

        const headersRow = this.getHeaderArray(csvRecordsArray);

        this.records = this.getDataRecordsArrayFromCSVFile(csvRecordsArray, headersRow.length);
      };

      // tslint:disable-next-line: only-arrow-functions
      reader.onerror = function () {
        console.log('error is occured while reading file!');
      };

    } else {
      alert('Please import valid .csv file.');
      this.fileReset();
    }
  }

  getDataRecordsArrayFromCSVFile(csvRecordsArray: any, headerLength: any): any[] {
    const csvArr = [];

    for (let i = 1; i < csvRecordsArray.length; i++) {
      const curruntRecord = (csvRecordsArray[i] as string).split(',');
      if (curruntRecord.length === headerLength) {
        const csvRecord: ExpenseCSVModel = new ExpenseCSVModel();
        csvRecord.reference = curruntRecord[0].trim();
        csvRecord.amount = parseInt(curruntRecord[1].trim(), 10);
        csvRecord.vendor = curruntRecord[2].trim();
        csvRecord.description = curruntRecord[3].trim();
        csvRecord.date = new Date(curruntRecord[4].trim());

        if (isNaN(csvRecord.amount)) {
          csvRecord.amount = 0;
        }
        if (isNaN(csvRecord.date.getDate())) {
          console.log('Fecha no permitida');
        }
        csvRecord.transactionNumber = ((this.revenueForm.controls['buildingId'].value + moment(csvRecord.date).format('MMDDYYYY').toString()).toString());

        csvRecord.isRepeated = false;

        for (let index = 0; index < csvArr.length; index++) {
          if (moment(csvArr[index].date).format('YYYYMMDD') === moment(csvRecord.date).format('YYYYMMDD')
            && csvArr[index].reference === csvRecord.reference
            && csvArr[index].amount === csvRecord.amount) {
            csvRecord.isRepeated = true;
            csvArr[index].isRepeated = true;
          }
        }

        csvArr.push(csvRecord);
      }
    }
    return csvArr;
  }

  isValidCSVFile(file: any): any {
    return file.name.endsWith('.csv');
  }

  getHeaderArray(csvRecordsArr: any): any[] {
    const headers = (csvRecordsArr[0] as string).split(',');
    const headerArray = [];
    // tslint:disable-next-line: prefer-for-of
    for (let j = 0; j < headers.length; j++) {
      headerArray.push(headers[j]);
    }
    return headerArray;
  }

  fileReset(): void {
    this.expenses = [];
  }

  // ClickSave() {
  //   this.expenses.forEach(element => {
  //     element.buildingId = this.revenueForm.get('buildingId').value;
  //     element.customerId = this.revenueForm.get('customerId').value;
  //     element.contractId = this.revenueForm.get('contractId').value;
  //   });

  //   const customerId = this.revenueForm.get('customerId').value;
  //   const buildingId = this.revenueForm.get('buildingId').value;
  //   const contractId = this.revenueForm.get('contractId').value;

  //   if (contractId === '' || buildingId === '' || customerId === '') {
  //     this.snackBar.open('Oops, select a building and budget', 'ok');
  //     return;
  //   }

  //   if (this.expenses.length === 0) {
  //     this.snackBar.open('Oops, CSV file not found', 'ok', { duration: 1000 });
  //     return;
  //   }

  //   console.log(this.expenses);
  //   this.expenseService.createElementCSV(this.expenses)
  //     .then(
  //       (createdProposal) => {
  //         this.dialogRef.close(this.revenueForm.value);
  //         this.snackBar.open('Revenues created successfully!!!', 'close', { duration: 1000 });
  //       },
  //       () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
  //     .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

  // }

  ActiveSave(): boolean {
    const customerId = this.revenueForm.get('customerId').value;
    const buildingId = this.revenueForm.get('buildingId').value;
    if (this.records.length > 0 && customerId !== '' && buildingId !== '') {
      this.isDone = false;
    } else {
      this.isDone = true;
    }
    return this.isDone;
  }

  deleteRevenue(id): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });
    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        const items = this.expenses.filter(i => i.row !== id);
        this.expenses = items;
        this.validateRepeatedRowDelete();
      }
      this.confirmDialogRef = null;
    });
  }

  // IMPORT DOCUMENT CSV

  onChangeCSV($event: any): void {

    const text = [];
    const files = $event.srcElement.files;

    if (this.isValidCSVFile(files[0])) {

      Papa.parse(files[0], {
        header: true,
        skipEmptyLines: true,
        complete: (result, file) => {
          this.recordCSV = result.data;
          this.validateRepeatedRow();
        }
      });
    } else {
      alert('Please import valid .csv file.');
      this.fileReset();
    }
  }

  validateRepeatedRow(): void {
    for (let i = 0; i < this.recordCSV.length; i++) {
      this.recordCSV[i].transactionNumber = ((this.revenueForm.controls['buildingId'].value + moment(this.recordCSV[i].date).format('MMDDYYYY').toString()).toString());
      this.recordCSV[i].row = i + 2;
      this.recordCSV[i].isRepeated = false;

      for (let index = 0; index < this.expenses.length; index++) {
        if (moment(this.expenses[index].date).format('YYYYMMDD') === moment(this.recordCSV[i].date).format('YYYYMMDD')
          && this.expenses[index].reference.toString() === this.recordCSV[i].reference.toString()
          && this.expenses[index].amount.toString() === this.recordCSV[i].amount.toString()) {
          this.recordCSV[i].isRepeated = true;
          this.expenses[index].isRepeated = true;
        }
      }
      this.expenses.push(this.recordCSV[i]);
    }
    const items = this.expenses.filter(i => i.reference !== '');
    this.expenses = items;
  }

  validateRepeatedRowDelete(): void {
    for (let i = 0; i < this.expenses.length; i++) {
      this.expenses[i].isRepeated = false;
      for (let index = 0; index < this.expenses.length; index++) {
        if (i !== index
          && moment(this.expenses[index].date).format('YYYYMMDD') === moment(this.expenses[i].date).format('YYYYMMDD')
          && this.expenses[index].reference === this.expenses[i].reference
          && this.expenses[index].amount === this.expenses[i].amount) {
          this.expenses[i].isRepeated = true;
          this.expenses[index].isRepeated = true;
        }
      }
    }
  }

}
