import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DepartmentBaseModel } from '@app/core/models/department/department-base.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { CalendarEvent } from 'angular-calendar';
import { DepartmentsService } from '../departments.service';

@Component({
  selector: 'app-department-form',
  templateUrl: './department-form.component.html',
  styleUrls: ['./department-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class DepartmentFormComponent implements OnInit {

  event: CalendarEvent;
  dialogTitle: string;
  departmentForm: FormGroup;
  action: string;
  department: DepartmentBaseModel;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  constructor(
    public dialogRef: MatDialogRef<DepartmentFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private departmentService: DepartmentsService,
    public snackBar: MatSnackBar
  ) {
    this.action = data.action;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Department';
      this.department = data.department;
      this.departmentForm = this.updateDepartmentForm();
    }
    else {
      this.dialogTitle = 'New Department';
      this.departmentForm = this.createDepartmentForm();
    }
  }

  ngOnInit(): void {
  }

  createDepartmentForm(): FormGroup {
    return this.formBuilder.group({
      name: ['', [Validators.required, Validators.maxLength(80)]]
    });
  }

  updateDepartmentForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.department.id],
      name: [this.department.name, [Validators.required, Validators.maxLength(80)]]
    });
  }

}
