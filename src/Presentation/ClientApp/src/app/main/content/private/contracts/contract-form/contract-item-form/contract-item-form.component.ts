import { Component, OnInit, ViewEncapsulation, OnDestroy, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { ContractItemBaseModel } from '@app/core/models/contract-item/contract-item-base.model';
import { OfficeTypeFormComponent } from '@app/core/modules/office-type-form/office-type-form.component';
import { fuseAnimations } from '@fuse/animations';
import { Subject, Subscription } from 'rxjs';
import { OfficeTypesService } from '../../../office-types/office-types.service';
import { ContractsService } from '../../contracts.service';

@Component({
  selector: 'app-contract-item-form',
  templateUrl: './contract-item-form.component.html',
  styleUrls: ['./contract-item-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ContractItemFormComponent implements OnInit, OnDestroy {

  contractItem: ContractItemBaseModel;

  contractItemForm: FormGroup;
  action: string;
  dialogTitle: string;
  isLoading = true;

  officeTypeService: any;
  officeServiceTypes: ListItemModel[] = [];
  filteredOfficeServiceTypes: Subject<any[]> = new Subject<any[]>();
  listOfficeServiceTypesSubscription: Subscription;
  officeTypes = ['Medic', 'Warehouse', 'Maintenance'];

  daysPerMonth: number;

  periodicities: string[] = ['Daily', 'Monthly', 'Bi-Monthly', 'Quarterly', 'Bi-Annually', 'Yearly'];
  serviceTypes: any[] = [
    { id: 0, name: 'Hour' },
    { id: 1, name: 'Unit' },
    { id: 2, name: 'Room' },
    { id: 3, name: 'Square Feet' }];

  constructor(
    public dialogRef: MatDialogRef<ContractItemFormComponent>,
    public officeTypeDialogRef: MatDialogRef<OfficeTypeFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private contractService: ContractsService,
    private officeServiceTypeService: OfficeTypesService,
    public snackBar: MatSnackBar,
    public dialog: MatDialog
  ) {

    this.action = data.action;
    this.daysPerMonth = data.daysPerMonth === 0 ? 1 : data.daysPerMonth;

    if (this.action === 'new') {
      this.dialogTitle = 'New Contract Item';
      this.contractItemForm = this.createContractItemForm();
    }
    else {
      this.dialogTitle = 'Edit Contract Item';
      this.contractItem = data.contractItem;
      this.contractItemForm = this.updateContractItemForm();
    }

  }

  ngOnInit(): void {
    this.getAllOfficeServiceTypes();
  }

  ngOnDestroy(): void {

    if (this.listOfficeServiceTypesSubscription && !this.listOfficeServiceTypesSubscription.closed) {
      this.listOfficeServiceTypesSubscription.unsubscribe();
    }

  }

  createContractItemForm(): FormGroup {
    return this.formBuilder.group({
      quantity: [1, [Validators.required]],
      description: ['', [Validators.required]],
      officeServiceTypeId: 0,
      officeServiceTypeName: [''],
      rate: [0],
      rateType: [0],
      ratePeriodicity: [''],
      hours: [0],
      amount: [0],
      rooms: [0],
      squareFeet: [0],
      dailyRate: ['', [Validators.required]],
      monthlyRate: ['', [Validators.required]],
      biMonthlyRate: ['', [Validators.required]],
      quarterly: ['', [Validators.required]],
      biAnnually: ['', [Validators.required]],
      yearlyRate: ['', [Validators.required]],
      updatePrepopulatedItems: [false]
    });
  }

  updateContractItemForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.contractItem.id],
      quantity: [this.contractItem.quantity, [Validators.required]],
      description: [this.contractItem.description, [Validators.required]],
      contractId: [this.contractItem.contractId],
      officeServiceTypeId: this.contractItem.officeServiceTypeId,
      officeServiceTypeName: [this.contractItem.officeServiceTypeName],
      rate: [this.contractItem.rate],
      rateType: [this.contractItem.rateType],
      ratePeriodicity: [this.contractItem.ratePeriodicity],
      hours: [this.contractItem.hours],
      amount: [this.contractItem.amount],
      rooms: [this.contractItem.rooms],
      squareFeet: [this.contractItem.squareFeet],
      dailyRate: ['', [Validators.required]],
      monthlyRate: ['', [Validators.required]],
      biMonthlyRate: ['', [Validators.required]],
      quarterly: ['', [Validators.required]],
      biAnnually: ['', [Validators.required]],
      yearlyRate: ['', [Validators.required]],
      updatePrepopulatedItems: [false]
    });
  }

  selectChangedOfficeServiceType(selectedId: number): void {

    this.cleanPeriodicityFormValues();
    if (selectedId !== null) {
      this.officeTypeService = this.officeServiceTypes.find(o => o.id === selectedId);
      if (this.officeTypeService) {
        if (this.officeTypeService.id === (this.contractItem ? this.contractItem.officeServiceTypeId : 0)) {
          this.officeTypeService.rateType = this.contractItem.rateType;
          this.officeTypeService.periodicity = this.contractItem.ratePeriodicity;
        }
        this.contractItemForm.patchValue({
          officeServiceTypeId: this.officeTypeService.id,
          officeServiceTypeName: this.officeTypeService.name,
          rate: this.contractItem ? this.contractItem.rate : this.officeTypeService.rate,
          rateType: this.officeTypeService.rateType,
          ratePeriodicity: this.officeTypeService.periodicity
        });
        if (this.contractItemForm) {
          let value = 0;
          if (this.contractItemForm.get('rateType').value === 0) {
            value = this.contractItemForm.get('hours').value;
          } else if (this.contractItemForm.get('rateType').value === 1) {
            value = 1;
          } else if (this.contractItemForm.get('rateType').value === 2) {
            value = this.contractItemForm.get('rooms').value;
          } else if (this.contractItemForm.get('rateType').value === 3) {
            value = this.contractItemForm.get('squareFeet').value;
          }
          this.calculateTotals(value);
        }
      } else {
        this.snackBar.open('The assigned service is not available', 'close', { duration: 3000 });
      }
    }
  }

  // Office Service Type
  getAllOfficeServiceTypes(): void {

    if (this.listOfficeServiceTypesSubscription && !this.listOfficeServiceTypesSubscription.closed) {
      this.listOfficeServiceTypesSubscription.unsubscribe();
    }

    this.listOfficeServiceTypesSubscription = this.officeServiceTypeService
      .getAllAsList('readall', '', 0, 999, 1, { 'isEnabled': '1' })
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.officeServiceTypes = response.payload;
        this.filteredOfficeServiceTypes.next(this.officeServiceTypes);
        this.isLoading = false;

        if (this.contractItem) {

          this.selectChangedOfficeServiceType(this.contractItem.officeServiceTypeId);
        }
      });
  }

  calculateTotals(value: any): void {
    let rate: number;
    let quantity: number;

    if (Number(this.contractItemForm.controls['quantity'].value)) {
      quantity = this.contractItemForm.controls['quantity'].value;
    } else {
      this.cleanPeriodicityFormValues();
      return;
    }

    if (Number(this.contractItemForm.controls['rate'].value)) {
      rate = this.contractItemForm.controls['rate'].value;
    } else {
      this.cleanPeriodicityFormValues();
      return;
    }

    if (!Number(value)) {
      this.cleanPeriodicityFormValues();
      return;
    }

    this.calculateValues(value, rate, quantity);
  }

  /**
   * Calculates the daily, monthly and yearly rate of the item service according the frequency.
   * @param value
   * @param rate
   */
  calculateValues(value: number, rate: number, quantity: number): void {
    let dailyRate = 0;
    let monthlyRate = 0;
    let yearlyRate = 0;
    let biMonthlyRate = 0;
    let quarterly = 0;
    let biAnnually = 0;

    switch (this.officeTypeService.periodicity) {
      case 'Daily':
        dailyRate = (value * rate);
        monthlyRate = (dailyRate * this.daysPerMonth);
        biMonthlyRate = monthlyRate * 2;
        quarterly = monthlyRate * 3;
        biAnnually = quarterly * 2;
        yearlyRate = (monthlyRate * 12);
        break;

      case 'Monthly':
        monthlyRate = (value * rate);
        biMonthlyRate = monthlyRate * 2;
        quarterly = monthlyRate * 3;
        biAnnually = quarterly * 2;
        dailyRate = (monthlyRate / this.daysPerMonth);
        yearlyRate = (monthlyRate * 12);
        break;

      case 'Bi-Monthly':
        biMonthlyRate = (value * rate);
        monthlyRate = biMonthlyRate / 2;
        quarterly = monthlyRate * 3;
        biAnnually = quarterly * 2;
        dailyRate = (monthlyRate / this.daysPerMonth);
        yearlyRate = (monthlyRate * 12);
        break;

      case 'Quarterly':
        quarterly = (value * rate);
        biAnnually = quarterly * 2;
        yearlyRate = biAnnually * 2;
        monthlyRate = yearlyRate / 12;
        biMonthlyRate = monthlyRate * 2;
        dailyRate = (monthlyRate / 21.5);
        break;

      case 'Bi-Annually':
        biAnnually = (value * rate);
        yearlyRate = biAnnually * 2;
        monthlyRate = yearlyRate / 12;
        dailyRate = monthlyRate / this.daysPerMonth;
        biMonthlyRate = monthlyRate * 2;
        quarterly = monthlyRate * 3;
        break;

      case 'Yearly':
        yearlyRate = (value * rate);
        monthlyRate = (yearlyRate / 12);
        biMonthlyRate = monthlyRate * 2;
        quarterly = monthlyRate * 3;
        biAnnually = quarterly * 2;
        dailyRate = (monthlyRate / this.daysPerMonth);
        break;
    }

    dailyRate = (dailyRate * quantity);
    monthlyRate = (monthlyRate * quantity);
    biMonthlyRate = (biMonthlyRate * quantity);
    quarterly = (quarterly * quantity);
    biAnnually = (biAnnually * quantity);
    yearlyRate = (yearlyRate * quantity);

    this.setPeriodicityFormValues(
      Math.round(dailyRate * 100) / 100,
      Math.round(monthlyRate * 100) / 100,
      Math.round(biMonthlyRate * 100) / 100,
      Math.round(quarterly * 100) / 100,
      Math.round(biAnnually * 100) / 100,
      Math.round(yearlyRate * 100) / 100);
  }

  cleanPeriodicityFormValues(): void {
    this.contractItemForm.patchValue({
      dailyRate: '',
      monthlyRate: '',
      biMonthlyRate: '',
      quarterly: '',
      biAnnually: '',
      yearlyRate: ''
    });
  }

  setPeriodicityFormValues(daily: number, monthly: number, biMonthlyRate: number, quarterly: number, biAnnually: number, yearly: number): void {
    this.contractItemForm.patchValue({
      dailyRate: daily,
      monthlyRate: monthly,
      biMonthlyRate: biMonthlyRate,
      quarterly: quarterly,
      biAnnually: biAnnually,
      yearlyRate: yearly
    });
  }

  newOfficeServiceType(): void {
    this.officeTypeDialogRef = this.dialog.open(OfficeTypeFormComponent, {
      panelClass: 'office-type-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.officeTypeDialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        this.officeServiceTypeService.createElement(response.getRawValue())
          .then(
            () => {
              this.snackBar.open('Office type created successfully!!!', 'close', { duration: 1000 });
              this.getAllOfficeServiceTypes();
            },
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });
  }

  // SAVE
  submit(): void {
    this.contractItemForm.patchValue({
      rateType: this.officeTypeService.rateType,
      ratePeriodicity: this.officeTypeService.periodicity
    });
    this.dialogRef.close(this.contractItemForm);
  }

}
