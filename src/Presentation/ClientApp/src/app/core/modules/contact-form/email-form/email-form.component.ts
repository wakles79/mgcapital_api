import { Component, OnInit, ViewEncapsulation, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { EmailModel } from '@core/models/common/email.model';
import { fuseAnimations } from '@fuse/animations';

@Component({
  selector: 'fuse-email-form',
  templateUrl: './email-form.component.html',
  styleUrls: ['./email-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class EmailFormComponent implements OnInit {
  email: EmailModel;
  emailForm: FormGroup;
  dialogTitle: string;
  action: string;
  entityId: number;


  constructor(
    public emailDialogRef: MatDialogRef<EmailFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder
  ) {
    this.action = data.action;
    this.entityId = data.entityId;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Email';
      this.email = data.email;
      this.emailForm = this.createEmailUpdateForm();
    }
    else {
      this.dialogTitle = 'New Email';
      this.emailForm = this.createEmailCreateForm();
    }
  }

  ngOnInit(): void {
  }

  createEmailCreateForm(): FormGroup {
    return this.formBuilder.group({
      entityId: [this.entityId],
      type: ['', [Validators.required]],
      default: [false],
      email: ['', [Validators.required, Validators.email]]
    });
  }

  createEmailUpdateForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.email.id],
      entityId: [this.email.entityId],
      type: [this.email.type, [Validators.required]],
      default: [this.email.default],
      email: [this.email.email, [Validators.required, Validators.email]]
    });

  }
}
