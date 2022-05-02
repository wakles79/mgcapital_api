import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { ListScheduleCategoryModel } from '@app/core/models/schedule-category/list-schedule-category.model';
import { fuseAnimations } from '@fuse/animations';
import { Subject } from 'rxjs';
import { ListScheduleSubCategoryModel } from '@app/core/models/schedule-subcategory/list-schedule-subcategory.model';
import { WoCalendarService } from '../../wo-calendar.service';
import { BuildingsService } from '../../../buildings/buildings.service';
import { CustomersService } from '../../../customers/customers.service';
import { ScheduleSettingsCategoryService } from '../../../schedule-settings-category/schedule-settings-category.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { takeUntil } from 'rxjs/operators';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { WORK_ORDER_TYPES } from '@app/core/models/work-order/work-order-type.model';
import * as moment from 'moment';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class MainComponent implements OnInit {

  today: Date = new Date();
  filterBy: { [key: string]: any } = {};

  searchBuilding: FormControl;
  searchBuildingCtrl: FormControl;
  buildings: { id: number, name: string }[] = [];
  filteredBuildings$: Subject<ListItemModel[]> = new Subject<ListItemModel[]>();
  // represents id of building selected on the buildings mat-select
  buildingSelected = 0;

  searchCustomer: FormControl;
  searchCustomerCtrl: FormControl;
  customers: { id: number, name: string }[] = [];
  filteredCustomers$: Subject<ListItemModel[]> = new Subject<ListItemModel[]>();
  // represents id of building selected on the buildings mat-select
  customerSelected = 0;

  dateFrom: FormControl;
  dateTo: FormControl;

  types: { id: number, name: string }[] = [];
  searchType: FormControl;


  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  // Filter Category
  scheduleCategories: { id: number, description: string }[] = [];
  filteredscheduleCategories$: Subject<ListScheduleCategoryModel[]> = new Subject<ListScheduleCategoryModel[]>();
  scheduleCategorySelected = 0;
  scheduleCategory: FormControl;

  // Filter SubCategory

  scheduleSubCategories: ListScheduleSubCategoryModel[] = [];
  filteredscheduleSubCategories$: Subject<ListScheduleSubCategoryModel[]> = new Subject<ListScheduleSubCategoryModel[]>();
  scheduleSubCategorySelected = 0;
  scheduleSubCategory: FormControl;

  // Status Filter
  approvedStatus: boolean = false;
  unapprovedStatus: boolean = false;

  // Data Confirmed Filter
  confirmedScheduleDate: boolean = false;
  notConfirmedScheduleDate: boolean = false;

  constructor(
    private _calendarService: WoCalendarService,
    private _buildingService: BuildingsService,
    private _customerService: CustomersService,
    private schedueSettingsCategoryService: ScheduleSettingsCategoryService,
    private _snackBar: MatSnackBar
  ) {
    this.filterBy = this._calendarService.filterBy;

    this.searchType = new FormControl();

    this.searchBuilding = new FormControl();
    this.searchBuildingCtrl = new FormControl();

    this.searchCustomer = new FormControl();
    this.searchCustomerCtrl = new FormControl();

    this.scheduleCategory = new FormControl();
    this.scheduleSubCategory = new FormControl();

    this.loadSessionFilter();
  }

  ngOnInit(): void {
    this.getTypesFromEnum();
    this.getCustomers();
    this.getBuildings();
    this.getScheduleCategory();

    this.searchBuildingCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBuildings();
      });

    this.searchBuilding.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBy['BuildingId'] = this.searchBuilding.value || null;
        this._calendarService.onFilterChanged.next(this.filterBy);
      });


    this.searchCustomerCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterCustomers();
      });

    this.searchCustomer.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBy['CustomerId'] = this.searchCustomer.value || null;
        this._calendarService.onFilterChanged.next(this.filterBy);
      });

    // this.dateFrom.valueChanges
    //   .pipe(takeUntil(this._onDestroy))
    //   .subscribe(() => {
    //     this.filterBy['DateFrom'] = this.dateFrom.value ? moment(this.dateFrom.value).format('YYYY-MM-DD') : null;

    //     this._calendarService.onFilterChanged.next(this.filterBy);
    //   });

    // this.dateTo.valueChanges
    //   .pipe(takeUntil(this._onDestroy))
    //   .subscribe(() => {
    //     this.filterBy['DateTo'] = this.dateTo.value ? moment(this.dateTo.value).format('YYYY-MM-DD') : null;
    //     this._calendarService.onFilterChanged.next(this.filterBy);
    //   });

    this.searchType.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((value) => {
        this.filterBy['TypeId'] = value >= 0 ? value : -1;
        this._calendarService.onFilterChanged.next(this.filterBy);
      });

    this.scheduleCategory.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((value) => {
        this.filterBy['ScheduleCategory'] = value;
        this._calendarService.onFilterChanged.next(this.filterBy);
        if (value.length > 0) {
          this.getScheduleSubCategory(this.filterBy);
        } else {
          this.scheduleSubCategory.reset();
          this.scheduleSubCategories = [];
          this.filterBy['ScheduleSubCategory'] = this.scheduleSubCategories;
          this.filteredscheduleSubCategories$.next(this.scheduleSubCategories);
        }

        this._calendarService.onFilterChanged.next(this.filterBy);
      });

    this.scheduleSubCategory.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((value) => {
        this.filterBy['ScheduleSubCategory'] = value;
        this._calendarService.onFilterChanged.next(this.filterBy);
        // this.getScheduleSubCategory(this.filterBy);
      });



    this._calendarService.onFilterChanged.subscribe((filterBy: { [key: string]: string }) => {
      const dFromArray = filterBy['DateFrom'].split('-');
      const dToArray = filterBy['DateTo'].split('-');
      this.dateFrom.setValue(new Date(Number(dFromArray[0]), Number(dFromArray[1]) - 1, Number(dFromArray[2])));
      this.dateTo.setValue(new Date(Number(dToArray[0]), Number(dToArray[1]) - 1, Number(dToArray[2])));
    });
  }

  // FILTERS
  dueDateStatusFilterChanged(event: MatCheckboxChange, status: number): void {
    try {
      // this.filterBy['DueDateStatus'] = event.checked ? status : null;
      // this._calendarService.onFilterChanged.next(this.filterBy);
      if (this.filterBy['DueDateStatus'] === status || this.filterBy['DueDateStatus'] === undefined || this.filterBy['DueDateStatus'] === null) {
        this.filterBy['DueDateStatus'] = event.checked ? status : null;
        this._calendarService.onFilterChanged.next(this.filterBy);
      } else {
        if (this.filterBy['DueDateStatus'] === 3) {
          if (status === 0) {
            this.filterBy['DueDateStatus'] = 1;
            this._calendarService.onFilterChanged.next(this.filterBy);
          } else {
            this.filterBy['DueDateStatus'] = 0;
            this._calendarService.onFilterChanged.next(this.filterBy);
          }
        } else {
          this.filterBy['DueDateStatus'] = 3;
          this._calendarService.onFilterChanged.next(this.filterBy);
        }
      }
    } catch (ex) {
      console.log(ex);
    }
  }

  scheduleDateFilterChanged(event: MatCheckboxChange, status: number): void {
    try {
      if (this.filterBy['ScheduleDateConfirmed'] === status || this.filterBy['ScheduleDateConfirmed'] === undefined || this.filterBy['ScheduleDateConfirmed'] === null) {
        this.filterBy['ScheduleDateConfirmed'] = event.checked ? status : null;
        this._calendarService.onFilterChanged.next(this.filterBy);
      } else {
        if (this.filterBy['ScheduleDateConfirmed'] === 3) {
          if (status === 0) {
            this.filterBy['ScheduleDateConfirmed'] = 1;
            this._calendarService.onFilterChanged.next(this.filterBy);
          } else {
            this.filterBy['ScheduleDateConfirmed'] = 0;
            this._calendarService.onFilterChanged.next(this.filterBy);
          }
        } else {
          this.filterBy['ScheduleDateConfirmed'] = 3;
          this._calendarService.onFilterChanged.next(this.filterBy);
        }
      }
    } catch (ex) {
      console.log(ex);
    }
  }

  clienteApprovedFilterChanged(event: MatCheckboxChange, status: number): void {
    try {
      // this.filterBy['ApprovedStatus'] = event.checked ? status : null;
      // this._calendarService.onFilterChanged.next(this.filterBy);

      if (this.filterBy['ApprovedStatus'] === status || this.filterBy['ApprovedStatus'] === undefined || this.filterBy['ApprovedStatus'] === null) {
        this.filterBy['ApprovedStatus'] = event.checked ? status : null;
        this._calendarService.onFilterChanged.next(this.filterBy);
      } else {
        if (this.filterBy['ApprovedStatus'] === 3) {
          if (status === 0) {
            this.filterBy['ApprovedStatus'] = 1;
            this._calendarService.onFilterChanged.next(this.filterBy);
          } else {
            this.filterBy['ApprovedStatus'] = 0;
            this._calendarService.onFilterChanged.next(this.filterBy);
          }
        } else {
          this.filterBy['ApprovedStatus'] = 3;
          this._calendarService.onFilterChanged.next(this.filterBy);
        }
      }
    } catch (ex) {
      console.log(ex);
    }
  }

  // TYPES
  getTypesFromEnum(): void {
    this.types = [];
    this.types.push({ id: -1, name: 'All' });
    // tslint:disable-next-line: forin
    for (const type in WORK_ORDER_TYPES) {
      this.types.push({ id: WORK_ORDER_TYPES[type].key, name: WORK_ORDER_TYPES[type].value });
    }
  }

  // BUILDINGS
  getBuildings(): void {
    this._buildingService.getAllAsList('readallcbo', '', 0, 25, this.buildingSelected, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings$.next(response.payload);
      },
        (error) => this._snackBar.open('Oops, there was an error fetching buildings', 'close', { duration: 1000 }));
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
    this._buildingService.getAllAsList('readallcbo', search, 0, 25, this.buildingSelected, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings$.next(response.payload);
      },
        (error) => this._snackBar.open('Oops, there was an error fetching buildings', 'close', { duration: 1000 }));

  }

  // CUSTOMERS
  getCustomers(): void {
    this._customerService.getAllAsList('readallcbo', '', 0, 25, this.customerSelected, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.customers = response.payload;
        this.filteredCustomers$.next(response.payload);
      },
        (error) => this._snackBar.open('Oops, there was an error fetching customers', 'close', { duration: 1000 }));
  }

  private filterCustomers(): void {
    if (!this.customers) {
      return;
    }
    // get the search keyword
    let search = this.searchCustomerCtrl.value;
    if (!search) {
      this.filteredCustomers$.next(this.customers.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter
    this._customerService.getAllAsList('readallcbo', search, 0, 25, this.customerSelected, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.customers = response.payload;
        this.filteredCustomers$.next(response.payload);
      },
        (error) => this._snackBar.open('Oops, there was an error fetching customers', 'close', { duration: 1000 }));
  }

  getScheduleCategory(filter = ''): void {
    this.schedueSettingsCategoryService.getAllAsList('readallcbo', filter, 0, 20, this.buildingSelected, {})
      .subscribe((response: { count: number, payload: ListScheduleCategoryModel[] }) => {
        this.scheduleCategories = response.payload;
        this.filteredscheduleCategories$.next(response.payload);
      },
        (error) => this._snackBar.open('Oops, there was an error fetching schedule categories', 'close', { duration: 1000 }));
  }

  getScheduleSubCategory(filter: any): void {
    this.schedueSettingsCategoryService.getAllAsListBySubCategory(this.filterBy)
      .subscribe((response: { count: number, payload: ListScheduleSubCategoryModel[] }) => {
        this.scheduleSubCategories = response.payload;
        this.filteredscheduleSubCategories$.next(this.scheduleSubCategories);
      }, (error) => {
        this._snackBar.open('Ops! Error when trying to get SubCategory', 'Close');
      });
  }

  // Session Filter
  loadSessionFilter(): void {
    if (Object.keys(this.filterBy).length > 0) {

      const typeId = this.filterBy['TypeId'] == null ? 4 : this.filterBy['TypeId'];
      this.filterBy['TypeId'] = typeId;
      this.searchType.setValue(typeId);

      const from = this.filterBy['DateFrom'] ? moment(this.filterBy['DateFrom']).toDate() : new Date(this.today.getFullYear(), this.today.getMonth(), 1);
      const to = this.filterBy['DateTo'] ? moment(this.filterBy['DateTo']).toDate() : new Date(this.today.getFullYear(), this.today.getMonth() + 1, 0);

      this.dateFrom = new FormControl({ value: from, disabled: true });
      this.dateTo = new FormControl({ value: to, disabled: true });

      this.filterBy['DateFrom'] = moment(this.dateFrom.value).format('YYYY-MM-DD');
      this.filterBy['DateTo'] = moment(this.dateTo.value).format('YYYY-MM-DD');

      if (this.filterBy['CustomerId']) {
        this.searchCustomer = new FormControl(this.filterBy['CustomerId']);
      }

      if (this.filterBy['BuildingId']) {
        this.searchBuilding = new FormControl(this.filterBy['BuildingId']);
      }

      if (this.filterBy['ScheduleCategory']) {
        const scheduleSelected = this.filterBy['ScheduleCategory'];
        this.scheduleCategory.setValue(scheduleSelected);

        if (scheduleSelected.length > 0) {
          this.getScheduleSubCategory(this.filterBy['ScheduleCategory']);
        }
      }

      if (this.filterBy['ScheduleSubCategory']) {
        const subcategorySelected = this.filterBy['ScheduleCategory'];
        if (subcategorySelected.length > 0) {
          this.scheduleSubCategory.setValue(subcategorySelected);
        }
      }

      const approvedStatusFilter = this.filterBy['ApprovedStatus'] == null ? 3 : this.filterBy['ApprovedStatus'];
      this.approvedStatus = approvedStatusFilter === 1 ? true : false;
      this.unapprovedStatus = approvedStatusFilter === 0 ? true : false;

      const scheduleDateFilter = this.filterBy['ScheduleDateConfirmed'] == null ? null : this.filterBy['ScheduleDateConfirmed'];
      if (scheduleDateFilter) {
        this.confirmedScheduleDate = scheduleDateFilter === 1 ? true : false;
        this.notConfirmedScheduleDate = scheduleDateFilter === 0 ? true : false;
      }
    } else {

      this.filterBy['TypeId'] = 4;
      this.searchType = new FormControl(4);

      const from = new Date(this.today.getFullYear(), this.today.getMonth(), 1);
      const to = new Date(this.today.getFullYear(), this.today.getMonth() + 1, 0);

      this.dateFrom = new FormControl({ value: from, disabled: true });
      this.dateTo = new FormControl({ value: to, disabled: true });

      this.filterBy['DateFrom'] = moment(this.dateFrom.value).format('YYYY-MM-DD');
      this.filterBy['DateTo'] = moment(this.dateTo.value).format('YYYY-MM-DD');

    }

    this._calendarService.onFilterChanged.next(this.filterBy);
  }

}
