import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { AuthService } from '@app/core/services/auth.service';
import { fuseAnimations } from '@fuse/animations';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BuildingsService } from '../../../buildings/buildings.service';
import { WorkOrdersService } from '../../work-orders.service';
import { WORK_ORDER_TYPES } from '@app/core/models/work-order/work-order-type.model';
import * as moment from 'moment';
import { WorkOrderServiceListItemModel } from '@app/core/models/work-order-services/work-order-services-list.model';
import { WorkOrderServicesService } from '../../../work-order-services/work-order-services.service';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss'],
  animations: fuseAnimations
})
export class MainComponent implements OnInit, OnDestroy {

  filterActive: string;
  filterBy: { [key: string]: any } = {};

  searchBuilding: FormControl;
  searchBuildingCtrl: FormControl;

  types: { id: number, name: string }[] = [];
  searchType: FormControl;

  dateFrom: FormControl;
  dateTo: FormControl;

  buildings: { id: number, name: string }[] = [];
  filteredBuildings$: Subject<ListItemModel[]> = new Subject<ListItemModel[]>();
  // represents id of building selected on the buildings mat-select
  buildingSelected = 0;
  roleLevelLoggedUser: any;
  filterOverDue: FormControl;

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  showUnscheduled = false;
  workOrderServiceCategoryId: FormControl;
  workOrderServiceId: FormControl;

  // WO Service
  categories: ListItemModel[] = [];
  selectedCategory: ListItemModel;
  workOrderServices: WorkOrderServiceListItemModel[] = [];
  selectedWorkOrderService: WorkOrderServiceListItemModel;


  filterBillableTasks: FormControl;
  resetFilters: boolean = false;

  constructor(
    private workOrderService: WorkOrdersService,
    private authService: AuthService,
    private buildingService: BuildingsService,
    public snackBar: MatSnackBar,
    public dialog: MatDialog,
    private _workOrderCategoryService: WorkOrderServicesService,
    private _snackBar: MatSnackBar,
  ) {

    this.searchBuilding = new FormControl();
    this.searchBuildingCtrl = new FormControl();
    this.filterOverDue = new FormControl();
    this.searchType = new FormControl();
    this.dateFrom = new FormControl();
    this.dateTo = new FormControl();
    this.workOrderServiceCategoryId = new FormControl();
    this.workOrderServiceId = new FormControl();
    this.filterBillableTasks = new FormControl();

    this.filterActive = 'all';
    this.filterBy['statusId'] = undefined;
    this.filterBy['serviceId'] = undefined;

    this.filterBy = this.workOrderService.filterBy;
    this.loadSessionFilters();

  }

