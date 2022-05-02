import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fuseAnimations } from '@fuse/animations';
import { ServiceFormComponent } from './service-form/service-form.component';
import { ServicesService } from './services.service';

@Component({
  selector: 'app-services',
  templateUrl: './services.component.html',
  styleUrls: ['./services.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ServicesComponent implements OnInit {

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  dialogRef: any;

  constructor(
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private serviceService: ServicesService
  ) { }

  ngOnInit(): void {
  }

  newService(): void {
    this.dialogRef = this.dialog.open(ServiceFormComponent, {
      panelClass: 'service-form-dialog',
      data: {
        action: 'new'
      }
    });
    this.dialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        this.serviceService.createElement(response.getRawValue())
          .then(
            () => this.snackBar.open('Service created successfully!!!', 'close', { duration: 1000 }),
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });
  }

}
