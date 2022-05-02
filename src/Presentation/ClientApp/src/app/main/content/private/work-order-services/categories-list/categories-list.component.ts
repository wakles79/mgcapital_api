import { Component, Input, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fuseAnimations } from '@fuse/animations';
import { CategoryFormComponent } from '../category-form/category-form.component';
import { WorkOrderServicesService } from '../work-order-services.service';
import { FormGroup } from '@angular/forms';
import { WorkOrderServiceCategoryModel } from '@app/core/models/work-order-services/work-order-service-category.model';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-categories-list',
  templateUrl: './categories-list.component.html',
  styleUrls: ['./categories-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class CategoriesListComponent implements OnInit, OnDestroy {
  @Input() readOnly: boolean;

  loading = false;

  selectedCategoryId: number = 0;
  categories: any[] = [];

  categoryDialog: MatDialogRef<CategoryFormComponent>;

  private _unsubscribeAll: Subject<any>;

  constructor(
    private _workOrderServicesService: WorkOrderServicesService,
    private _snackBar: MatSnackBar,
    private _dialog: MatDialog
  ) {
    this._unsubscribeAll = new Subject();

    this._workOrderServicesService.updateCategoryList
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe(updated => {
        if (updated) {
          this.getCategories();
        }
      });
  }

  ngOnInit(): void {
    this.getCategories();
  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  // Categories
  getCategories(): void {
    this.categories = [];
    this.loading = true;
    this._workOrderServicesService.readAllCategories('ReadAllCategories', { 'SortField': 'Name', 'SortOrder': 'ASC' })
      .subscribe((result: { count: number, payload: any[] }) => {
        this.loading = false;
        this.categories = result.payload;
      }, (error) => {
        this.loading = false;
      });
  }

  // Buttons
  selectCategory(id: number): void {
    this.selectedCategoryId = id;

    this._workOrderServicesService.selectedCategoryChanged.next(id);
  }

  editCategory(id: number): void {
    this._workOrderServicesService.get(id, 'GetCategory')
      .subscribe((response) => {
        if (!response) {
          this._snackBar.open('Oops! there was an error', 'close', { duration: 3000 });
          return;
        }

        const category = new WorkOrderServiceCategoryModel(response);
        this.categoryDialog = this._dialog.open(CategoryFormComponent, {
          panelClass: 'service-category-form-dialog',
          data: {
            action: 'edit',
            category: category
          }
        });

        this.categoryDialog.afterClosed()
          .subscribe((dialogResult: FormGroup) => {
            if (!dialogResult) {
              return;
            }

            this._workOrderServicesService.updateCategory(dialogResult.getRawValue())
              .subscribe(() => {
                this._snackBar.open('Category updated successfully.', 'close', { duration: 3000 });
                this.getCategories();
              }, (error) => {
                this._snackBar.open('Oops! there was an error', 'close', { duration: 3000 });

              });
          });
      }, () => {
        this._snackBar.open('Oops! there was an error', 'close', { duration: 3000 });
      });
  }

}
