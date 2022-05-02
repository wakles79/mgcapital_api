import { DataSource } from '@angular/cdk/table';
import { AfterViewInit, Component, OnInit, OnDestroy, ViewChild, TemplateRef, Input, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { ServiceGridModel } from '@app/core/models/service/service-grid.model';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { merge, Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { ServicesService } from '../services.service';
import { ServiceUpdateModel } from '@app/core/models/service/service-update.model';
import { ServiceFormComponent } from '../service-form/service-form.component';
import { fuseAnimations } from '@fuse/animations';

@Component({
  selector: 'app-service-list',
  templateUrl: './service-list.component.html',
  styleUrls: ['./service-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ServiceListComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  get servicesCount(): any { return this.serviceService.elementsCount; }

  @Input() readOnly: boolean;

  services: ServiceGridModel[];
  service: ServiceGridModel;

  dataSource: ServicesDataSource | null;

  displayedColumns = ['name', 'unitFactor', 'unitPrice', 'minPrice', 'buttons'];

  onServicesChangedSubscription: Subscription;
  onServiceDataChangedSubscription: Subscription;

  loading$ = this.serviceService.loadingSubject.asObservable();

  dialogRef: any;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  searchInput: FormControl;

  constructor(
    private serviceService: ServicesService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {
    this.searchInput = new FormControl(this.serviceService.searchText);

    this.onServicesChangedSubscription =
      this.serviceService.allElementsChanged.subscribe(services => {
        this.services = services;
      });

    this.onServiceDataChangedSubscription =
      this.serviceService.elementChanged.subscribe(service => {
        this.service = service;
      });
  }

  ngOnInit(): void {

    this.searchInput.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.serviceService.searchTextChanged.next(searchText);
      });
    this.dataSource = new ServicesDataSource(this.serviceService);

  }

  ngAfterViewInit(): void {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.serviceService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize))
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.onServicesChangedSubscription.unsubscribe();
    this.onServiceDataChangedSubscription.unsubscribe();
  }

  editService(service): void {
    this.serviceService.get(service.id, 'update')
      .subscribe((serviceData: any) => {
        if (serviceData) {

          const serviceUpdateObj = new ServiceUpdateModel(serviceData);

          this.dialogRef = this.dialog.open(ServiceFormComponent, {
            panelClass: 'service-form-dialog',
            data: {
              action: 'edit',
              service: serviceUpdateObj
            }
          });

          this.dialogRef.afterClosed()
            .subscribe(response => {
              if (!response) {
                return;
              }
              const actionType: string = response[0];
              const formData: FormGroup = response[1];
              const updatedServiceObj = new ServiceUpdateModel(formData.getRawValue());
              switch (actionType) {
                /**
                 * Save
                 */
                case 'save':
                  this.serviceService.updateElement(updatedServiceObj)
                    .then(
                      () => this.snackBar.open('Service updated successfully!!!', 'close', { duration: 1000 }),
                      () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
                    .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

                  break;
                /**
                 * Delete
                 */
                case 'delete':

                  this.deleteService(updatedServiceObj);

                  break;
              }
            });
        } else {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      },
        (error) => {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });

  }

  deleteService(service): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to disable the service?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.serviceService.delete(service).then(
          () => this.snackBar.open('Service delete successfully!!!', 'close', { duration: 3000 }),
          () => this.snackBar.open('Oops,  Cannot be deleted', 'close', { duration: 3000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 3000 }));
      }
      this.confirmDialogRef = null;
    });
  }

}

export class ServicesDataSource extends DataSource<any>
{
  constructor(private serviceService: ServicesService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.serviceService.allElementsChanged;
  }

  disconnect(): void {
  }
}

