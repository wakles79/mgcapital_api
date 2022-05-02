import { Component, OnInit, ViewEncapsulation, OnDestroy, AfterViewInit, ViewChild, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { InspectionGridModel } from '@app/core/models/inspections/inspection-grid.model';
import { InspectionBaseModel } from '@app/core/models/inspections/inspection-base.model';
import { fuseAnimations } from '@fuse/animations';
import { merge, Observable, Subscription } from 'rxjs';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { InspectionsService } from '../inspections.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '@app/core/services/auth.service';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { InspectionsFormComponent } from '@app/core/modules/inspections-form/inspections-form/inspections-form.component';
import { DataSource } from '@angular/cdk/table';

@Component({
  selector: 'app-inspection-list',
  templateUrl: './inspection-list.component.html',
  styleUrls: ['./inspection-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class InspectionListComponent implements OnInit, OnDestroy, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @Input() roleLevelLoggedUser: number;
  @Input() loggedUserId: number;

  dialogRef: any;

  loading$ = this.inspectionService.loadingSubject.asObservable();

  searchInput: FormControl;

  get getInspectionsCount(): any { return this.inspectionService.elementsCount; }
  inspections: InspectionGridModel[] = [];
  inspection: InspectionBaseModel;
  dataSource: InspectionDataSource | null;
  columnsToDisplay = ['number', 'building', 'employee', 'beginNotes', 'closingNotes', 'createdOn', 'closedOn', 'status', 'buttons'];

  onInspectionsChangedSubscription: Subscription;
  onInspectionsDataChangedSubscription: Subscription;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  constructor(
    private inspectionService: InspectionsService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private authService: AuthService
  ) {

    this.dataSource = new InspectionDataSource(this.inspectionService);
    this.searchInput = new FormControl(this.inspectionService.searchText);

    this.onInspectionsChangedSubscription = this.inspectionService.allElementsChanged
      .subscribe(inspections => {
        this.inspections = inspections;
      });

    this.onInspectionsDataChangedSubscription = this.inspectionService.elementChanged
      .subscribe(inspection => {
        this.inspection = inspection;
      });
  }

  ngOnInit(): void {
    this.roleLevelLoggedUser = this.authService.currentUser.roleLevel;

    this.searchInput.valueChanges
    .pipe(
      debounceTime(300),
      distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.inspectionService.searchTextChanged.next(searchText);
      });

  }

  ngOnDestroy(): void {

  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.inspectionService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize
        ))
      )
      .subscribe();
  }

  editInspection(id: any): void {
    this.inspectionService.get(id)
      .subscribe((response: any) => {
        if (!response) {
          this.snackBar.open('Oops, there was an error retreiving inspection', 'close', { duration: 1000 });
          return;
        }

        const inspection = new InspectionBaseModel(response);

        this.dialogRef = this.dialog.open(InspectionsFormComponent, {
          panelClass: 'inspection-form-dialog',
          data: {
            action: 'edit',
            inspection: inspection,
            isInspector: (inspection.employeeId === this.loggedUserId),
            loggedUserLevel: this.roleLevelLoggedUser
          }
        });

        this.dialogRef.afterClosed()
          .subscribe((inspectionForm: FormGroup) => {
            if (!inspectionForm) {
              return;
            }

            const inspectionToUpdate = new InspectionBaseModel(inspectionForm.getRawValue());
            this.inspectionService.loadingSubject.next(true);
            this.inspectionService.updateElement(inspectionToUpdate)
              .then(
                () => {
                  this.inspectionService.loadingSubject.next(false);
                  this.snackBar.open('Inspection updated successfully!!!', 'close', { duration: 1000 });
                },
                () => {
                  this.inspectionService.loadingSubject.next(false);
                  this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                })
              .catch(() => {
                this.inspectionService.loadingSubject.next(false);
                this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
              });

          });

      }, (error) => { this.snackBar.open('Oops, there was an error retreiving inspection', 'close', { duration: 1000 }); });
  }



  /**
   * Delete Inspection
   */
  deleteInspection(id: any): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.inspectionService.deleteInspection(id)
          .then(
            () => {
              this.snackBar.open('Inspection deleted successfully!!!', 'close', { duration: 1000 });
            },
            (error) => {
              this.snackBar.open(error, 'close', { duration: 1000 });
            }
          ).catch(
            (error) => {
              this.snackBar.open(error, 'close', { duration: 1000 });
            }
          );
      }
      this.confirmDialogRef = null;
    });

  }

}

export class InspectionDataSource extends DataSource<any>{
  constructor(private inspectionService: InspectionsService) {
    super();
  }

  connect(): Observable<any[]> {
    return this.inspectionService.allElementsChanged;
  }

  disconnect(): void {

  }
}
