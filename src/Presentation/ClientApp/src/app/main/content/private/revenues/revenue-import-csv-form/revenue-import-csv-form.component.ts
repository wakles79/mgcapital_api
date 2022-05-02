import { Component, Inject, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';
import { RevenueCSV } from '@app/core/models/revenue/revenue-csv-model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BuildingsService } from '../../buildings/buildings.service';
import { ContractsService } from '../../contracts/contracts.service';
import { CustomersService } from '../../customers/customers.service';
import { RevenuesService } from '../revenues.service';
import * as Papa from 'papaparse';
import * as moment from 'moment';

@Component({
  selector: 'app-revenue-import-csv-form',
  templateUrl: './revenue-import-csv-form.component.html',
  styleUrls: ['./revenue-import-csv-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class RevenueImportCsvFormComponent implements OnInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  csvReader: any;
  public records: RevenueCSV[] = [];
  public revenues: RevenueCSV[] = [];
  revenueForm: FormGroup;
  private _onDestroy = new Subject<void>();

  // Building
  buildings: ListItemModel[] = [];
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
  dataList: any[];

  constructor(
    public dialogRef: MatDialogRef<RevenueImportCsvFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private snackBar: MatSnackBar,
    private buildingService: BuildingsService,
    private contractService: ContractsService,
    private custumerService: CustomersService,
    private revenueService: RevenuesService,
    public dialog: MatDialog
  ) {
    this.revenueForm = this.createRevenueForm();
    this.action = 'edit';
  }

  ngOnInit(): void {
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

  getContract(): void {
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

  contractChanged(id: number): void {
    const objContract = this.contracts.find(t => t.id === id);
    this.revenueForm.patchValue({
      customerId: objContract.customerId
    });
  }

  // IMPORT DOCUMENT CSV
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

  ClickSave(): void {
    this.records.forEach(element => {
      // element.buildingId = this.revenueForm.get('buildingId').value;
      // element.customerId = this.revenueForm.get('customerId').value;
      // element.contractId = this.revenueForm.get('contractId').value;
    });

    if (this.records.length === 0) {
      this.snackBar.open('Oops, CSV file not found', 'ok', { duration: 1000 });
      return;
    }

    this.revenueService.createElementCSV(this.records)
      .then(
        (createdProposal) => {
          this.dialogRef.close(this.revenueForm.value);
          this.snackBar.open('Revenues created successfully!!!', 'close', { duration: 1000 });
        },
        () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
      .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

  }

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
        const items = this.records.filter(i => i.row !== id);
        const itemsClear = this.records.filter(i => i.row === id);
        this.records = items;
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
          this.revenues = result.data;
          this.validateRepeatedRow();
        }
      });
    } else {
      alert('Please import valid .csv file.');
      this.fileReset();
    }
  }

  fileReset(): void {
    this.records = [];
  }

  isValidCSVFile(file: any): any {
    return file.name.endsWith('.csv');
  }

  validateRepeatedRow(): void {
    for (let i = 0; i < this.revenues.length; i++) {
      this.revenues[i].transactionNumber = ((this.revenueForm.controls['buildingId'].value + moment(this.revenues[i].date).format('MMDDYYYY').toString()).toString());
      this.revenues[i].row = i + 2;
      this.revenues[i].isRepeated = false;

      for (let index = 0; index < this.records.length; index++) {
        if (moment(this.records[index].date).format('YYYYMMDD') === moment(this.revenues[i].date).format('YYYYMMDD')
          && this.records[index].reference.toString() === this.revenues[i].reference.toString()
          && this.records[index].total.toString() === this.revenues[i].total.toString()
          && this.records[index].subTotal.toString() === this.revenues[i].subTotal.toString()) {
          this.revenues[i].isRepeated = true;
          this.records[index].isRepeated = true;
        }
      }
      this.records.push(this.revenues[i]);
    }
    const items = this.records.filter(i => i.reference !== '');
    this.records = items;
  }

  validateRepeatedRowDelete(): void {
    for (let i = 0; i < this.records.length; i++) {
      this.records[i].isRepeated = false;
      for (let index = 0; index < this.records.length; index++) {
        if (i !== index
          && moment(this.records[index].date).format('YYYYMMDD') === moment(this.records[i].date).format('YYYYMMDD')
          && this.records[index].reference === this.records[i].reference
          && this.records[index].total === this.records[i].total
          && this.records[index].subTotal === this.records[i].subTotal) {
          this.records[i].isRepeated = true;
          this.records[index].isRepeated = true;
        }
      }
    }
  }

}
