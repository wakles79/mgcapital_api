import { DataSource } from '@angular/cdk/table';
import { Component, OnInit, ViewEncapsulation, OnDestroy, AfterViewInit, ViewChild, TemplateRef } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { DepartmentBaseModel } from '@app/core/models/department/department-base.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { merge, Observable, Subscription } from 'rxjs';
import { debounceTime, tap } from 'rxjs/operators';
import { DepartmentFormComponent } from '../department-form/department-form.component';
import { DepartmentsService } from '../departments.service';

@Component({
  selector: 'app-department-list',
  templateUrl: './department-list.component.html',
  styleUrls: ['./department-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class DepartmentListComponent implements OnInit, OnDestroy, AfterViewInit {

  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  get deparmentsCount(): number { return this.departmentService.elementsCount; }

  searchInput: FormControl;
  departments: DepartmentBaseModel[];
  department: DepartmentBaseModel;
  dataSource: DepartmentDataSource | null;
  displayedColumns = ['created_date', 'name', 'buttons'];
  onDepartmentsChangedSubscription: Subscription;
  onDepartmentDataChangedSubscription: Subscription;
  loading$ = this.departmentService.loadingSubject.asObservable();
  dialogRef: any;
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  constructor(
    private departmentService: DepartmentsService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {

    this.searchInput = new FormControl(this.departmentService.searchText);

    this.onDepartmentsChangedSubscription =
      this.departmentService.allElementsChanged.subscribe(departments => {
        this.departments = departments;
      });

    this.onDepartmentDataChangedSubscription =
      this.departmentService.elementChanged.subscribe(department => {
        this.department = department;
      });
  }

  ngOnInit(): void {
    this.dataSource = new DepartmentDataSource(this.departmentService);

    try {
      this.searchInput.valueChanges
        .pipe(debounceTime(300))
        .subscribe(searchText => {
          this.paginator.pageIndex = 0;
          this.departmentService.searchTextChanged.next(searchText);
        });
    } catch (e) { console.log(e); }
  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.departmentService.getElements(
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
    this.onDepartmentsChangedSubscription.unsubscribe();
    this.onDepartmentDataChangedSubscription.unsubscribe();
  }

  // Edit Department
  editDepartment(department): void {
    this.departmentService.get(department.id, 'update')
      .subscribe((departmentData: any) => {
        if (departmentData) {
          const departmentUpdateObj = new DepartmentBaseModel(departmentData);
          this.dialogRef = this.dialog.open(DepartmentFormComponent, {
            panelClass: 'department-form-dialog',
            data: {
              department: departmentUpdateObj,
              action: 'edit'
            }
          });

          this.dialogRef.afterClosed()
            .subscribe(response => {
              if (!response) {
                return;
              }
              const actionType: string = response[0];
              const formData: FormGroup = response[1];

              const updatedDepartmentObj = new DepartmentBaseModel(formData.getRawValue());
              switch (actionType) {
                /**
                 * Save
                 */
                case 'save':

                  this.departmentService.updateElement(updatedDepartmentObj)
                    .then(
                      () => this.snackBar.open('Department updated successfully!!!', 'close', { duration: 1000 }),
                      () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
                    .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

                  break;
                /**
                 * Delete
                 */
                case 'delete':

                  this.deleteDepartment(departmentUpdateObj);

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

  // Delete Department
  deleteDepartment(department): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.departmentService.deleteDepartment(department.id);
      }
      this.confirmDialogRef = null;
    });
  }

}

export class DepartmentDataSource extends DataSource<any>
{
  constructor(private departmentService: DepartmentsService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.departmentService.allElementsChanged;
  }

  disconnect(): void {
  }
}
