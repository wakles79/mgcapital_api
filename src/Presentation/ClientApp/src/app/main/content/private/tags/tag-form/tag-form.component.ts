import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TagBaseModel } from '@app/core/models/tag/tag-base.model';
import { TAG_COLORS } from '@app/core/models/tag/tag-color.model';
import { fuseAnimations } from '@fuse/animations';

@Component({
  selector: 'app-tag-form',
  templateUrl: './tag-form.component.html',
  styleUrls: ['./tag-form.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: fuseAnimations
})
export class TagFormComponent implements OnInit {

  action: string;
  dialogTitle: string;

  tagForm: FormGroup;

  tag: TagBaseModel;

  colors = TAG_COLORS;

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    public dialogRef: MatDialogRef<TagFormComponent>,
    private _formBuilder: FormBuilder
  ) {
    this.action = data.action;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Tag';
      this.tag = data.tag;
      this.tagForm = this.updateTagForm();
    } else {
      this.dialogTitle = 'New Tag';
      this.tagForm = this.createTagForm();
    }
  }

  ngOnInit(): void {
  }

  // Form
  createTagForm(): FormGroup {
    return this._formBuilder.group({
      description: ['', [Validators.required]],
      hexColor: ['', [Validators.required]]
    });
  }
  updateTagForm(): FormGroup {
    return this._formBuilder.group({
      id: this.tag.id,
      description: this.tag.description,
      type: this.tag.type,
      hexColor: [this.tag.hexColor, [Validators.required]]
    });
  }
}
