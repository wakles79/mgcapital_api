import { Component, Inject, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, AbstractControl } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { ServiceGridModel } from '../../../models/service/service-grid.model';
import { ServicesService } from '../../../../main/content/private/services/services.service';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';

@Component({
  selector: 'app-wo-task-billing-form',
  templateUrl: './wo-task-billing-form.component.html',
  styleUrls: ['./wo-task-billing-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class WoTaskBillingFormComponent implements OnInit, OnDestroy {

  dialogTitle: string;
  woTaskBillingForm: FormGroup;
  action: string;
  woTask: any;

  unitPrice = 0;
  unitFactor: string;
  quantity = 1;
  discountPercentage = 0;
  minPrice = 0;
  serviceName: string;

  get total(): number {
    return this.unitPrice * this.quantity * (1 - this.discountPercentage / 100);
  }

  services: ServiceGridModel[] = [];
  selectedService: ServiceGridModel;

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    public dialogRef: MatDialogRef<WoTaskBillingFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private service: ServicesService,
    private epochPipe: FromEpochPipe,
  ) {
    this.action = data.action;
    this.woTask = data.task;
    this.quantity = this.woTask.quantity;
    this.unitPrice = this.woTask.unitPrice;
    this.unitFactor = this.woTask.unitFactor;
    this.dialogTitle = 'Task Billing';
    this.woTaskBillingForm = this.createWOSCreateForm();
  }

  ngOnInit(): void {
    this.getServices();

    if (this.action === 'edit') {
      this.initAdditionalEditionData();
    }

    this.woTaskBillingForm.get('serviceId').valueChanges.
      subscribe(selectedServiceId => {
        if (selectedServiceId != null) {
          this.selectedService = this.services.find(service => service.id === selectedServiceId);
          this.unitPrice = this.selectedService.unitPrice;
          this.unitFactor = this.selectedService.unitFactor;
          this.minPrice = this.selectedService.minPrice;

          /* this.overrideMinPrice.setValue(false); */
        }
      });
  }

  initAdditionalEditionData(): void {
    this.woTaskBillingForm.addControl('createdDate',
      new FormControl({ value: this.getValidDate(this.woTask.createdDate, this.woTask.epochCreatedDate), disabled: this.readOnly }, null));

    this.woTaskBillingForm.addControl('lastCheckedDate',
      new FormControl({ value: this.getValidDate(this.woTask.lastCheckedDate, this.woTask.echoLastCheckedDate), disabled: this.readOnly }, null));
  }

  /*   get overrideMinPrice(){
      return this.woTaskBillingForm.get('overrideMinPrice');
    } */

  submit(): void {
    if (!this.selectedService) {
      this.selectedService = this.services.find(service => service.id === this.woTask.serviceId);
    }
    this.minPrice = this.selectedService.minPrice;
    this.unitFactor = this.selectedService.unitFactor;
    this.serviceName = this.selectedService.name;

    /*if (this.total < this.minPrice && !this.overrideMinPrice.value) {
      this.unitPrice = this.minPrice;
      this.quantity = 1;
      this.discountPercentage = 0;
    } */
    this.dialogRef.close(this.woTaskBillingForm);
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  getServices(): void {
    this.service.getAll()
      .subscribe((response: { count: number, payload: ServiceGridModel[] }) => {
        this.services = response.payload;
        if (this.woTask && this.woTask.serviceId) {
          this.minPrice = this.services.find(s => s.id === this.woTask.serviceId).minPrice;
          this.unitPrice = this.services.find(s => s.id === this.woTask.serviceId).unitPrice;
          /* if (this.minPrice > this.total) {
            this.overrideMinPrice.setValue(true);
          } */
        }
      },
        (error) => this.snackBar.open('Oops, there was an error fetching services', 'close', { duration: 1000 })
      );
  }

  createWOSCreateForm(): FormGroup {
    return this.formBuilder.group({
      description: [{ value: this.woTask.description, disabled: this.readOnly}],
      isComplete: [{ value: this.woTask.isComplete, disabled: this.readOnly}],
      serviceId: [{ value: this.woTask.serviceId, disabled: this.readOnly}],
      unitPrice: [{ value: this.woTask.unitPrice, disabled: this.readOnly}],
      quantity: [{ value: this.woTask.quantity, disabled: this.readOnly}],
      discountPercentage: [{ value: this.woTask.discountPercentage, disabled: this.readOnly}],
      total: [{ value: '', disabled: this.readOnly}],
      unitFactor: [{ value: this.unitFactor, disabled: this.readOnly}],
      serviceName: [{ value: this.serviceName, disabled: this.readOnly}],
      overrideMinPrice: [{ value: false, disabled: this.readOnly}],
      note: [{ value: this.woTask.note, disabled: this.readOnly}]
    });
  }

  get createdDate(): AbstractControl {
    return this.woTaskBillingForm.get('createdDate');
  }

  // Return a valid date, a valid date is a date different of null or default value
  getValidDate(dateToValidate: any, epochDate: number): any {

    const possibleDate: any = new Date(dateToValidate);
    const dateToCompare = new Date('2000-01-01');

    if (possibleDate < dateToCompare) {
      return null;
    }
    else {
      return new Date(this.epochPipe.transform(epochDate));
    }
  }

}
