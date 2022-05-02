import { DataSource } from '@angular/cdk/table';
import { Component, OnInit, ViewEncapsulation, OnDestroy, AfterViewInit, ViewChild, TemplateRef, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { OfficeTypeBaseModel } from '@app/core/models/office-type/office-type-base.model';
import { OfficeTypeGridModel } from '@app/core/models/office-type/office-type-grid.model';
import { OfficeTypeFormComponent } from '@app/core/modules/office-type-form/office-type-form.component';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { merge, Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { OfficeTypesService } from '../office-types.service';

@Component({
  selector: 'app-office-type-list',
  templateUrl: './office-type-list.component.html',
  styleUrls: ['./office-type-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class OfficeTypeListComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @Input() readOnly: boolean;

  officeTypes: OfficeTypeGridModel[];
  get officeTypesCount(): any { return this.officeTypeService.elementsCount; }
  officeType: OfficeTypeBaseModel;
  dataSource: OfficeTypeDataSource | null;
  displayedColumns = ['name', 'rate', 'type', 'periodicity', 'status', 'buttons'];

  onOfficeTypesChangedSubscription: Subscription;
  onOfficeTypeDataChangedSubscription: Subscription;

  loading$ = this.officeTypeService.loadingSubject.asObservable();

  dialogRef: any;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  searchInput: FormControl;

  constructor(
    private officeTypeService: OfficeTypesService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {
    this.searchInput = new FormControl(this.officeTypeService.searchText);

    this.onOfficeTypesChangedSubscription =
      this.officeTypeService.allElementsChanged.subscribe(officetypes => {
        this.officeTypes = officetypes;
      });

    this.onOfficeTypeDataChangedSubscription =
      this.officeTypeService.elementChanged.subscribe(officetype => {
        this.officeType = officetype;
      });
  }

  ngOnInit(): void {

    this.dataSource = new OfficeTypeDataSource(this.officeTypeService);

    this.searchInput.valueChanges
    .pipe(
      debounceTime(300),
      distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.officeTypeService.searchTextChanged.next(searchText);
      });
  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.officeTypeService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize
        ))
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.onOfficeTypeDataChangedSubscription.unsubscribe();
    this.onOfficeTypesChangedSubscription.unsubscribe();
  }

  activeStatus(status: boolean): any {
    return status ? 'Active' : 'Inactive';
  }

  activeOptions(status: boolean): any {
    return status ? 'Disable' : 'Enable';
  }

  /**
   * Edit office type
   * @param officeType
   */
  editOfficeType(officeType): void {
    this.officeTypeService.get(officeType.id, 'update')
      .subscribe(
        (officeTypeData: any) => {
          if (officeTypeData) {
            const officeTypeUpdateData = new OfficeTypeBaseModel(officeTypeData);
            this.dialogRef = this.dialog.open(OfficeTypeFormComponent,
              {
                panelClass: 'office-type-form-dialog',
                data: {
                  officeType: officeTypeUpdateData,
                  action: 'edit'
                }
              });

            this.dialogRef.afterClosed()
              .subscribe(
                response => {
                  if (!response) {
                    return;
                  }
                  const actionType: string = response[0];
                  const formData: FormGroup = response[1];

                  const updatedOfficeTypeObj = new OfficeTypeBaseModel(formData.getRawValue());
                  if (updatedOfficeTypeObj.rateType !== 3) {
                    updatedOfficeTypeObj.supplyFactor = 0;
                  }
                  switch (actionType) {
                    /* Update item*/
                    case 'save':
                      this.officeTypeService.updateElement(updatedOfficeTypeObj)
                        .then(
                          () => this.snackBar.open('Office type updated successfully!!!', 'close', { duration: 1000 }),
                          () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 })
                        )
                        .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
                      break;

                    /* Delete item */
                    case 'delete':
                      this.deleteOfficeType(updatedOfficeTypeObj);
                      break;
                  }
                }
              );
          } else {
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          }
        },
        (error) => {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      );
  }

  /**
   * Delete office type
   * @param officeType
   */
  deleteOfficeType(officeType): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete the office type?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.officeTypeService.delete(officeType);
      }
      this.confirmDialogRef = null;
    });
  }

  updateStatusOfficeType(id: number): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to disable the office type?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.officeTypeService.loadingSubject.next(true);
        this.officeTypeService.get(id, 'update').subscribe((officeServiceType: OfficeTypeBaseModel) => {
          officeServiceType.status = !officeServiceType.status;

          this.officeTypeService.updateElement(officeServiceType)
            .then(() => {
              this.officeTypeService.loadingSubject.next(false);
              this.snackBar.open('Office type updated successfully!!!', 'close', { duration: 1000 });
            })
            .catch(() => {
              this.officeTypeService.loadingSubject.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            });
        }, (error) => {
          this.officeTypeService.loadingSubject.next(false);
        });
      }
      this.confirmDialogRef = null;
    });
  }

}

export class OfficeTypeDataSource extends DataSource<any>
{
  constructor(private officeTypeService: OfficeTypesService) {
    super();
  }

  connect(): Observable<any[]> {
    return this.officeTypeService.allElementsChanged;
  }

  disconnect(): void {
  }
}
