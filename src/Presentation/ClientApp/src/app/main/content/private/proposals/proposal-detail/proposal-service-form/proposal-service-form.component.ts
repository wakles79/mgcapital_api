import { Component, OnInit, ViewEncapsulation, OnDestroy, Inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { ProposalServiceBaseModel } from '@app/core/models/proposal-service/proposal-service-base.model';
import { fuseAnimations } from '@fuse/animations';
import { Subject, Subscription } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BuildingsService } from '../../../buildings/buildings.service';
import { OfficeTypesService } from '../../../office-types/office-types.service';
import { OfficeTypeFormComponent } from '@app/core/modules/office-type-form/office-type-form.component';

@Component({
  selector: 'app-proposal-service-form',
  templateUrl: './proposal-service-form.component.html',
  styleUrls: ['./proposal-service-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ProposalServiceFormComponent implements OnInit, OnDestroy {

  proposalId: number;
  proposalServiceForm: FormGroup;
  proposalService: ProposalServiceBaseModel;

  action: string;
  dialogTitle: string;
  private _onDestroy = new Subject<void>();

  get buildingFilter(): AbstractControl { return this.proposalServiceForm.get('buildingFilter'); }
  buildings: ListItemModel[] = [];
  filteredBuildings: Subject<any[]> = new Subject<any[]>();
  listBuildingsSubscription: Subscription;

  get officeServiceTypeFilter(): AbstractControl { return this.proposalServiceForm.get('officeServiceTypeFilter'); }
  officeServiceTypes: ListItemModel[] = [];
  filteredOfficeServiceTypes: Subject<any[]> = new Subject<any[]>();
  listOfficeServiceTypesSubscription: Subscription;
  selectedOfficeServiceType: any;

  get quantityLineItem(): AbstractControl { return this.proposalServiceForm.get('quantity'); }
  get rateLineItem(): AbstractControl { return this.proposalServiceForm.get('rate'); }
  get totalLineItem(): AbstractControl { return this.proposalServiceForm.get('total'); }

  private officeTypeDialogRef: any;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    public dialogRef: MatDialogRef<ProposalServiceFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private buildingService: BuildingsService,
    private officeServiceTypeService: OfficeTypesService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.action = data.action;

    if (this.action === 'new') {
      this.dialogTitle = 'New Line Item';
      this.proposalId = data.proposalId;
      this.proposalServiceForm = this.createProposalServiceForm();
    } else {
      this.dialogTitle = 'Update Line Item';
      this.proposalService = data.proposalService;
      this.proposalServiceForm = this.updateProposalServiceForm();
    }
  }

  ngOnInit(): void {
    this.getBuildings();

    this.getOfficeServiceTypes();

    this.buildingFilter.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBuildings();
      });

    this.officeServiceTypeFilter.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterOfficeServiceTypes();
      });
  }

  ngOnDestroy(): void {
    if (this.listBuildingsSubscription && !this.listBuildingsSubscription.closed) {
      this.listBuildingsSubscription.unsubscribe();
    }

    if (this.listOfficeServiceTypesSubscription && !this.listOfficeServiceTypesSubscription.closed) {
      this.listOfficeServiceTypesSubscription.unsubscribe();
    }
  }

  /** FORM */
  createProposalServiceForm(): FormGroup {
    return this.formBuilder.group({
      proposalId: [this.proposalId],
      buildingId: ['', Validators.required],
      buildingName: [''],
      officeServiceTypeId: ['', Validators.required],
      quantity: [1, Validators.required],
      requesterName: ['', Validators.required],
      description: [''],
      location: ['', Validators.required],
      rate: [0, Validators.required],
      total: [0, Validators.required],
      dateToDelivery: [''],
      buildingFilter: [''],
      officeServiceTypeFilter: ['']
    });
  }

  updateProposalServiceForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.proposalService.id],
      proposalId: [this.proposalService.proposalId, Validators.required],
      buildingId: [this.proposalService.buildingId, Validators.required],
      buildingName: [this.proposalService.buildingName],
      officeServiceTypeId: [this.proposalService.officeServiceTypeId, Validators.required],
      quantity: [this.proposalService.quantity, Validators.required],
      requesterName: [this.proposalService.requesterName, Validators.required],
      description: [this.proposalService.description],
      location: [this.proposalService.location, Validators.required],
      rate: [this.proposalService.rate, Validators.required],
      total: [this.proposalService.rate * this.proposalService.quantity, Validators.required],
      dateToDelivery: [this.proposalService.dateToDelivery ? this.proposalService.dateToDelivery : ''],
      buildingFilter: [''],
      officeServiceTypeFilter: ['']
    });
  }

  /** BUILDINGS */
  getBuildings(): void {
    if (this.listBuildingsSubscription && !this.listBuildingsSubscription.closed) {
      this.listBuildingsSubscription.unsubscribe();
    }

    this.listBuildingsSubscription = this.buildingService.getAllAsList('readallcbo', '', 0, 99999, null)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        const otherItem = new ListItemModel();
        otherItem.id = -1;
        otherItem.name = 'Other';

        this.buildings = response.payload;
        this.buildings.unshift(otherItem);

        this.filteredBuildings.next(this.buildings);
      });
  }

  private filterBuildings(): void {
    if (!this.buildings) {
      return;
    }
    // get the search keyword
    let search = this.buildingFilter.value;
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

  /** OFFICE SERVICE TYPES */
  getOfficeServiceTypes(): void {
    if (this.listOfficeServiceTypesSubscription && !this.listOfficeServiceTypesSubscription.closed) {
      this.listOfficeServiceTypesSubscription.unsubscribe();
    }

    this.listOfficeServiceTypesSubscription = this.officeServiceTypeService.getAllAsList('readall', '', 0, 99999, null)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.officeServiceTypes = response.payload;
        this.filteredOfficeServiceTypes.next(this.officeServiceTypes);
      });
  }

  private filterOfficeServiceTypes(): void {
    if (!this.officeServiceTypes) {
      return;
    }
    // get the search keyword
    let search = this.officeServiceTypeFilter.value;
    if (!search) {
      this.filteredOfficeServiceTypes.next(this.officeServiceTypes.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the customers
    this.filteredOfficeServiceTypes.next(
      this.officeServiceTypes.filter(type => type.name.toLowerCase().indexOf(search) > -1)
    );
  }

  officeServiceTypeChange(id: number): void {
    this.selectedOfficeServiceType = this.officeServiceTypes.find(o => o.id === id);

    this.proposalServiceForm.patchValue({
      rate: this.selectedOfficeServiceType.rate,
      total: (this.selectedOfficeServiceType.rate * (Number(this.quantityLineItem.value) ? this.quantityLineItem.value : 0))
    });
  }

  /** BUTTONS */
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
              this.getOfficeServiceTypes();
            },
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });
  }

  /** INPUTS */
  onQuantityKeyUp(event: any): void {
    const value = event.target.value;

    if (!this.rateLineItem.value) {
      return;
    }

    if (!Number(this.rateLineItem.value)) {
      this.proposalServiceForm.patchValue({ total: '' });
      this.snackBar.open('Error! Invalid rate value', 'close', { duration: 1000 });
      return;
    }

    if (Number(value)) {
      this.proposalServiceForm.patchValue({ total: this.rateLineItem.value * value });
    } else {
      this.proposalServiceForm.patchValue({ total: '' });
      this.snackBar.open('Error! Invalid quantity value', 'close', { duration: 1000 });
    }
  }

  onRateKeyUp(event: any): void {
    const value = event.target.value;

    if (!Number(this.quantityLineItem.value)) {
      this.proposalServiceForm.patchValue({ total: '' });
      this.snackBar.open('Error! Invalid quantity value', 'close', { duration: 1000 });
      return;
    }

    if (Number(value)) {
      this.proposalServiceForm.patchValue({ total: this.quantityLineItem.value * value });
    } else {
      this.proposalServiceForm.patchValue({ total: '' });
      this.snackBar.open('Error! Invalid rate value', 'close', { duration: 1000 });
    }
  }

  onTotalKeyUp(event: any): void {
    const value = event.target.value;

    if (!Number(this.quantityLineItem.value)) {
      this.proposalServiceForm.patchValue({ rate: '' });
      this.snackBar.open('Error! Invalid quantity value', 'close', { duration: 1000 });
      return;
    }

    if (Number(value)) {
      this.proposalServiceForm.patchValue({ rate: value / this.quantityLineItem.value });
    } else {
      this.proposalServiceForm.patchValue({ rate: '' });
      this.snackBar.open('Error! Invalid total value', 'close', { duration: 1000 });
    }
  }

}
