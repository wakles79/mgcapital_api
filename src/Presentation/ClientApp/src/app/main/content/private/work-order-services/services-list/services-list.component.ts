import { DataSource } from '@angular/cdk/table';
import { AfterViewInit, Component, Input, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { fuseAnimations } from '@fuse/animations';
import { merge, Observable, Subject } from 'rxjs';
import { ServiceFormComponent } from '../service-form/service-form.component';
import { WorkOrderServicesService } from '../work-order-services.service';
import { WorkOrderServiceGridModel } from '@app/core/models/work-order-services/work-order-services-grid.model';
import { WorkOrderServiceBaseModel } from '@app/core/models/work-order-services/work-order-services-base.model';

import { takeUntil, tap } from 'rxjs/operators';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-services-list',
  templateUrl: './services-list.component.html',
  styleUrls: ['./services-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ServicesListComponent implements OnInit, OnDestroy, AfterViewInit {
  @Input() readOnly: boolean;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  loading$ = this._workOrderServicesService.loadingSubject.asObservable();

  get servicesCount(): number { return this._workOrderServicesService.elementsCount; }

  services: WorkOrderServiceGridModel[] = [];
  dataSource: WorkOrderServicesDataSource | null;
  displayedColumns = ['name', 'category', 'frequency', 'unitFactor', 'rate', 'rules', 'options'];

  serviceDialog: MatDialogRef<ServiceFormComponent>;

  private _unsubscribeAll: Subject<any>;

  constructor(
    private _workOrderServicesService: WorkOrderServicesService,
    private _snackBar: MatSnackBar,
    private _dialog: MatDialog
  ) {
    this._unsubscribeAll = new Subject();
    this.dataSource = new WorkOrderServicesDataSource(this._workOrderServicesService);

    this._workOrderServicesService.selectedCategoryChanged
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe(id => {
        if (id > 0) {
          this._workOrderServicesService.onFilterChanged.next({ 'categoryId': id });
        } else {
          this._workOrderServicesService.onFilterChanged.next({ 'categoryId': null });
        }
      });
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  ngAfterViewInit(): void {
    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this._workOrderServicesService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize))
      )
      .subscribe();
  }

  // Buttons
  editService(id: number): void {
    this._workOrderServicesService.get(id)
      .subscribe((response) => {
        if (!response) {
          this._snackBar.open('Oops! there was an error', 'close', { duration: 3000 });
          return;
        }

        const service = new WorkOrderServiceBaseModel(response);
        this.serviceDialog = this._dialog.open(ServiceFormComponent, {
          panelClass: 'service-form-dialog',
          data: {
            action: 'edit',
            service: service
          }
        });

        this.serviceDialog.afterClosed()
          .subscribe((dialogResult: FormGroup) => {
            if (!dialogResult) {
              return;
            }

            this._workOrderServicesService.loadingSubject.next(true);
            this._workOrderServicesService.updateElement(dialogResult.getRawValue())
              .then(() => {
                this._workOrderServicesService.loadingSubject.next(false);
                this._snackBar.open('Service updated successfully.', 'close', { duration: 3000 });
              }, () => {
                this._workOrderServicesService.loadingSubject.next(false);
                this._snackBar.open('Oops! there was an error', 'close', { duration: 3000 });
              })
              .catch(() => {
                this._workOrderServicesService.loadingSubject.next(false);
                this._snackBar.open('Oops! there was an error', 'close', { duration: 3000 });
              });
          });
      }, (error) => {
        this._snackBar.open('Oops! there was an error', 'close', { duration: 3000 });
      });
  }

}

export class WorkOrderServicesDataSource extends DataSource<any>{
  constructor(private _servicesService: WorkOrderServicesService) {
    super();
  }

  connect(): Observable<any[]> {
    return this._servicesService.allElementsChanged;
  }

  disconnect(): void {

  }
}
