import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { GenericFilterBaseModel, GenericFilterType } from '../../../../../../core/models/common/generic-filter-base.model';
import { UsersService } from '../../../users/users.service';
import { DailyWoReportByOperationsManagerService } from '../../wo-daily-report-operations-manager.service';
import { USER_ROLE_LEVELS } from '../../../../../../core/models/user/user-role-level';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';

@Component({
  selector: 'app-wo-daily-report-sidenav',
  templateUrl: './wo-daily-report.component.html',
  styleUrls: ['./wo-daily-report.component.scss']
})
export class WoDailyReportSidenavComponent implements OnInit, OnDestroy {

  dateFrom = moment(moment().add(-1, 'd').toDate()).format('YYYY-MM-DD');
  dateTo = moment(moment().add().toDate()).format('YYYY-MM-DD');
  dateFromCtrl = new FormControl('', Validators.required);
  dateToCtrl = new FormControl('', Validators.required);

  operationsManagerId: any = null;
  searchOperationsManager = new FormControl('All');
  operationsManagers: any = {};
  operationsManagerRoleLevel: any;

  // generic filter to list buildings
  buildingFilter: GenericFilterBaseModel;
  selectedBuildingIds = [];

  // generic filter to list trip status
  statusFilter: GenericFilterBaseModel;
  selectedStatusIds = [];

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  constructor(
    private dailyReportService: DailyWoReportByOperationsManagerService,
    private usersService: UsersService,
    public snackBar: MatSnackBar,
  ) {

    this.initializeFilters();
  }

  ngOnInit(): void {
    this.dateFromCtrl.setValue(this.dailyReportService.dateFrom);
    this.dateToCtrl.setValue(this.dailyReportService.dateTo);
    this.searchOperationsManager.setValue(this.dailyReportService.selectedOperationsManager.id || 'All');

    this.operationsManagerRoleLevel = USER_ROLE_LEVELS.find(item => item.key === 'Operations_Manager').value;
    this.getOperationsManagers();
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  initializeFilters(): void {
    this.buildingFilter = new GenericFilterBaseModel({
      identifier: 'buildingIds',
      displayName: 'Buildings',
      type: GenericFilterType.Multiselect,
      apiURL: 'buildings/readallcbo',
      defaultValues: [],
      displayOptionAll: true,
      isRequired: false
    });

    this.statusFilter = new GenericFilterBaseModel({
      identifier: 'statusIds',
      displayName: 'Status',
      type: GenericFilterType.Multiselect,
      displayOptionAll: true,
      isRequired: false,
      defaultValues: [],
      values: [
        { id: 0, name: 'Draft' },
        { id: 1, name: 'Stand By' },
        { id: 2, name: 'Active' },
        { id: 3, name: 'Closed' }
      ]
    });
  }

  updateSelectedFilters(): void {
    this.dailyReportService.onFilterChanged.next({
      operationsManagerId: this.operationsManagerId,
      'dateFrom': moment(this.dateFrom).format('YYYY-MM-DD'),
      'dateTo': moment(this.dateTo).format('YYYY-MM-DD'),
      'buildingIds': this.selectedBuildingIds,
      'statusIds': this.selectedStatusIds
    });
  }

  changeDates(event: MatDatepickerInputEvent<Date>): void {
    this.dateFrom = this.dateFromCtrl.value;
    this.dateTo = this.dateToCtrl.value;
    // update filters
    this.updateSelectedFilters();
    this.dailyReportService.onDatesChanges.next({ 'dateFrom': this.dateFrom, 'dateTo': this.dateTo });
  }

  getOperationsManagers(): void {
    this.usersService.getAll('readall', '', '', '', 0, 100, { 'roleLevel': this.operationsManagerRoleLevel })
      .subscribe((response: { count: number, payload: any[] }) => {
        this.operationsManagers = response.payload;
      },
        (error) => this.snackBar.open('Oops, there was an error fetching the operations managers', 'close', { duration: 2000 }));
  }

  onChangeOperationsManager(event: any): void {

    if (event.value === 'All') {
      this.operationsManagerId = null;
    }
    else {
      this.operationsManagerId = event.value;
    }

    // In case will be necessary to know in another component the selected operations manager
    const selectedOperationsManager = this.operationsManagers.find(item => item.id === event.value);
    if (selectedOperationsManager) {
      this.dailyReportService.onSelectedOperationsManagerChange.next(selectedOperationsManager);
    }

    this.updateSelectedFilters();
  }

  filterOnChanged($event: any): void {
    const identifier = $event.filter.identifier;
    const values = $event.selectedValues;

    if (identifier === 'buildingIds') {
      this.selectedBuildingIds = values;
    }

    if (identifier === 'statusIds') {
      this.selectedStatusIds = values;
    }

    this.updateSelectedFilters();
  }
}
