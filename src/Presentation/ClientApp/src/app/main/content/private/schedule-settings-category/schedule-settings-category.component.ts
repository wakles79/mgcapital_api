import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';
import { ScheduleSettingsCategoryFormComponent } from './schedule-settings-category-form/schedule-settings-category-form.component';

@Component({
  selector: 'app-schedule-settings-category',
  templateUrl: './schedule-settings-category.component.html',
  styleUrls: ['./schedule-settings-category.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ScheduleSettingsCategoryComponent implements OnInit {

  dialogRef: any;
  expenseTypeFormRef: any;

  get readOnly(): any {
    return localStorage.getItem('readOnly') === null ? false : true;
  }

  constructor(
    private router: Router,
    private dialog: MatDialog,
    public snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
  }

  newScheduleSettings(): void {
    this.dialogRef = this.dialog.open(ScheduleSettingsCategoryFormComponent, {
      panelClass: 'Schedule-Settings-form-dialog',
      data: {
        action: 'new'
      }
    });
    this.dialogRef.afterClosed().subscribe((response: string) => {
      if (!FormGroup) {
        return;
      }
      if (response === 'success') {
        this.snackBar.open('Schedule Category created successfully!!!', 'close', { duration: 1000 });
      } else {
        this.snackBar.open(response, 'close', { duration: 1000 });
      }
    });
  }

}
