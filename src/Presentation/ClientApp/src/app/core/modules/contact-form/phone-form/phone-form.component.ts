import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { fuseAnimations } from '@fuse/animations';
import { PhoneModel } from '@app/core/models/common/phone.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-phone-form',
  templateUrl: './phone-form.component.html',
  styleUrls: ['./phone-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class PhoneFormComponent implements OnInit {

  phone: PhoneModel;
  phoneForm: FormGroup;
  dialogTitle: string;
  action: string;
  entityId: number;

  constructor(
    public phoneDialogRef: MatDialogRef<PhoneFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder
  ) {
    this.action = data.action;
    this.entityId = data.entityId;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Phone';
      this.phone = data.phone;
      this.phoneForm = this.createPhoneUpdateForm();
    }
    else {
      this.dialogTitle = 'New Phone';
      this.phoneForm = this.createPhoneCreateForm();
    }
  }

  ngOnInit(): void {
  }

  createPhoneCreateForm(): FormGroup {
    return this.formBuilder.group({
      entityId: [this.entityId],
      type: ['', [Validators.required, Validators.maxLength(13)]],
      default: [false],
      phone: ['', [Validators.required, Validators.maxLength(13)]],
      ext: ['']
    });
  }

  createPhoneUpdateForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.phone.id],
      entityId: [this.phone.entityId],
      type: [this.phone.type, [Validators.required]],
      default: [this.phone.default],
      phone: [this.phone.phone, [Validators.required]],
      ext: [this.phone.ext]
    });

  }

}
