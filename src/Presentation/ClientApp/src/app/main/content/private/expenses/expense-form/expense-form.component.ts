import { Component, OnInit, ViewEncapsulation, OnDestroy, Inject } from '@angular/core';
import { FormControl, FormGroup, AbstractControl, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';
import { ExpenseBaseModel, ExpenseCategory } from '@app/core/models/expense/expense-base.model';
import { fuseAnimations } from '@fuse/animations';
import { Subject, Subscription } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BuildingsService } from '../../buildings/buildings.service';
import { ContractsService } from '../../contracts/contracts.service';

@Component({
  selector: 'app-expense-form',
  templateUrl: './expense-form.component.html',
  styleUrls: ['./expense-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ExpenseFormComponent implements OnInit, OnDestroy {

  private _onDestroy = new Subject<void>();
  private expense: ExpenseBaseModel;

  expenseForm: FormGroup;
  get getBuildingId(): FormControl { return this.expenseForm.get('isDirect') as FormControl; }

  action: string;
  dialogTitle: string;

  get getBuildingFilter(): AbstractControl { return this.expenseForm.get('buildingFilter'); }
  buildings: ListItemModel[] = [];
  filteredBuildings: Subject<any[]> = new Subject<any[]>();
  listBuildingsSubscription: Subscription;

  listContractsSubscription: Subscription;

  categories: { id: number, name: string }[] = [];

  private today = new Date();

  // Cotracts
  contracts: ContractBaseModel[] = [];
  filteredContracts: Subject<any[]> = new Subject<any[]>();
  get contractsFilter(): AbstractControl { return this.expenseForm.get('contractFilter'); }

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    public dialogRef: MatDialogRef<ExpenseFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private buildingService: BuildingsService,
    private contractService: ContractsService,
    private snackBar: MatSnackBar
  ) {
    this.action = data.action;

    if (this.action === 'new') {
      this.dialogTitle = 'New Expense';
      this.expenseForm = this.createExpenseForm();
    } else {
      this.dialogTitle = 'Edit Expense';
      this.expense = data.expense;
      this.expenseForm = this.updateExpenseForm();
    }
  }

  ngOnInit(): void {

    this.getCategoriesToArray();

    this.getBuildings();

    if (this.action === 'edit') {
      this.expenseForm.patchValue({
        buildingId: this.expense.buildingId,
        contractId: this.expense.contractId
      });
      this.getContracts(this.expense.buildingId, this.expense.contractId);
    }

    this.getBuildingId.valueChanges
      .subscribe(value => {
        if (value) {
          this.expenseForm.get('contractId').setValidators([Validators.required]);
          this.expenseForm.get('buildingId').setValidators([Validators.required]);
        } else {
          this.expenseForm
            .patchValue({
              contractId: null,
              buildingId: null,
              employeeId: null,
            });
          this.expenseForm.get('contractId').setValidators(null);
          this.expenseForm.get('buildingId').setValidators(null);
        }

        this.expenseForm.get('contractId').updateValueAndValidity();
        this.expenseForm.get('buildingId').updateValueAndValidity();
      });

    this.expenseForm.get('date').valueChanges
      .subscribe(date => {
        if (!date) {
          console.log('invalid date');
          return;
        }

        if (this.expenseForm.get('buildingId').value) {
          console.log('buildingId with value');
          this.getBuildingContracts(this.expenseForm.get('buildingId').value);
        } else {
          console.log('buildingId without value');
        }
      });

    this.getBuildingFilter.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBuildings();
      });

  }

  ngOnDestroy(): void {

    if (this.listBuildingsSubscription && this.listBuildingsSubscription.closed) {
      this.listBuildingsSubscription.unsubscribe();
    }

    if (this.listContractsSubscription && this.listContractsSubscription.closed) {
      this.listContractsSubscription.unsubscribe();
    }

  }

  /** FORM */
  createExpenseForm(): FormGroup {
    return this.formBuilder.group({
      isDirect: [true, [Validators.required]],
      contractId: [null, [Validators.required]],
      buildingId: [null, [Validators.required]],
      customerId: [null],
      type: [null, [Validators.required]],
      date: [new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate(), 18, 0, 0), [Validators.required]],
      reference: [''],
      amount: [null, [Validators.required]],
      vendor: [''],
      description: [''],
      buildingFilter: ['']
    });
  }

  updateExpenseForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.expense.id],
      isDirect: [{ value: this.expense.buildingId !== null, disabled: this.readOnly }, [Validators.required]],
      contractId: [{ value: this.expense.contractId, disabled: this.readOnly }, [Validators.required]],
      buildingId: [{ value: this.expense.buildingId, disabled: this.readOnly }],
      customerId: [{ value: this.expense.customerId, disabled: this.readOnly }],
      type: [{ value: this.expense.type, disabled: this.readOnly }, [Validators.required]],
      date: [{ value: this.expense.date, disabled: this.readOnly }, [Validators.required]],
      reference: [{ value: this.expense.reference, disabled: this.readOnly }],
      amount: [{ value: this.expense.amount, disabled: this.readOnly }, [Validators.required]],
      vendor: [{ value: this.expense.vendor, disabled: this.readOnly }],
      description: [{ value: this.expense.description, disabled: this.readOnly }],
      buildingFilter: [''],
      transactionNumber: [{ value: this.expense.transactionNumber, disabled: this.readOnly }]
    });
  }

  getCategoriesToArray(): void {
    for (const category in ExpenseCategory) {
      if (typeof ExpenseCategory[category] === 'number') {
        this.categories.push({ id: ExpenseCategory[category] as any, name: category });

      }
    }
  }

  /** BUILDINGS */
  getBuildings(): void {
    if (this.listBuildingsSubscription && this.listBuildingsSubscription.closed) {
      this.listBuildingsSubscription.unsubscribe();
    }

    this.buildingService.getAllAsList('ReadAllCbo', '', 0, 999, null, null)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings.next(this.buildings);
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get buildings', 'Close');
      });
  }

  private filterBuildings(): void {
    if (!this.buildings) {
      return;
    }
    // get the search keyword
    let search = this.getBuildingFilter.value;
    if (!search) {
      this.filteredBuildings.next(this.buildings.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the customers
    this.filteredBuildings.next(
      this.buildings.filter(building => building.name.toLowerCase().indexOf(search) > -1)
    );
  }

  buildingChanged(id: number): void {
  }

  /** CONTRACTS */
  getBuildingContracts(buildingId: number): void {

    this.contracts = [];
    this.expenseForm.patchValue({ contractId: null, customerId: null });

    if (this.listContractsSubscription && this.listContractsSubscription.closed) {
      this.listContractsSubscription.unsubscribe();
    }

    this.contractService.getAllAsListByBuilding(buildingId)
      .subscribe((response: { count: number, payload: ContractBaseModel[] }) => {
        this.contracts = response.payload;
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get contracts', 'Close');
      });
  }

  contractChanged(id: number): void {
    const contract: any = this.contracts.find(c => c.id === id);

    this.expenseForm.patchValue({ customerId: contract.customerId });
  }

  getContracts(buildingId, contractId): void {
    this.contractService.getAllAsListByBuildingWithContract(buildingId, contractId)
      .subscribe((response: { count: number, payload: ContractBaseModel[] }) => {
        this.contracts = response.payload;
        this.filteredContracts.next(this.contracts);
        if (this.contracts.length === 1) {
          const objContract = this.contracts.find(t => t.buildingId === buildingId);
          this.expenseForm.patchValue({
            contractId: objContract.id,
            customerId: objContract.customerId
          });
        }
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get contracts', 'Close');
      });
  }

}
