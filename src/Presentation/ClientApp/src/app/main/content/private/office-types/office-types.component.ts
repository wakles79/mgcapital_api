import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fuseAnimations } from '@fuse/animations';
import { OfficeTypesService } from './office-types.service';
import { OfficeTypeFormComponent } from '@app/core/modules/office-type-form/office-type-form.component';
import { FormGroup } from '@angular/forms';
import { OfficeTypeBaseModel } from '@app/core/models/office-type/office-type-base.model';

@Component({
  selector: 'app-office-types',
  templateUrl: './office-types.component.html',
  styleUrls: ['./office-types.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class OfficeTypesComponent implements OnInit, OnDestroy {

  hasSelectedOfficeType: boolean;
  dialogRef: any;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    private officeTypeService: OfficeTypesService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {

  }

  ngOnDestroy(): void {

  }

  newOfficeType(): void {
    this.dialogRef = this.dialog.open(OfficeTypeFormComponent, {
      panelClass: 'office-type-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.dialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        const newOfficeTypeObj = new OfficeTypeBaseModel(response.getRawValue());
        this.officeTypeService.createElement(newOfficeTypeObj)
          .then(
            () => this.snackBar.open('Office type created successfully!!!', 'close', { duration: 1000 }),
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });
  }

}
