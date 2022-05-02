import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import * as moment from 'moment';
import { FormControl, Validators } from '@angular/forms';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ContractsService } from '../../../contracts/contracts.service';
import { CustomersService } from '../../../customers/customers.service';
import { BuildingsService } from '../../../buildings/buildings.service';
import { RevenuesService } from '../../revenues.service';
import { takeUntil } from 'rxjs/operators';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit, OnDestroy {

  filterActive: string;
  filterBy: { [key: string]: any } = {
    'contractId': 'null',
    'customerId': 'null',
    'buildingId': 'null',
    'dateFrom': '',
    'dateTo': '',
  };
  private _onDestroy = new Subject<void>();

  // Date filter
  dateFrom = moment(moment().add(-1, 'd').toDate()).format('YYYY-MM-DD');
  dateTo = moment(moment().add().toDate()).format('YYYY-MM-DD');
  dateFromCtrl = new FormControl('', Validators.required);
  dateToCtrl = new FormControl('', Validators.required);

  // buildingFilter: FormControl;
  searchBuilding: FormControl;
  searchBuildingCtrl: FormControl;
  buildingSelected = 0;
  buildings: ListItemModel[] = [];
  filteredBuildings$: Subject<any[]> = new Subject<any[]>();
  BuildingAllFilter: ListItemModel = { id: null, name: 'All' };

  // customerFilter: FormControl;
  searchCustomer: FormControl;
  searchCustomerCtrl: FormControl;
  customerSelected = 0;
  customers: ListItemModel[] = [];
  filteredCustomers$: Subject<any[]> = new Subject<any[]>();
  CustomerAllFilter: ListItemModel = { id: null, name: 'All' };


  // contractFilter: FormControl;
  searchContract: FormControl;
  searchContractCtrl: FormControl;
  contractSelected = 0;
  contracts: ListItemModel[] = [];
  filteredContracts$: Subject<any[]> = new Subject<any[]>();
  ContractAllFilter: ListItemModel = { id: null, name: 'All' };

  constructor(
    private snackBar: MatSnackBar,
    private contractService: ContractsService,
    private buildingService: BuildingsService,
    private customerService: CustomersService,
    private revenueService: RevenuesService
  ) {
    this.searchBuilding = new FormControl();
    this.searchBuildingCtrl = new FormControl();

    this.searchCustomer = new FormControl();
    this.searchCustomerCtrl = new FormControl();

    this.searchContract = new FormControl();
    this.searchContractCtrl = new FormControl();
    this.filterBy = {};
  }

  ngOnInit(): void {
    this.getBuildings();
    this.getCustomers();
    this.getContracts();

    this.revenueService.onFilterChanged.next(this.filterBy);

    this.searchBuildingCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBuildings();
      });

    this.searchContractCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterContracts();
      });

    this.searchCustomerCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterCustomers();
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
        this.buildings.unshift(this.BuildingAllFilter);
        this.filteredBuildings$.next(response.payload);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching buildings', 'close', { duration: 1000 }));
  }

  getCustomers(): void {
    this.customerService.getAllAsList('readallcbo', '', 0, 999, null, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.customers = response.payload;
        this.customers.unshift(this.CustomerAllFilter);
        this.filteredCustomers$.next(response.payload);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching customer', 'close', { duration: 1000 }));
  }


  getContracts(): void {
    this.contractService.getAllAsList('readallcbo', '', 0, 999, null, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.contracts = response.payload;
        this.contracts.unshift(this.ContractAllFilter);
        this.filteredContracts$.next(response.payload);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching contract', 'close', { duration: 1000 }));
  }

  changeBuilding(buildingId: number): void {
    if (!buildingId) {
      this.filterBy['buildingId'] = null;
    } else {
      this.filterBy['buildingId'] = buildingId;
    }

    this.revenueService.onFilterChanged.next(this.filterBy);
  }

  changeCustomer(customerId: number): void {
    if (!customerId) {
      this.filterBy['customerId'] = null;
    } else {
      this.filterBy['customerId'] = customerId;
    }

    this.revenueService.onFilterChanged.next(this.filterBy);
  }

  changeContract(contractId: number): void {
    if (!contractId) {
      this.filterBy['contractId'] = null;
    } else {
      this.filterBy['contractId'] = contractId;
    }

    this.revenueService.onFilterChanged.next(this.filterBy);
  }

  changeDates(event: MatDatepickerInputEvent<Date>): void {
    this.dateFrom = this.dateFromCtrl.value;
    this.dateTo = this.dateToCtrl.value;

    this.filterBy['dateFrom'] = moment(this.dateFrom).format('YYYY-MM-DD');
    this.filterBy['dateTo'] = moment(this.dateTo).format('YYYY-MM-DD');
    this.revenueService.onFilterChanged.next(this.filterBy);
    this.revenueService.onDatesChanges.next({ 'dateFrom': this.dateFrom, 'dateTo': this.dateTo });
  }

  // filter
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
        this.buildings.unshift(this.BuildingAllFilter);
        this.filteredBuildings$.next(response.payload);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching buildings', 'close', { duration: 1000 }));
  }

  private filterContracts(): void {
    if (!this.contracts) {
      return;
    }
    // get the search keyword
    let search = this.searchContractCtrl.value;
    if (!search) {
      this.filteredContracts$.next(this.contracts.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the buildings
    this.contractService.getAllAsList('readallcbo', search, 0, 100, this.contractSelected, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.contracts = response.payload;
        this.contracts.unshift(this.ContractAllFilter);
        this.filteredContracts$.next(response.payload);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching contracts', 'close', { duration: 1000 }));
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
    // filter the buildings
    this.customerService.getAllAsList('readallcbo', search, 0, 100, this.customerSelected, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.customers = response.payload;
        this.customers.unshift(this.CustomerAllFilter);
        this.filteredCustomers$.next(response.payload);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching customers', 'close', { duration: 1000 }));
  }

}
