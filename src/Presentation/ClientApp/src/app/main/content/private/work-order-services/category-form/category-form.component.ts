import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { WorkOrderServiceCategoryModel } from '@app/core/models/work-order-services/work-order-service-category.model';
import { fuseAnimations } from '@fuse/animations';

@Component({
  selector: 'app-category-form',
  templateUrl: './category-form.component.html',
  styleUrls: ['./category-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class CategoryFormComponent implements OnInit {
  action: string;
  dialogTitle: string;

  category: WorkOrderServiceCategoryModel;
  categoryForm: FormGroup;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') === null ? false : true;
  }

  constructor(  @Inject(MAT_DIALOG_DATA) private data: any,
                public dialogRef: MatDialogRef<CategoryFormComponent>,
                private _formBuilder: FormBuilder) {
    this.action = data.action;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Work Order Category';
      this.category = data.category;
      this.categoryForm = this.updateCategoryForm();
    } else {
      this.dialogTitle = 'New Work Order Category';
      this.categoryForm = this.createCategoryForm();
    }
   }

  ngOnInit(): void {
  }

    // Form
    createCategoryForm(): FormGroup {
      return this._formBuilder.group({
        name: ['', [Validators.required]]
      });
    }
    updateCategoryForm(): FormGroup {
      return this._formBuilder.group({
        id: [this.category.id],
        name: [{ value: this.category.name, disabled: this.readOnly }, [Validators.required]]
      });
    }

}
