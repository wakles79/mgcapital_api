import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ListBuildingModel } from '@app/core/models/building/list-buildings.model';
import { Subject } from 'rxjs';
import { ListCustomerModel } from '@app/core/models/customer/list-customers.model';
import { ContractsService } from '../../contracts.service';
import { BuildingsService } from '../../../buildings/buildings.service';
import { CustomersService } from '../../../customers/customers.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { debounceTime, takeUntil } from 'rxjs/operators';
import * as moment from 'moment';

@Component({
  selector: 'app-contracts-main-sidenav',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit, OnDestroy {

  filterActive: string;
  filterBy: { [key: string]: string } = {};

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  updatedDateFrom: FormControl;
  updatedDateTo: FormControl;

  dateFilter: FormControl;

  buildings: ListBuildingModel[] = [];
  filteredBuildings$: Subject<any[]> = new Subject<any[]>();
  buildingIdCtrl: FormControl;
  selectedBuilding = 0;

  customerIdCtrl: FormControl;
  customers: ListCustomerModel[] = [];
  filteredCustomers$: Subject<any[]> = new Subject<any[]>();
  selectedCustomer = 0;


  constructor(
    private contractService: ContractsService,
    private buildingService: BuildingsService,
    private _customerService: CustomersService,
    public snackBar: MatSnackBar,
  ) {
    this.updatedDateFrom = new FormControl();
    this.updatedDateTo = new FormControl();
    this.dateFilter = new FormControl();
    this.buildingIdCtrl = new FormControl('');
    this.customerIdCtrl = new FormControl('');

    this.filterActive = '-1';
    this.filterBy = this.contractService.filterBy;
    this.loadFilters();
  }

  ngOnInit(): void {
    this.getBuildings();
    this.getCustomers();

    this.updatedDateFrom.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBy['UpdatedDateFrom'] = this.updatedDateFrom.value ? moment(this.updatedDateFrom.value).format('YYYY-MM-DD') : null;

        this.contractService.onFilterChanged.next(this.filterBy);
      });

    this.updatedDateTo.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBy['UpdatedDateTo'] = this.updatedDateTo.value ? moment(this.updatedDateTo.value).format('YYYY-MM-DD') : null;

        this.contractService.onFilterChanged.next(this.filterBy);
      });

    this.dateFilter.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBy['CreatedDate'] = this.dateFilter.value ? moment(this.dateFilter.value).format('YYYY-MM-DD') : null;

        this.contractService.onFilterChanged.next(this.filterBy);
      });

    this.buildingIdCtrl.valueChanges
      .pipe(
        debounceTime(300))
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBuildings();
      });

    this.customerIdCtrl.valueChanges
      .pipe(
        debounceTime(300))
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterCustomers();
      });

  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  changeFilter(filter: any): void {

    if (filter === '0') {
      this.filterActive = '0';
      this.filterBy['status'] = '0';
    } else if (filter === '1') {
      this.filterActive = '1';
      this.filterBy['status'] = '1';
    } else if (filter === '2') {
      this.filterActive = '2';
      this.filterBy['status'] = '2';
    } else if (filter === '3') {
      this.filterActive = '3';
      this.filterBy['status'] = '3';
    } else {
      this.filterActive = '-1';
      this.filterBy['status'] = '-1';
    }

    this.contractService.onFilterChanged.next(this.filterBy);
  }

  /** BUILDING */
  getBuildings(): void {
    this.buildingService.getAllAsList('readallcbo', '', 0, 999, null, {})
      .subscribe((response: { count: number, payload: ListBuildingModel[] }) => {
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
    let search = this.buildingIdCtrl.value;
    if (!search) {
      this.filteredBuildings$.next(this.buildings.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the buildings
    this.filteredBuildings$.next(
      this.buildings.filter(building => (building.code + building.name).toLowerCase().indexOf(search) > -1)
    );
  }

  changeBuilding(buildingId: string): void {
    this.selectedBuilding = Number(buildingId);
    if (buildingId === '0') {
      this.filterBy['buildingId'] = null;
    } else {

      this.filterBy['buildingId'] = buildingId.toString();
    }

    this.contractService.onFilterChanged.next(this.filterBy);
  }

  /** CUSTOMERS */
  getCustomers(): void {
    this._customerService.getAllAsList('readallcbo', '', 0, 999, null, {})
      .subscribe((response: { count: number, payload: ListCustomerModel[] }) => {
        this.customers = response.payload;
        this.filteredCustomers$.next(response.payload);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching customers', 'close', { duration: 1000 }));
  }

  private filterCustomers(): void {
    if (!this.customers) {
      return;
    }

    // get the search keyword
    let search = this.customerIdCtrl.value;
    if (!search) {
      this.filteredCustomers$.next(this.customers.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the customers
    this.filteredCustomers$.next(
      this.customers.filter(customer => (customer.code + customer.name).toLowerCase().indexOf(search) > -1)
    );
  }

  changeCustomer(customerId: string): void {
    this.selectedCustomer = Number(customerId);
    if (customerId === '0') {
      this.filterBy['customerId'] = null;
    } else {

      this.filterBy['customerId'] = customerId.toString();
    }

    this.contractService.onFilterChanged.next(this.filterBy);
  }

  // Default
  loadFilters(): void {
    if (Object.keys(this.filterBy).length > 0) {
      this.filterActive = this.filterBy['status'] ? this.filterBy['status'] : '-1';
      this.updatedDateFrom = new FormControl(this.filterBy['UpdatedDateFrom'] ? moment(this.filterBy['UpdatedDateFrom']).toDate() : '');
      this.updatedDateTo = new FormControl(this.filterBy['UpdatedDateTo'] ? moment(this.filterBy['UpdatedDateTo']).toDate() : '');
      this.dateFilter = new FormControl(this.filterBy['CreatedDate'] ? moment(this.filterBy['CreatedDate']).toDate() : '');
      this.selectedBuilding = this.filterBy['buildingId'] ? Number(this.filterBy['buildingId']) : 0;
    }
  }

}
