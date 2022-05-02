import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BuildingUpdateModel } from '@app/core/models/building/building-update.model';
import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';
import { ContractOfficeSpaceModel } from '@app/core/models/contract/contract-office-space.model';
import { ContractSummaryModel } from '@app/core/models/contract/contract-summary.model';
import { CustomerBaseModel } from '@app/core/models/customer/customer-base.model';
import { fuseAnimations } from '@fuse/animations';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { BuildingsService } from '../../buildings/buildings.service';
import { CustomersService } from '../../customers/customers.service';
import { ContractsService } from '../contracts.service';
import { OfficeSpaceFormComponent } from '../office-space-form/office-space-form.component';

@Component({
  selector: 'app-edit-contract-form',
  templateUrl: './edit-contract-form.component.html',
  styleUrls: ['./edit-contract-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class EditContractFormComponent implements OnInit {

  dialogTitle = 'Edit Budget';

  contractForm: FormGroup;
  contract: ContractBaseModel;

  assignedCustomer: CustomerBaseModel;
  assignedBuilding: BuildingUpdateModel;

  contractStatus: any = [
    { 'id': 0, 'name': 'Pending' },
    { 'id': 1, 'name': 'Active' },
    { 'id': 2, 'name': 'Completed' },
    { 'id': 3, 'name': 'Declined' }];

  contractSummary: ContractSummaryModel;
  invalidCode = false;

  officeSpaces: ContractOfficeSpaceModel[] = [];
  officeSpaceFormDialog: MatDialogRef<OfficeSpaceFormComponent>;

  private _onDestroy = new Subject<void>();

  constructor(
    public dialogRef: MatDialogRef<EditContractFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private contractService: ContractsService,
    private customerService: CustomersService,
    private buildingService: BuildingsService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {

    this.contract = data.contract;
    this.contractForm = this.updateContractForm();

    this.contractService.getActiveContractBuilding(this.contract.buildingId, this.contract.id)
      .subscribe((response: ContractSummaryModel) => {
        if (response) {
          this.contractForm.get('status').disable();
        }
      });

    if (this.contract.editionCompleted) {
      this.getAllRevenueSquareFeet(this.contract.id);
    } else {
      this.getAllOfficeSpaces(this.contract.id);
    }

    this.getCustomer(this.contract.customerId);
    this.getBuilding(this.contract.buildingId);

  }

  ngOnInit(): void {

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

    this.contractForm.get('unoccupiedSquareFeets').valueChanges
      .subscribe((value) => {

        if (!Number(value)) {
          return;
        }

        this.updateTotalSquareFeet();
      });
  }

  /** FORM */
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
      editionCompleted: [this.contract.editionCompleted],
      updatePrepopulatedItems: true
    });
  }

  /** BUILDING */
  getBuilding(id: number): void {
    this.buildingService.get(id, 'update')
      .subscribe((buildingData: BuildingUpdateModel) => {
        if (buildingData) {
          this.assignedBuilding = buildingData;
        } else {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      }, (error) => {
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

  /** CUSTOMER */
  getCustomer(id: number): void {
    this.customerService.get(id)
      .subscribe((response: CustomerBaseModel) => {
        if (response) {
          this.assignedCustomer = response;
        } else {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      }, (error) => {
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

  /** CONTRACT */


  /** OFFICE SPACE */
  newOfficeSpace(): void {

    if (this.contract.editionCompleted) {
      return;
    }

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

      officeSpace.contractId = this.contract.id;
      this.contractService.addOfficeSpace(officeSpace)
        .subscribe(() => {
          this.getAllOfficeSpaces(this.contract.id);
        }, (error) => {
          this.snackBar.open('Ops! Error when trying to add office space', 'Close');
        });

      this.updateBusySquare();
    });
  }

  editOfficeSpace(officeSpace: ContractOfficeSpaceModel): void {

    if (this.contract.editionCompleted) {
      return;
    }

    let addedTypes = '';
    this.officeSpaces.forEach((i: ContractOfficeSpaceModel) => {

      if (i.officeTypeId !== officeSpace.officeTypeId) {
        addedTypes += (addedTypes ? ',' + i.officeTypeId : i.officeTypeId.toString());
      }

    });

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

  deleteOfficeSpace(officeSpace: ContractOfficeSpaceModel): void {
    event.stopPropagation();

    if (this.contract.editionCompleted) {
      return;
    }

    this.officeSpaces = this.officeSpaces.filter(o => o.id !== officeSpace.id);

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

  getAllRevenueSquareFeet(contractId: number): void {
    this.contractService.getAllRevenueSquareFeet(contractId).subscribe(
      (response: ContractOfficeSpaceModel[]) => {
        this.officeSpaces = response;
        this.updateBusySquare();
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get office spaces', 'Close');
      });
  }

}
