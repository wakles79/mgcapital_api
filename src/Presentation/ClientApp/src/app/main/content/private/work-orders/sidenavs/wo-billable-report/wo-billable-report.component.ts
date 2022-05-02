import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { MatSnackBar } from '@angular/material/snack-bar';
import { GenericFilterBaseModel, GenericFilterType } from '@app/core/models/common/generic-filter-base.model';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import * as moment from 'moment';
import { Subject, Subscription } from 'rxjs';
import { CustomersService } from '../../../customers/customers.service';
import { WoBillableReportService } from '../../wo-billable-report/wo-billable-report.service';

@Component({
  selector: 'app-wo-billable-report-sidenav',
  templateUrl: './wo-billable-report.component.html',
  styleUrls: ['./wo-billable-report.component.scss']
})
export class WoBillableReportSidenavComponent implements OnInit, OnDestroy {

  dateFrom = moment(moment().add(-1, 'd').toDate()).format('YYYY-MM-DD');
  dateTo = moment(moment().add().toDate()).format('YYYY-MM-DD');
  dateFromCtrl = new FormControl('', Validators.required);
  dateToCtrl = new FormControl('', Validators.required);
  customerIdCtrl = new FormControl('', Validators.nullValidator);

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  customerId: number;
  customers: ListItemModel[] = [];
  listCustomersSubscription: Subscription;

  // generic filter to list buildings
  buildingFilter: GenericFilterBaseModel;
  selectedBuildingIds = [];
  filterBy: { [key: string]: any } = {};

  constructor(
    private woBillableReportService: WoBillableReportService,
    public snackBar: MatSnackBar,
    private customerService: CustomersService
  ) {
    this.initializeFilters();
  }

  ngOnInit(): void {
    this.dateFromCtrl.setValue(this.woBillableReportService.dateFrom);
    this.dateToCtrl.setValue(this.woBillableReportService.dateTo);

    this.getcustomers();
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();

    if (this.listCustomersSubscription && !this.listCustomersSubscription.closed) {
      this.listCustomersSubscription.unsubscribe();
    }
  }

  initializeFilters(): void {
    this.buildingFilter = new GenericFilterBaseModel({
      identifier: 'buildingIds',
      displayName: 'Buildings',
      type: GenericFilterType.Select,
      apiURL: 'buildings/readallcbo',
      defaultValues: [],
      displayOptionAll: true,
      isRequired: false
    });

  }

  changeDates(event: MatDatepickerInputEvent<Date>): void {
    this.dateFrom = this.dateFromCtrl.value;
    this.dateTo = this.dateToCtrl.value;
    // update filters
    this.woBillableReportService.onFilterChanged.next({
      'dateFrom': moment(this.dateFrom).format('YYYY-MM-DD'),
      'dateTo': moment(this.dateTo).format('YYYY-MM-DD'),
      'customerId': this.customerId,
      'buildingIds': this.selectedBuildingIds
    });
    this.woBillableReportService.onDatesChanges.next({ 'dateFrom': this.dateFrom, 'dateTo': this.dateTo });
  }

  getcustomers(): void {
    this.customerService.getAllAsList('readallcbo', '', 0, 99999, null, { 'withContacts': '1' })
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.customers = response.payload;
      });
  }

  onChangeManagementCo(event: any): void {
    this.customerId = event.value;

    const selectedCustomer = this.customers.find(item => item.id === this.customerId);
    if (selectedCustomer) {
      this.customerService.onSelectedCustomersChanged.next(selectedCustomer);
    }

    this.woBillableReportService.onFilterChanged.next({
      'dateFrom': moment(this.dateFrom).format('YYYY-MM-DD'),
      'dateTo': moment(this.dateTo).format('YYYY-MM-DD'),
      'customerId': this.customerId,
      'buildingIds': this.selectedBuildingIds

    });

  }

  filterOnChanged($event: any): void {
    const identifier = $event.filter.identifier;
    const values = $event.selectedValues;

    if (identifier === 'buildingIds') {
      this.selectedBuildingIds = values;
    }

    this.updateSelectedFilters();
  }

  updateSelectedFilters(): void {
    this.woBillableReportService.onFilterChanged.next({
      'dateFrom': moment(this.dateFrom).format('YYYY-MM-DD'),
      'dateTo': moment(this.dateTo).format('YYYY-MM-DD'),
      'buildingIds': this.selectedBuildingIds,
      'customerId': this.customerId
    });
  }
}
