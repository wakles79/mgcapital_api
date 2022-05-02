import { Component, OnInit, ViewEncapsulation, OnDestroy, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ServiceUpdateModel } from '@app/core/models/service/service-update.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { Subject } from 'rxjs';
import { ServicesService } from '../services.service';

@Component({
  selector: 'app-service-form',
  templateUrl: './service-form.component.html',
  styleUrls: ['./service-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ServiceFormComponent implements OnInit, OnDestroy {

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  dialogTitle: string;
  serviceForm: FormGroup;
  action: string;

  service: ServiceUpdateModel;

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  dialogCustomerRef: any;

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<ServiceFormComponent>,
    public snackBar: MatSnackBar,
    private serviceService: ServicesService
  ) {
    this.action = data.action;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Service';
      this.service = data.service;
      this.serviceForm = this.createWOUpdateForm();
    }
    else {
      this.dialogTitle = 'New Service';
      this.serviceForm = this.createWOCreateForm();
    }
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  createWOCreateForm(): FormGroup {
    return this.formBuilder.group({
      name: ['', Validators.required],
      unitFactor: ['', Validators.required],
      unitPrice: ['', Validators.required],
      minPrice: ['', Validators.required]
    });
  }

  createWOUpdateForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.service.id],
      name: [{ value: this.service.name, disabled: this.readOnly }, Validators.required],
      unitFactor: [{ value: this.service.unitFactor, disabled: this.readOnly }, Validators.required],
      unitPrice: [{ value: this.service.unitPrice, disabled: this.readOnly }, Validators.required],
      minPrice: [{ value: this.service.minPrice, disabled: this.readOnly }, Validators.required]
    });
  }

}
