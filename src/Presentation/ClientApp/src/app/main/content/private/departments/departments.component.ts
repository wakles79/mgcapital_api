import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fuseAnimations } from '@fuse/animations';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';
import { DepartmentFormComponent } from './department-form/department-form.component';
import { DepartmentsService } from './departments.service';

@Component({
  selector: 'app-departments',
  templateUrl: './departments.component.html',
  styleUrls: ['./departments.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class DepartmentsComponent implements OnInit, OnDestroy {

  hasSelectedDepartments: boolean;
  dialogRef: any;

  constructor(
    private departmentsService: DepartmentsService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private _fuseSidebarService: FuseSidebarService
  ) {
  }

  ngOnInit(): void {
  }

  newDepartment(): void {
    this.dialogRef = this.dialog.open(DepartmentFormComponent, {
      panelClass: 'department-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.dialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        this.departmentsService.createElement(response.getRawValue())
          .then(
            () => this.snackBar.open('Department created successfully!!!', 'close', { duration: 1000 }),
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });
  }

  ngOnDestroy(): void {
  }

  /**
   * Toggle sidebar
   *
   * @param name
   */
  toggleSidebar(name): void {
    this._fuseSidebarService.getSidebar(name).toggleOpen();
  }

}
