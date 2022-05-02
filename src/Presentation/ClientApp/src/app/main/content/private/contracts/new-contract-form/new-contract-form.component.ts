import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BuildingBaseModel } from '@app/core/models/building/building-base.model';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';
import { ContractSummaryModel } from '@app/core/models/contract/contract-summary.model';
import { BuildingFormComponent } from '@app/core/modules/building-form/building-form/building-form.component';
import { CustomerFormComponent } from '@app/core/modules/customer-form/customer-form.component';
import { fuseAnimations } from '@fuse/animations';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { BuildingsService } from '../../buildings/buildings.service';
import { CustomersService } from '../../customers/customers.service';
import { ContractsService } from '../contracts.service';

@Component({
  selector: 'app-new-contract-form',
  templateUrl: './new-contract-form.component.html',
  styleUrls: ['./new-contract-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class NewContractFormComponent implements OnInit {

  contractForm: FormGroup;

  customers: ListItemModel[] = [];
  filteredCustomers: Subject<any[]> = new Subject<any[]>();
  listCustomersSubscription: Subscription;

  createdBuilding: BuildingBaseModel;
  buildings: ListItemModel[] = [];
  filteredBuildings: Subject<any[]> = new Subject<any[]>();
  listBuildingsSubscription: Subscription;
  availableBuilding = true;

  contractSummary: ContractSummaryModel;

  invalidCode = false;

  private _onDestroy = new Subject<void>();

  constructor(
    public dialogRef: MatDialogRef<NewContractFormComponent>,
    public customerDialogRef: MatDialogRef<CustomerFormComponent>,
    public buildingDialogRef: MatDialogRef<BuildingFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private contractService: ContractsService,
    private customerService: CustomersService,
    private buildingService: BuildingsService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {

    this.contractForm = this.createContractForm();
  }

  ngOnInit() {

    this.getCustomers();
    this.getBuildings();

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

    this.contractForm.get('contractNumber').valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged())
      .subscribe(value => {
        if (value) {
          this.contractService.validateAvailabilityContractNumber(value, -1)
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

  }

  /** FORM */
  createContractForm() {
    return this.formBuilder.group({
      customerIdCtrl: [''],
      customerId: ['', [Validators.required]],
      buildingIdCtrl: [''],
      buildingId: ['', [Validators.required]],
      contractNumber: ['', [Validators.required]],
      status: [0]
    });
  }

  /** BUILDINGS */
  getBuildings() {
    if (this.listBuildingsSubscription && this.listBuildingsSubscription.closed) {
      this.listBuildingsSubscription.unsubscribe();
    }

    this.listBuildingsSubscription = this.buildingService.getAllAsList('ReadAllCbo', '', 0, 99999, null)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings.next(this.buildings);
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get customer buildings', 'Close');
      });
  }

  newBuilding() {
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
        this.filteredBuildings.next(this.buildings);
        this.contractForm.patchValue({ buildingId: 0 });
      });
  }

  get buildingIdCtrl() {
    return this.contractForm.get('buildingIdCtrl');
  }

  private filterBuildings() {
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

  buildingChange(event) {
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

  /** CUSTOMERS */
  getCustomers() {
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

  newCustomer() {
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

  get customerIdCtrl() {
    return this.contractForm.get('customerIdCtrl');
  }

  private filterCustomers() {
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

  /**CONTRACT */
  saveContract() {
    const contract = new ContractBaseModel(this.contractForm.getRawValue());
    contract.officeSpaces = [];

    const result: { message: string, id: number } = { message: '', id: 0 };

    if (contract.buildingId === null) {
      this.buildingService.createElement(this.createdBuilding)
        .then(
          (buildingResponse) => {
            const newBuilding = buildingResponse['body'];
            contract.buildingId = newBuilding.id;
            this.contractService.createElement(contract)
              .then(
                (response) => {
                  const newContract = response['body'];
                  result.message = 'success';
                  result.id = newContract.id;
                  this.dialogRef.close(result);
                },
                () => this.dialogRef.close(result))
              .catch(() => this.dialogRef.close(result));
          },
          () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
        .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
    } else {
      this.contractService.createElement(contract)
        .then(
          (response) => {
            const newContract = response['body'];
            result.message = 'success';
            result.id = newContract.id;
            this.dialogRef.close(result);
          },
          () => this.dialogRef.close(result))
        .catch(() => this.dialogRef.close(result));
    }
  }

}
