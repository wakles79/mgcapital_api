import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';
import { fuseAnimations } from '@fuse/animations';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { InspectionsService } from './inspections.service';
import { AuthService } from '@app/core/services/auth.service';
import { InspectionsFormComponent } from '@app/core/modules/inspections-form/inspections-form/inspections-form.component';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-inspections',
  templateUrl: './inspections.component.html',
  styleUrls: ['./inspections.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class InspectionsComponent implements OnInit {

  roleLevelLoggedUser: number;
  loggedUserId: number;

  dialogRef: any;

  get readonly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private inspectionService: InspectionsService,
    private authService: AuthService,
    private _fuseSidebarService: FuseSidebarService
  ) { }

  ngOnInit(): void {
    this.roleLevelLoggedUser = this.authService.currentUser.roleLevel;
    this.loggedUserId = this.authService.currentUser.employeeId;
  }

  newInspection(): void {
    this.dialogRef = this.dialog.open(InspectionsFormComponent, {
      panelClass: 'inspection-form-dialog',
      data: {
        action: 'new',
        loggedUserLevel: this.roleLevelLoggedUser
      }
    });

    this.dialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        this.inspectionService.createElement(response.getRawValue())
          .then(
            (addedInspection) => {
              this.snackBar.open('Inspection created successfully!!!', 'close', { duration: 1000 });
            },
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });
  }

    /**
     * Toggle sidebar
     *
     * @param name
     */
    toggleSidebar(name): void
    {
        this._fuseSidebarService.getSidebar(name).toggleOpen();
    }
}
