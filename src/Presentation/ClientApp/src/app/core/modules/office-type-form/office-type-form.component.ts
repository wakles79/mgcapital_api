import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OfficeTypeBaseModel } from '@app/core/models/office-type/office-type-base.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-office-type-form',
  templateUrl: './office-type-form.component.html',
  styleUrls: ['./office-type-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class OfficeTypeFormComponent implements OnInit {

  officeType: OfficeTypeBaseModel;
  officeTypeForm: FormGroup;
  dialogTitle: string;
  action: string;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  periodicities: string[] = ['Daily', 'Monthly', 'Bi-Monthly', 'Quarterly', 'Bi-Annually', 'Yearly'];
  serviceTypes: any[] = [
    { id: 0, name: 'Hour' },
    { id: 1, name: 'Unit' },
    { id: 2, name: 'Room' },
    { id: 3, name: 'Square Feet' }];

  get rateType(): any { return this.officeTypeForm.get('rateType').value; }

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    public dialogRef: MatDialogRef<OfficeTypeFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    public snacBar: MatSnackBar
  ) {
    this.action = data.action;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Office Service Type';
      this.officeType = data.officeType;
      this.officeTypeForm = this.updateOfficeTypeform();
    }
    else {
      this.dialogTitle = 'New Office Service Type';
      this.officeTypeForm = this.createOfficeTypeForm();
    }
  }

  ngOnInit(): void {
    this.rateTypeSelectChanged(this.officeType.rateType);
  }

  createOfficeTypeForm(): FormGroup {
    return this.formBuilder.group({
      name: [{ value: '', disabled: this.readOnly }, [Validators.required, Validators.maxLength(64)]],
      rate: [{ value: '', disabled: this.readOnly }, [Validators.required]],
      rateType: [{ value: 0, disabled: this.readOnly }],
      periodicity: [{ value: 'Daily', disabled: this.readOnly }],
      status: [{ value: 1, disabled: this.readOnly }],
      supplyFactor: [{ value: 0, disabled: this.readOnly }]
    });
  }

  updateOfficeTypeform(): FormGroup {
    return this.formBuilder.group({
      id: this.officeType.id,
      name: [{ value: this.officeType.name, disabled: this.readOnly }, [Validators.required, Validators.maxLength(64)]],
      rate: [{ value: this.officeType.rate, disabled: this.readOnly }, [Validators.required]],
      rateType: [{ value: this.officeType.rateType, disabled: this.readOnly }],
      periodicity: [{ value: this.officeType.periodicity, disabled: this.readOnly }],
      status: [{ value: this.officeType.status, disabled: this.readOnly }],
      supplyFactor: [{ value: this.officeType.supplyFactor, disabled: this.readOnly }]
    });
  }

  rateTypeSelectChanged(selectedId: number): void {
    if (selectedId !== null) {
      if (this.officeTypeForm) {
        this.officeTypeForm.patchValue({ supplyfactor: '' });
      }
    }
  }

}
