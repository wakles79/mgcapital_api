import { Component, OnInit, ViewEncapsulation, OnDestroy, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BuildingBaseModel } from '@app/core/models/building/building-base.model';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';
import { ContractOfficeSpaceModel } from '@app/core/models/contract/contract-office-space.model';
import { ContractSummaryModel } from '@app/core/models/contract/contract-summary.model';
import { BuildingFormComponent } from '@app/core/modules/building-form/building-form/building-form.component';
import { CustomerFormComponent } from '@app/core/modules/customer-form/customer-form.component';
import { CustomersBaseService } from '@app/core/services/customers.service';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { BuildingsService } from '../../buildings/buildings.service';
import { CustomersService } from '../../customers/customers.service';
import { ContractsService } from '../contracts.service';
import { OfficeSpaceFormComponent } from '../office-space-form/office-space-form.component';

export interface Transaction {
  item: string;
  cost: number;
}

@Component({
  selector: 'app-contract-form',
  templateUrl: './contract-form.component.html',
  styleUrls: ['./contract-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ContractFormComponent implements OnInit, OnDestroy {

  dialogTitle: string;
  action: string;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  contractForm: FormGroup;
  contract: ContractBaseModel;

  customers: ListItemModel[] = [];
  filteredCustomers: Subject<any[]> = new Subject<any[]>();
  listCustomersSubscription: Subscription;
  currentCustomerSelected: -1;
  assignedCustomer: ListItemModel;

  createdBuilding: BuildingBaseModel;
  buildings: ListItemModel[] = [];
  filteredBuildings: Subject<any[]> = new Subject<any[]>();
  listBuildingsSubscription: Subscription;
  assignedBuilding: ListItemModel;
  availableBuilding = true;

  contractSummary: ContractSummaryModel;

  dialogResponse: any;

  officeSpaces: ContractOfficeSpaceModel[] = [];
  officeSpaceFormDialog: MatDialogRef<OfficeSpaceFormComponent>;

  contractStatus: any = [
    { 'id': 0, 'name': 'Pending' },
    { 'id': 1, 'name': 'Active' },
    { 'id': 2, 'name': 'Completed' },
    { 'id': 3, 'name': 'Declined' }];

  private _onDestroy = new Subject<void>();

  invalidCode = false;

  constructor(
    public dialogRef: MatDialogRef<ContractFormComponent>,
    public customerDialogRef: MatDialogRef<CustomerFormComponent>,
    public buildingDialogRef: MatDialogRef<BuildingFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private contractService: ContractsService,
    private customerService: CustomersService,
    private customerBaseService: CustomersBaseService,
    private buildingService: BuildingsService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {
    this.action = data.action;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Budget';
      this.contract = data.contract;
      this.getAllOfficeSpaces(this.contract.id);
      this.contractForm = this.updateContractForm();

      this.contractService.getActiveContractBuilding(this.contract.buildingId, this.contract.id)
        .subscribe((response: ContractSummaryModel) => {
          if (response) {
            this.contractForm.get('status').disable();
          }
        });

      this.getContractBuilding(this.contract.buildingId);
      this.getContractCustomer(this.contract.customerId);
    }
    else if (this.action === 'new') {
      this.dialogTitle = 'New Budget';
      this.contractForm = this.createContractForm();
    } else if (this.action === 'clone') {
      this.dialogTitle = 'Clonse Budget';
      this.contractForm = this.cloneContractForm();
    }
  }

  ngOnInit(): void {

    if (this.action === 'new') {
      this.getCustomers();
      this.getBuildings();
    }

    this.contractForm.get('contractNumber').valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged())
      .subscribe(value => {
        if (value) {
          this.contractService.validateAvailabilityContractNumber(value, this.contract ? this.contract.id : -1)
            .subscribe((result: string) => {
              if (result === 'Existing') {
                this.invalidCode = true;
                this.contractForm.get('contractNumber').setErrors({ 'incorrect': true });
              } else {
                this.invalidCode = false;
                this.contractForm.get('contractNumber').setErrors(null);
              }
            }, (error) => {
              console.log('Error');
            });
        }
      });

    this.customerIdCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterCustomers();
      });

    this.buildingIdCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBuildings();
      });

    this.contractForm.get('unoccupiedSquareFeets').valueChanges
      .subscribe((value) => {

        if (!Number(value)) {
          return;
        }

        this.updateTotalSquareFeet();
      });
  }

  ngOnDestroy(): void {
    if (this.listCustomersSubscription && !this.listCustomersSubscription.closed) {
      this.listCustomersSubscription.unsubscribe();
    }

    if (this.listBuildingsSubscription && this.listBuildingsSubscription.closed) {
      this.listBuildingsSubscription.unsubscribe();
    }
  }

  /** FORMS METHODS */
  createContractForm(): FormGroup {
    return this.formBuilder.group({
      customerIdCtrl: [''],
      customerId: ['', [Validators.required]],
      buildingIdCtrl: [''],
      buildingId: ['', [Validators.required]],
      contractNumber: ['', [Validators.required]],
      // description: [''],
      // daysPerMonth: [21.5, [Validators.required]],
      // numberOfPeople: [1, [Validators.required]],
      // numberOfRestrooms: [0],
      // frequencyPerYear: [0],
      // expirationDate: ['', [Validators.required]],
      // busySquare: [{ value: '', disabled: true }],
      // productionRate: [6500],
      // unoccupiedSquareFeets: [0],
      // totalSquareFeet: [{ value: 0, disabled: true }],
      status: [0]
    });
  }

  updateContractForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.contract.id],
      customerIdCtrl: [''],
      customerId: [this.contract.customerId, [Validators.required]],
      buildingIdCtrl: [''],
      buildingId: [this.contract.buildingId, [Validators.required]],
      contractNumber: [this.contract.contractNumber, [Validators.required]],
      description: [this.contract.description],
      daysPerMonth: [this.contract.daysPerMonth, [Validators.required]],
      numberOfPeople: [this.contract.numberOfPeople, [Validators.required]],
      numberOfRestrooms: [this.contract.numberOfRestrooms],
      frequencyPerYear: [this.contract.frequencyPerYear],
      expirationDate: [this.contract.expirationDate],
      status: [this.contract.status],
      busySquare: [{ value: '', disabled: true }],
      productionRate: [this.contract.productionRate],
      unoccupiedSquareFeets: [this.contract.unoccupiedSquareFeets],
      totalSquareFeet: [{ value: 0, disabled: true }],
      editionCompleted: [this.contract.editionCompleted]
    });
  }

  cloneContractForm(): FormGroup {
    return this.formBuilder.group({});
  }

  /** CUSTOMERS */
  getCustomers(): void {
    const idCustomer = null;

    if (this.listCustomersSubscription && this.listCustomersSubscription.closed) {
      this.listCustomersSubscription.unsubscribe();
    }

    this.listCustomersSubscription = this.customerService.getAllAsList('readallcbo', '', 0, 99999, idCustomer)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.customers = response.payload;
        this.filteredCustomers.next(this.customers);
      });
  }

  getContractCustomer(customerId: number): void {
    this.customerService.get(customerId).subscribe((customer: any) => {
      if (customer) {
        this.assignedCustomer = new ListItemModel();
        this.assignedCustomer.id = customer.id;
        this.assignedCustomer.name = customer.name;
      }
    }, () => {
      this.snackBar.open('Oops, error getting the assigned Customer', 'close', { duration: 3000 });
    });
  }

  newCustomer(): void {
    this.customerDialogRef = this.dialog.open(CustomerFormComponent, {
      panelClass: 'customer-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.customerDialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        this.customerService.createCustomer(response.getRawValue())
          .then(
            () => {
              this.snackBar.open('Customer created successfully!!!', 'close', { duration: 1000 });
              this.getCustomers();
            },
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });
  }

  get customerIdCtrl(): AbstractControl {
    return this.contractForm.get('customerIdCtrl');
  }

  private filterCustomers(): void {
    if (!this.customers) {
      return;
    }
    // get the search keyword
    let search = this.customerIdCtrl.value;
    if (!search) {
      this.filteredCustomers.next(this.customers.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the customers
    this.filteredCustomers.next(
      this.customers.filter(customer => customer.name.toLowerCase().indexOf(search) > -1)
    );
  }

  /** BUILDINGS */
  getBuildings(): void {

    if (this.listBuildingsSubscription && this.listBuildingsSubscription.closed) {
      this.listBuildingsSubscription.unsubscribe();
    }

    this.listBuildingsSubscription = this.buildingService.getAllAsList('ReadAllCbo', '', 0, 99999, null)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.buildings = response.payload;
        if (this.createdBuilding) {
          this.createdBuilding = null;
          this.contractForm.patchValue({ buildingId: '' });
        }
        this.filteredBuildings.next(this.buildings);
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get customer buildings', 'Close');
      });
  }

  getContractBuilding(buildingId: number): void {
    this.buildingService.get(buildingId, 'update').subscribe((building: any) => {
      if (building) {
        this.assignedBuilding = new ListItemModel();
        this.assignedBuilding.id = building.id;
        this.assignedBuilding.name = building.name;
      }
    }, () => {
      this.snackBar.open('Oops, error getting the assigned Building', 'close', { duration: 3000 });
    });
  }

  newBuilding(): void {
    this.buildingDialogRef = this.dialog.open(BuildingFormComponent, {
      panelClass: 'building-form-dialog',
      data: {
        action: 'new'
      }
    });
    this.buildingDialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        // Create building

        this.createdBuilding = new BuildingBaseModel(response.getRawValue());
        this.createdBuilding.id = 0;

        const buildingItem = new ListItemModel();
        buildingItem.id = 0;
        buildingItem.name = this.createdBuilding.name;
        this.buildings.push(buildingItem);
        this.contractForm.patchValue({ buildingId: 0 });
      });
  }

  get buildingIdCtrl(): AbstractControl {
    return this.contractForm.get('buildingIdCtrl');
  }

  private filterBuildings(): void {
    if (!this.buildings) {
      return;
    }
    // get the search keyword
    let search = this.buildingIdCtrl.value;
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

  buildingChange(event): void {
    this.contractSummary = null;
    // this.contractForm.get('buildingId').setErrors(null);

    this.contractService.getActiveContractBuilding(event.value)
      .subscribe((response: ContractSummaryModel) => {
        if (!response) {
          this.availableBuilding = true;
          return;
        }

        // this.contractForm.get('buildingId').setErrors({ 'incorrect': true });
        this.contractSummary = response;
        this.availableBuilding = false;
      }, (error) => {
        this.availableBuilding = true;
        this.snackBar.open('Ops! Unable to validate building availability', 'Close', { duration: 400 });
      });
  }

  /** CONTRACT */
  saveContract(contractFormData: FormGroup): void {
    const contract = new ContractBaseModel(contractFormData.getRawValue());

    contract.officeSpaces = this.officeSpaces;

    if (contract.buildingId !== null) {
      this.contractService.createElement(contract)
        .then(
          (response) => {
            this.dialogRef.close('success');
          },
          () => this.dialogRef.close('error'))
        .catch(() => this.dialogRef.close('error'));
    } else {
      this.buildingService.createElement(this.createdBuilding)
        .then(
          (buildingResponse) => {
            const createdBuilding = buildingResponse['body'];
            contract.buildingId = createdBuilding.id;
            this.contractService.createElement(contract)
              .then(
                (response) => {
                  this.dialogRef.close('success');
                },
                () => this.dialogRef.close('error'))
              .catch(() => this.dialogRef.close('error'));
          },
          () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
        .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
    }

    return;
  }

  updateContract(contractFormData: FormGroup): void {
    const contract = new ContractBaseModel(contractFormData.getRawValue());

    contract.officeSpaces = this.officeSpaces;

    this.contractService.updateElement(contract)
      .then(
        () => {
          this.dialogRef.close('success');
        },
        () => this.dialogRef.close('error'))
      .catch(() => this.dialogRef.close('error'));
  }

  /** OFFICE SPACE */
  newOfficeSpace(): void {

    let addedTypes = '';
    this.officeSpaces.forEach((i: ContractOfficeSpaceModel) => {
      addedTypes += (addedTypes ? ',' + i.officeTypeId : i.officeTypeId.toString());
    });

    this.officeSpaceFormDialog = this.dialog.open(OfficeSpaceFormComponent, {
      panelClass: 'office-space-form-dialog',
      data: {
        action: 'new',
        addedTypes: addedTypes
      }
    });

    this.officeSpaceFormDialog.afterClosed().subscribe((response: FormGroup) => {
      if (!response) {
        return;
      }

      const officeSpace = new ContractOfficeSpaceModel(response.getRawValue());

      if (this.action === 'new') {
        officeSpace.id = this.getLastIdFromOfficeSpaceList();
        this.officeSpaces.push(officeSpace);
      } else {
        officeSpace.contractId = this.contract.id;
        this.contractService.addOfficeSpace(officeSpace)
          .subscribe(() => {
            this.getAllOfficeSpaces(this.contract.id);
          }, (error) => {
            this.snackBar.open('Ops! Error when trying to add office space', 'Close');
          });
      }

      this.updateBusySquare();
    });
  }

  editOfficeSpace(officeSpace: ContractOfficeSpaceModel): void {

    let addedTypes = '';
    this.officeSpaces.forEach((i: ContractOfficeSpaceModel) => {

      if (i.officeTypeId !== officeSpace.officeTypeId) {
        addedTypes += (addedTypes ? ',' + i.officeTypeId : i.officeTypeId.toString());
      }

    });

    if (this.action === 'new') {
      this.officeSpaceFormDialog = this.dialog.open(OfficeSpaceFormComponent, {
        panelClass: 'office-space-form-dialog',
        data: {
          action: 'edit',
          addedTypes: addedTypes,
          officeSpace: officeSpace
        }
      });

      this.officeSpaceFormDialog.afterClosed()
        .subscribe((response: FormGroup) => {
          if (!response) {
            return;
          }

          const officeSpaceUpdated = new ContractOfficeSpaceModel(response.getRawValue());
          const index = this.officeSpaces.findIndex(o => o.id === officeSpaceUpdated.id);
          this.officeSpaces[index] = officeSpaceUpdated;

          this.updateBusySquare();
        });
    } else {
      this.contractService.getOfficeSpace(officeSpace.id).subscribe(
        (response: ContractOfficeSpaceModel) => {
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

              const officeSpaceUpdated = new ContractOfficeSpaceModel(responseForm.getRawValue());
              officeSpaceUpdated.contractId = this.contract.id;
              this.contractService.updateOfficeSpace(officeSpaceUpdated)
                .subscribe(() => {
                  this.getAllOfficeSpaces(this.contract.id);
                }, (error) => {
                  this.snackBar.open('Ops! Error when trying to update office space', 'Close');
                });

            });
        }, (error) => {
          this.snackBar.open('Ops! Error when trying to get office space', 'Close');
        });
    }

  }

  deleteOfficeSpace(officeSpace: ContractOfficeSpaceModel): void {

    event.stopPropagation();
    if (this.action === 'new') {
      this.officeSpaces = this.officeSpaces.filter(o => o.id !== officeSpace.id);
    } else {

    }

    this.updateBusySquare();
  }

  updateBusySquare(): void {
    let total = 0;
    this.officeSpaces.forEach((item: ContractOfficeSpaceModel) => {
      total += item.squareFeet;
    });
    this.contractForm
      .patchValue({
        busySquare: total
      });

    this.updateTotalSquareFeet();
  }

  updateTotalSquareFeet(): void {
    const occupiedSpace = Number(this.contractForm.get('busySquare').value) ? this.contractForm.get('busySquare').value : 0;
    const unoccupiedSpace = Number(this.contractForm.get('unoccupiedSquareFeets').value) ? this.contractForm.get('unoccupiedSquareFeets').value : 0;

    this.contractForm.patchValue({
      totalSquareFeet: occupiedSpace + unoccupiedSpace
    });
  }

  getLastIdFromOfficeSpaceList(): number {
    let id = 0;
    this.officeSpaces.forEach((item: ContractOfficeSpaceModel) => {
      if (item.id > id) {
        id = item.id;
      }
    });
    return id + 1;
  }

  getAllOfficeSpaces(contractId: number): void {
    this.contractService.getAllOfficeSpaces(contractId).subscribe(
      (response: { count: number, payload: ContractOfficeSpaceModel[] }) => {
        this.officeSpaces = response.payload;
        this.updateBusySquare();
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get office spaces', 'Close');
      });
  }

}
