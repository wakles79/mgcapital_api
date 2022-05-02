import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ScheduleSubcategoryBaseModel } from '@app/core/models/schedule-subcategory/schedule-subcategory-base.model';
import { fuseAnimations } from '@fuse/animations';

@Component({
  selector: 'app-schedule-settings-subcategory-form',
  templateUrl: './schedule-settings-subcategory-form.component.html',
  styleUrls: ['./schedule-settings-subcategory-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ScheduleSettingsSubcategoryFormComponent implements OnInit {

  subCategoryForm: FormGroup;
  action: string;
  dialogTitle: string;
  subcategories: ScheduleSubcategoryBaseModel;
  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    public dialogRef: MatDialogRef<ScheduleSettingsSubcategoryFormComponent>,
    public snackBar: MatSnackBar,
    private formBuilder: FormBuilder
  ) {
    this.action = data.action;

    if (this.action === 'new') {
      this.dialogTitle = 'New SubCategory';
      this.action = 'new';
      this.subCategoryForm = this.createSubcategoryForm();
    } else if (this.action === 'edit') {
      this.dialogTitle = 'Update SubCategory';
      this.action = 'edit';
      this.subcategories = data.subcategories;
      this.subCategoryForm = this.updateSubcategoryForm();
    }

  }

  ngOnInit(): void {
  }

  createSubcategoryForm(): any {
    return this.formBuilder.group({
      name: ['', Validators.required],
    });
  }

  updateSubcategoryForm(): any {
    return this.formBuilder.group({
      id: [this.subcategories.id, Validators.required],
      name: [this.subcategories.name, Validators.required],
    });
  }
}
