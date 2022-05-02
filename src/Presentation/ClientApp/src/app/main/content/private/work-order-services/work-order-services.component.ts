import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fuseAnimations } from '@fuse/animations';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CategoryFormComponent } from './category-form/category-form.component';
import { ServiceFormComponent } from './service-form/service-form.component';
import { WorkOrderServicesService } from './work-order-services.service';

@Component({
  selector: 'app-work-order-services',
  templateUrl: './work-order-services.component.html',
  styleUrls: ['./work-order-services.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class WorkOrderServicesComponent implements OnInit, OnDestroy {

  // Dialog
  categoryDialog: MatDialogRef<CategoryFormComponent>;
  serviceDialog: MatDialogRef<ServiceFormComponent>;

  selectedCategory = 0;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') === null ? false : true;
  }

  private _unsubscribeAll: Subject<any>;

  constructor(
    private _workOrderServicesService: WorkOrderServicesService,
    private _dialog: MatDialog,
    private _snackBar: MatSnackBar
  ) {
    this._unsubscribeAll = new Subject();

    this._workOrderServicesService.selectedCategoryChanged
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe(id => {
        this.selectedCategory = id;
      });
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  newCategory(): void {
    this.categoryDialog = this._dialog.open(CategoryFormComponent, {
      panelClass: 'service-category-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.categoryDialog.afterClosed()
      .subscribe((dialogResult: FormGroup) => {
        if (!dialogResult) {
          return;
        }

        this._workOrderServicesService.createCategory(dialogResult.getRawValue())
          .subscribe(() => {
            this._workOrderServicesService.updateCategoryList.next(true);
            this._snackBar.open('Category added successfully.', 'close', { duration: 3000 });
          }, () => {
            this._snackBar.open('Oops! there was an error.', 'close', { duration: 3000 });
          });
      }, (error) => {

      });
  }

  newService(): void {
    this.serviceDialog = this._dialog.open(ServiceFormComponent, {
      panelClass: 'service-form-dialog',
      data: {
        action: 'new',
        categoryId: this.selectedCategory
      }
    });

    this.serviceDialog.afterClosed()
      .subscribe((dialogResult: FormGroup) => {
        if (!dialogResult) {
          return;
        }

        this._workOrderServicesService.createElement(dialogResult.getRawValue())
          .then(() => {
            this._snackBar.open('Service added successfully.', 'close', { duration: 3000 });
          }, () => {
            this._snackBar.open('Oops! there was an error.', 'close', { duration: 3000 });
          })
          .catch();
      }, (error) => {
        this._snackBar.open('Oops! there was an error.', 'close', { duration: 3000 });
      });
  }

}
