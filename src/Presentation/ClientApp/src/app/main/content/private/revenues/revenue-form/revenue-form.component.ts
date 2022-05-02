import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';
import { RevenueBaseModel } from '@app/core/models/revenue/revenue-base.model';
import { fuseAnimations } from '@fuse/animations';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BuildingsService } from '../../buildings/buildings.service';
import { ContractsService } from '../../contracts/contracts.service';

@Component({
  selector: 'app-revenue-form',
  templateUrl: './revenue-form.component.html',
  styleUrls: ['./revenue-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class RevenueFormComponent implements OnInit {

  private revenue: RevenueBaseModel;
  revenueForm: FormGroup;

  action: string;
  dialogTitle: string;
  private _onDestroy = new Subject<void>();

  // Building
  buildings: ListItemModel[] = [];
  filteredBuildings: Subject<any[]> = new Subject<any[]>();
  get buildingFilter(): AbstractControl { return this.revenueForm.get('buildingFilter'); }

  // Cotracts
  contracts: ContractBaseModel[] = [];
  filteredContracts: Subject<any[]> = new Subject<any[]>();
  get contractsFilter(): AbstractControl { return this.revenueForm.get('contractFilter'); }

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    public dialogRef: MatDialogRef<RevenueFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private buildingService: BuildingsService,
    private contractService: ContractsService,
    private snackBar: MatSnackBar
  ) {
    this.action = data.action;

    if (this.action === 'new') {
      this.dialogTitle = 'New Revenue';
      this.revenueForm = this.createExpenseForm();
    } else {
      this.revenue = data.revenue;
      this.dialogTitle = 'Edit Revenue';
      this.revenueForm = this.updateExpenseForm();
    }
  }

  ngOnInit(): void {
    this.getBuildings();
    if (this.action === 'edit') {
      this.revenueForm.patchValue({
        buildingId: this.revenue.buildingId,
        contractId: this.revenue.contractId
      });
      this.getContracts(this.revenue.buildingId, this.revenue.contractId);
    }
    this.buildingFilter.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.buildingFilters();
      })
      ;
  }

  createExpenseForm(): FormGroup {
    return this.formBuilder.group({
      date: [''],
      subTotal: [''],
      tax: [''],
      total: [''],
      description: [''],
      reference: [''],
      buildingFilter: [''],
      contractFilter: [''],
      custumer: [''],
      buildingId: ['', [Validators.required]],
      customerId: ['', [Validators.required]],
      contractId: [''],
      transactionNumber: ['']
    });
  }

  updateExpenseForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.revenue.id],
      date: [{ value: this.revenue.date, disabled: this.readOnly }],
      subTotal: [{ value: this.revenue.subTotal, disabled: this.readOnly }],
      tax: [{ value: this.revenue.tax, disabled: this.readOnly }],
      total: [{ value: this.revenue.total, disabled: this.readOnly }],
      description: [{ value: this.revenue.description, disabled: this.readOnly }],
      reference: [{ value: this.revenue.reference, disabled: this.readOnly }],
      buildingFilter: [''],
      contractFilter: [''],
      custumer: [''],
      buildingId: [{ value: this.revenue.buildingId, disabled: this.readOnly }, [Validators.required]],
      customerId: [{ value: this.revenue.customerId, disabled: this.readOnly }, [Validators.required]],
      contractId: [{ value: this.revenue.contractId, disabled: this.readOnly }],
      transactionNumber: [{ value: this.revenue.transactionNumber, disabled: this.readOnly }]
    });
  }

  getBuildings(): void {
    this.buildingService.getAllAsList('ReadAllCbo', '', 0, 999, null, null)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings.next(this.buildings);
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get buildings', 'Close');
      });
  }

  getContracts(buildingId, contractId): void {
    this.contractService.getAllAsListByBuildingWithContract(buildingId, contractId)
      .subscribe((response: { count: number, payload: ContractBaseModel[] }) => {
        this.contracts = response.payload;
        this.filteredContracts.next(this.contracts);
        if (this.contracts.length === 1) {
          const objContract = this.contracts.find(t => t.buildingId === buildingId);
          this.revenueForm.patchValue({
            contractId: objContract.id,
            customerId: objContract.customerId
          });
        }
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get contracts', 'Close');
      });
  }

  // onChange Controller
  onChangeTax(): void {
    const tax = this.revenueForm.get('tax').value;
    const subTotal = this.revenueForm.get('subTotal').value;

    const value = subTotal + ((subTotal / 100) * tax);

    if (subTotal !== 0) {
      this.revenueForm.patchValue({
        total: value
      });
    }
  }

  buildingChanged(buildingId: number): void {
    this.contractService.getAllAsListByBuilding(buildingId)
      .subscribe((response: { count: number, payload: ContractBaseModel[] }) => {
        this.contracts = response.payload;
        this.filteredContracts.next(this.contracts);
        if (this.contracts.length === 1) {
          const objContract = this.contracts.find(t => t.buildingId === buildingId);
          this.revenueForm.patchValue({
            customerId: objContract.customerId,
            contractId: objContract.id
          });
        }

      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get buildings', 'Close');
      });
  }

  OnChangeBuilding(): void {
  }

  contractChanged(id: number): void {
    const objContract = this.contracts.find(t => t.id === id);
    this.revenueForm.patchValue({
      customerId: objContract.customerId
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

}