  ngOnInit(): void {
    this.roleLevelLoggedUser = this.authService.currentUser.roleLevel;
    this.getTypesFromEnum();
    this.getBuildings();
    this.getWorkOrderServices();

    this.searchBuildingCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBuildings();
      });

    this.searchBuilding.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBy.buildingId = this.searchBuilding.value || null;
        if(!this.resetFilters)
          this.workOrderService.onFilterChanged.next(this.filterBy);
      });

    this.filterOverDue.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBy['isExpired'] = this.filterOverDue.value || null;
        if(!this.resetFilters)
          this.workOrderService.onFilterChanged.next(this.filterBy);
      });

    this.searchType.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBy['typeId'] = this.searchType.value === undefined ? null : this.searchType.value;
        if(!this.resetFilters)
          this.workOrderService.onFilterChanged.next(this.filterBy);
      });

    this.dateFrom.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBy['DateFrom'] = this.dateFrom.value ? moment(this.dateFrom.value).format('YYYY-MM-DD') : null;

        if(!this.resetFilters)
        this.workOrderService.onFilterChanged.next(this.filterBy);
      });

    this.dateTo.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBy['DateTo'] = this.dateTo.value ? moment(this.dateTo.value).format('YYYY-MM-DD') : null;
        if(!this.resetFilters)
        this.workOrderService.onFilterChanged.next(this.filterBy);
      });

    this.workOrderServiceCategoryId.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((value) => {

        this.filterBy['categoryId'] = value;
        if (value.length > 0) {
          this.getWorkOrderServices(value);
        } else {
          this.getWorkOrderServices();
        }

        if(!this.resetFilters)
        this.workOrderService.onFilterChanged.next(this.filterBy);

      });

    this.workOrderServiceId.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((value) => {

        if (value.length > 0) {
          this.filterBy['serviceId'] = value;
        } else {
          this.filterBy['serviceId'] = null;
        }

        if(!this.resetFilters)
        this.workOrderService.onFilterChanged.next(this.filterBy);

      });


    this.filterBillableTasks.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBy['isBillable'] = this.filterBillableTasks.value || null;
        if(!this.resetFilters)
        this.workOrderService.onFilterChanged.next(this.filterBy);
      });

    this.getCategories();

  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  changeFilter(filter: any): void {

    this.resetFilters = false;
    if (filter === 'all') {
      this.filterActive = 'all';
      this.filterBy = {};

      this.resetFilters = true;
      this.cleanFilters();
      this.resetFilters = false;

      this.workOrderService.onFilterChanged.next();
    }

    if (filter === 'myWorkOrders') {
      this.filterActive = 'myWorkOrders';
      this.filterBy['statusId'] = null;
      this.filterBy['administratorId'] = this.authService.currentUser.employeeId;
      this.workOrderService.onFilterChanged.next(this.filterBy);
    }

    if (filter === 'standBy') {
      this.filterActive = 'standBy';
      this.filterBy['administratorId'] = null;
      this.filterBy['statusId'] = 1;
      this.workOrderService.onFilterChanged.next(this.filterBy);
    }

    if (filter === 'active') {
      this.filterActive = 'active';
      this.filterBy['administratorId'] = null;
      this.filterBy['statusId'] = 2;
      this.workOrderService.onFilterChanged.next(this.filterBy);
    }

    if (filter === 'draft') {
      this.filterActive = 'draft';
      this.filterBy['administratorId'] = null;
      this.filterBy['statusId'] = 0;
      this.workOrderService.onFilterChanged.next(this.filterBy);
    }

    if (filter === 'closed') {
      this.filterActive = 'closed';
      this.filterBy['administratorId'] = null;
      this.filterBy['statusId'] = 3;
      this.workOrderService.onFilterChanged.next(this.filterBy);
    }

    if (filter === 'cancelled') {
      this.filterActive = 'cancelled';
      this.filterBy['administratorId'] = null;
      this.filterBy['statusId'] = 4;
      this.workOrderService.onFilterChanged.next(this.filterBy);
    }
  }

  getBuildings(): void {
    this.buildingService.getAllAsList('readallcbo', '', 0, 100, this.buildingSelected, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings$.next(response.payload);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching buildings', 'close', { duration: 1000 }));
  }

  private filterBuildings(): void {
    if (!this.buildings) {
      return;
    }
    // get the search keyword
    let search = this.searchBuildingCtrl.value;
    if (!search) {
      this.filteredBuildings$.next(this.buildings.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the buildings
    this.buildingService.getAllAsList('readallcbo', search, 0, 100, this.buildingSelected, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings$.next(response.payload);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching buildings', 'close', { duration: 1000 }));

  }

  getTypesFromEnum(): void {
    // tslint:disable-next-line: forin
    for (const type in WORK_ORDER_TYPES) {
      this.types.push({ id: WORK_ORDER_TYPES[type].key, name: WORK_ORDER_TYPES[type].value });
    }
  }

  filterUnscheduled(): void {
    this.showUnscheduled = !this.showUnscheduled;
    this.filterBy['unscheduled'] = this.showUnscheduled;
    this.workOrderService.onFilterChanged.next(this.filterBy);
  }

  // Session Filter
  loadSessionFilters(): void {
    if (Object.keys(this.filterBy).length > 0) {

      if (this.filterBy['statusId'] !== undefined) {
        this.filterActive = this.getFilterActiveByKey(this.filterBy['statusId']);
      }

      if (this.filterBy['buildingId']) {
        this.searchBuilding.setValue(this.filterBy['buildingId']);
      }

      if (this.filterBy['isExpired']) {
        this.filterOverDue.setValue(this.filterBy['isExpired']);
      }

      if (this.filterBy['typeId'] !== undefined && this.filterBy['typeId'] !== null) {
        this.searchType.setValue(this.filterBy['typeId']);
      }

      if (this.filterBy['unscheduled'] !== undefined && this.filterBy['unscheduled'] !== null) {
        this.showUnscheduled = this.filterBy['unscheduled'];
      }

      if (this.filterBy['categoryId'] !== undefined && this.filterBy['categoryId'] !== null) {
        const workOrderServiceCategoryId = this.filterBy['categoryId'];
        this.workOrderServiceCategoryId.setValue(workOrderServiceCategoryId);

        if (workOrderServiceCategoryId.length > 0) {
          this.getWorkOrderServices(this.workOrderServiceCategoryId.value);
        } else {
          this.getWorkOrderServices();
        }

      }

      if (this.filterBy['serviceId'] !== undefined && this.filterBy['serviceId'] !== null) {
        const workOrderServiceId = this.filterBy['serviceId'];
        this.workOrderServiceId.setValue(workOrderServiceId);
      }

      if (this.filterBy['DateFrom'] !== undefined && this.filterBy['DateFrom'] !== null) {
        const from = moment(this.filterBy['DateFrom']).toDate();
        this.dateFrom.setValue(from);
      }

      if (this.filterBy['DateTo'] !== undefined && this.filterBy['DateTo'] !== null) {
        const to = moment(this.filterBy['DateTo']).toDate();
        this.dateTo.setValue(to);
      }

      if (this.filterBy['isBillable']) {
        this.filterBillableTasks.setValue(this.filterBy['isBillable']);
      }

    }
  }

  getFilterActiveByKey(key: number): string {
    let selected = '';
    switch (key) {
      case 0:
        selected = 'draft';
        break;
      case 1:
        selected = 'standBy';
        break;
      case 2:
        selected = 'active';
        break;
      case 3:
        selected = 'closed';
        break;
      case 4:
        selected = 'cancelled';
        break;
      default:
        selected = 'myWorkOrders';
        break;
    }
    return selected;
  }

  cleanFilters(): void {
    this.searchBuilding.setValue('');
    this.filterOverDue.setValue(false);
    this.searchType.setValue(null);
    this.showUnscheduled = false;
    this.dateFrom.setValue(null);
    this.dateTo.setValue(null);
    this.workOrderServiceId.setValue('');
    this.workOrderServiceCategoryId.setValue('');
    this.filterBillableTasks.setValue(false);

  }

  // Categories
  getCategories(): void {
    this.categories = [];
    this._workOrderCategoryService.getAllAsList('ReadAllCategoriesCbo', '', 0, 20, null, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {

        this.categories = response.payload;

      }, (error) => {
        this._snackBar.open('Ops! Error when trying to get categories', 'Close');
      });
  }

  // Services
  getWorkOrderServices(categoryIds: any = null): void {
    this.workOrderServices = [];
    this._workOrderCategoryService.getAllAsList('ReadAllServicesCbo', '', 0, 20, null, { 'categoryIds': categoryIds })
      .subscribe((response: { count: number, payload: WorkOrderServiceListItemModel[] }) => {

        this.workOrderServices = response.payload;

      }, (error) => {
        this._snackBar.open('Ops! Error when trying to get services', 'Close');
      });
  }

}
