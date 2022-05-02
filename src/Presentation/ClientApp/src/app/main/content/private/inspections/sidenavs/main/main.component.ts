import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BuildingsService } from '../../../buildings/buildings.service';
import { UsersBaseService } from '../../../users/users-base.service';
import { InspectionsService } from '../../inspections.service';
import { InspectionStatus } from '@app/core/models/inspections/inspection-base.model';
import * as moment from 'moment';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit, OnDestroy {

  @Input() roleLevelLoggedUser: number;

  filterActive: string;
  filterBy: { [key: string]: any } = {};

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  customAllFilter: ListItemModel = { id: null, name: 'All' };

  // buildingFilter: FormControl;
  buildings: ListItemModel[] = [];
  filteredBuildings$: Subject<any[]> = new Subject<any[]>();

  // employeeFilter: FormControl;
  employees: ListItemModel[] = [];
  filteredEmployees$: Subject<any[]> = new Subject<any[]>();

  beforeSnoozeDate: FormControl;

  today: Date = new Date();

  constructor(
    private inspectionService: InspectionsService,
    private buildingService: BuildingsService,
    private userService: UsersBaseService,
    private snackBar: MatSnackBar
  ) {
    this.filterActive = 'all';
    this.filterBy['status'] = -1;

    this.beforeSnoozeDate = new FormControl();
    this.beforeSnoozeDate.setValue(new Date(
      this.today.getFullYear(),
      this.today.getMonth(),
      this.today.getDate() + 30));
  }

  ngOnInit(): void {
    this.getBuildings();

    this.getEmployees();

    /*this.buildingFilter.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBuildings();
      }); */

    /*this.employeeFilter.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterEmployees();
      }); */

    this.beforeSnoozeDate.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBy['BeforeSnoozeDate'] = this.beforeSnoozeDate.value ? moment(this.beforeSnoozeDate.value).format('YYYY-MM-DD') : null;

        this.inspectionService.onFilterChanged.next(this.filterBy);
      });
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  /** BUILDING */
  getBuildings(): void {
    this.buildingService.getAllAsList('readallcbo', '', 0, 999, null, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.buildings = response.payload;
        this.buildings.unshift(this.customAllFilter);
        this.filteredBuildings$.next(response.payload);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching buildings', 'close', { duration: 1000 }));
  }

  /*private filterBuildings() {
    if (!this.buildings) {
      return;
    }
    // get the search keyword
    let search = this.buildingFilter.value;
    if (!search) {
      this.filteredBuildings$.next(this.buildings.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the buildings
    this.buildingService.getAllAsList('readallcbo', search, 0, 100, null, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings$.next(response.payload);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching buildings', 'close', { duration: 1000 }));

  } */

  /** EMPLOYEE */
  getEmployees(): void {
    this.userService.getAllAsList('readallcbo', '', 0, 99999, null)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.employees = response.payload;
        this.employees.unshift(this.customAllFilter);
        this.filteredEmployees$.next(this.employees);
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get employees', 'Close');
      });
  }

  /*filterEmployees() {
    if (!this.employees) {
      return;
    }

    let search = this.employeeFilter.value;
    if (!search) {
      this.filteredEmployees$.next(this.employees.slice());
      return;
    } else {
      search = search.toLowerCase();
    }

    this.userService.getAllAsList('readallcbo', search, 0, 100, null)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.employees = response.payload;
        // this.employees.unshift(this.customAllFilter);
        this.filteredEmployees$.next(this.employees);
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get employees', 'Close');
      });
  }*/

  /** FILTERS */
  changeFilter(filter: any): void {
    if (filter === 'all') {

      this.filterActive = 'all';
      this.filterBy = {};
      this.inspectionService.onFilterChanged.next();

    } else if (filter === 'pending') {

      this.filterActive = 'pending';
      this.filterBy['status'] = InspectionStatus.Pending;
      this.inspectionService.onFilterChanged.next(this.filterBy);

    } else if (filter === 'scheduled') {

      this.filterActive = 'scheduled';
      this.filterBy['status'] = InspectionStatus.Scheduled;
      this.inspectionService.onFilterChanged.next(this.filterBy);

    } else if (filter === 'walkthrough') {

      this.filterActive = 'walkthrough';
      this.filterBy['status'] = InspectionStatus.Walkthrough;
      this.inspectionService.onFilterChanged.next(this.filterBy);

    } else if (filter === 'walkthroughComplete') {

      this.filterActive = 'walkthroughComplete';
      this.filterBy['status'] = InspectionStatus.WalkthroughComplete;
      this.inspectionService.onFilterChanged.next(this.filterBy);

    } else if (filter === 'active') {

      this.filterActive = 'active';
      this.filterBy['status'] = InspectionStatus.Active;
      this.inspectionService.onFilterChanged.next(this.filterBy);

    } else if (filter === 'closed') {

      this.filterActive = 'closed';
      this.filterBy['status'] = InspectionStatus.Closed;
      this.inspectionService.onFilterChanged.next(this.filterBy);

    }
  }

  changeBuilding(buildingId: number): void {
    if (!buildingId) {
      this.filterBy['buildingId'] = null;
    } else {
      this.filterBy['buildingId'] = buildingId;
    }

    this.inspectionService.onFilterChanged.next(this.filterBy);
  }

  changeEmployee(employeeId: number): void {
    if (!employeeId) {
      this.filterBy['employeeId'] = null;
    } else {
      this.filterBy['employeeId'] = employeeId;
    }

    this.inspectionService.onFilterChanged.next(this.filterBy);
  }

}
