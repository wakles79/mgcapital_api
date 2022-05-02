import { DataSource } from '@angular/cdk/table';
import { AfterViewInit, Component, Input, OnDestroy, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { BuildingGridModel } from '@app/core/models/building/building-grid.model';
import { AuthService } from '@app/core/services/auth.service';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { merge, Observable, Subscription, from } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { BuildingActivityLogDialogComponent } from '../building-activity-log-dialog/building-activity-log-dialog.component';
import { BuildingsService } from '../buildings.service';
import { BuildingUpdateModel } from '@app/core/models/building/building-update.model';
import { BuildingFormComponent } from '@app/core/modules/building-form/building-form/building-form.component';

@Component({
  selector: 'app-building-list',
  templateUrl: './building-list.component.html',
  styleUrls: ['./building-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class BuildingListComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @Input() readOnly: boolean;

  get buildingsCount(): any { return this.buildingService.elementsCount; }

  // Level Role
  roleLevelLoggedUser: number;
  buildings: BuildingGridModel[];
  building: BuildingGridModel;

  dataSource: BuildingsDataSource | null;

  displayedColumns = ['code', 'name', 'fullAddress', 'customerCode', 'operationsManagerFullName', 'emergencyPhone', 'isComplete', 'isActive', 'buttons'];

  selectedBuildings: any[];
  checkboxes: {};

  onBuildingsChangedSubscription: Subscription;
  onselectedBuildingsChangedSubscription: Subscription;
  onBuildingDataChangedSubscription: Subscription;

  loading$ = this.buildingService.loadingSubject.asObservable();

  dialogRef: any;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  searchInput: FormControl;

  buildingActivityDialog: MatDialogRef<BuildingActivityLogDialogComponent>;

  constructor(
    private buildingService: BuildingsService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private authService: AuthService
  ) {
    this.searchInput = new FormControl(this.buildingService.searchText);

    this.onBuildingsChangedSubscription =
      this.buildingService.allElementsChanged.subscribe((buildings: BuildingGridModel[]) => {
        this.buildings = buildings;
      });

    this.onselectedBuildingsChangedSubscription =
      this.buildingService.selectedElementsChanged.subscribe(selectedBuildings => {
        for (const id in this.checkboxes) {
          if (!this.checkboxes.hasOwnProperty(id)) {
            continue;
          }

          this.checkboxes[id] = selectedBuildings.includes(id);
        }
        this.selectedBuildings = selectedBuildings;
      });

    this.onBuildingDataChangedSubscription =
      this.buildingService.elementChanged.subscribe(building => {
        this.building = building;
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
        this.buildingService.searchTextChanged.next(searchText);
      });
    this.dataSource = new BuildingsDataSource(this.buildingService);
  }

  ngAfterViewInit(): void {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.buildingService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize))
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.onBuildingsChangedSubscription.unsubscribe();
    this.onselectedBuildingsChangedSubscription.unsubscribe();
    this.onBuildingDataChangedSubscription.unsubscribe();
  }

  editBuilding(building): void {
    this.buildingService.loadingSubject.next(true);

    this.buildingService.get(building.id, 'update')
      .subscribe((buildingData: any) => {
        if (buildingData) {
          this.buildingService.loadingSubject.next(false);
          const buildingUpdateObj = new BuildingUpdateModel(buildingData);

          this.dialogRef = this.dialog.open(BuildingFormComponent, {
            panelClass: 'building-form-dialog',
            data: {
              action: 'edit',
              building: buildingUpdateObj
            }
          });

          this.dialogRef.afterClosed()
            .subscribe(response => {
              if (!response) {
                return;
              }
              const actionType: string = response[0];
              const formData: any = response[1];
              const updatedBuildingObj = new BuildingUpdateModel(formData);
              this.buildingService.loadingSubject.next(true);
              switch (actionType) {
                /**
                 * Save
                 */
                case 'save':
                  this.buildingService.updateElement(updatedBuildingObj)
                    .then(
                      () => {
                        this.buildingService.loadingSubject.next(false);
                        this.snackBar.open('Building updated successfully!!!', 'close', { duration: 1000 });
                      },
                      () => {
                        this.buildingService.loadingSubject.next(false);
                        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                      })
                    .catch(() => {
                      this.buildingService.loadingSubject.next(false);
                      this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                    });

                  break;
                /**
                 * Delete
                 */
                case 'delete':
                  this.buildingService.loadingSubject.next(false);
                  this.deleteBuilding(updatedBuildingObj);

                  break;
              }
            });
        } else {
          this.buildingService.loadingSubject.next(false);
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      },
        (error) => {
          this.buildingService.loadingSubject.next(false);
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });

  }

  /**
   * Delete building
   */
  deleteBuilding(building, disable = true): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    if (disable) {
      this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to disable the building?';
    } else {
      this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to enable the building?';
    }

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.buildingService.get(building.id, 'update')
          .subscribe((buildingData: any) => {
            const buildingUpdateObj = new BuildingUpdateModel(buildingData);
            if (buildingUpdateObj.isActive) {
              buildingUpdateObj.isActive = false;
            } else {
              buildingUpdateObj.isActive = true;
            }
            this.buildingService.updateElement(buildingUpdateObj)
              .then(
                () => {
                  this.buildingService.loadingSubject.next(false);
                  this.snackBar.open('Building updated successfully!!!', 'close', { duration: 1000 });
                });
          });
        // this.buildingService.deleteElement(building);
      }
      this.confirmDialogRef = null;
    });
  }

  onSelectedChange(buildingId): void {
    this.buildingService.toggleSelectedElement(buildingId);
  }

  activeStatus(value): string {
    return value ? 'Active' : 'Not Active';
  }

  completeStatus(value): string {
    return value ? 'Yes' : 'No';
  }

  availableStatus(value): string {
    return value ? 'Yes' : 'No';
  }

  displayActivityLog(id: number): void {
    this.buildingActivityDialog = this.dialog.open(BuildingActivityLogDialogComponent, {
      panelClass: 'building-activity-log-dialog',
      data: {
        buildingId: id
      }
    });
  }

}

export class BuildingsDataSource extends DataSource<any>
{
  constructor(private buildingService: BuildingsService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.buildingService.allElementsChanged;
  }

  disconnect(): void {
  }
}

