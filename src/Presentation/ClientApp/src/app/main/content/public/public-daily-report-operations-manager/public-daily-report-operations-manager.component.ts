import { AfterViewInit, Component, OnInit, ViewEncapsulation, OnDestroy, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfigService } from '@fuse/services/config.service';
import { merge, Observable, Subscription } from 'rxjs';
import { WORK_ORDER_STATUS } from '@app/core/models/work-order/work-order-status.model';
import { tap } from 'rxjs/operators';
import * as moment from 'moment';
import { DataSource } from '@angular/cdk/table';
import { PublicDailyReportService } from '@app/main/content/public/public-daily-report-operations-manager/public-daily-report.service';

@Component({
  selector: 'app-public-daily-report-operations-manager',
  templateUrl: './public-daily-report-operations-manager.component.html',
  styleUrls: ['./public-daily-report-operations-manager.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class PublicDailyReportOperationsManagerComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  loading$ = this.publicDailyReportService.loadingSubject.asObservable();
  get workOrdersCount(): number { return this.publicDailyReportService.elementsCount; }

  dailyReportSubscription: Subscription;

  // user who generates the report
  loggedUser: any;
  selectedOperationsManager: any = {};
  dateFrom: any;
  dateTo: any;

  statusToFilter: any[] = [];
  filterStatus: FormControl;
  selectedStatusesToFilter = '';

  dataSource: DailyWoReportDataSource | null;
  displayedColumns = ['number', 'locationName', 'description', 'dateSubmitted', 'dueDate', 'statusName', 'hasImages', 'buttons'];

  constructor(
    private fuseConfig: FuseConfigService,
    private publicDailyReportService: PublicDailyReportService,
  ) {

    this.fuseConfig.config = {
      layout: {
        navbar: {
          hidden: true
        },
        toolbar: {
          hidden: true
        },
        footer: {
          hidden: true
        },
        sidepanel: {
          hidden: true
        }
      }
    };

    this.dateFrom = this.publicDailyReportService.dailyReportOmDateFrom;
    this.dateTo = this.publicDailyReportService.dailyReportOmdateTo;
    this.selectedOperationsManager = this.publicDailyReportService.selectedOperationsManager;
    this.loggedUser = this.publicDailyReportService.loggedUser;

    this.filterStatus = new FormControl([WORK_ORDER_STATUS.find(status => status.value === 'stand-by').key,
    WORK_ORDER_STATUS.find(status => status.value === 'active').key]);
  }

  ngOnInit(): void {
    this.dataSource = new DailyWoReportDataSource(this.publicDailyReportService);
    this.getStatusesToFilter();
  }

  ngAfterViewInit(): void {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.publicDailyReportService.getElements(
          'ReadDailyReportByOperationsManager', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize,
          {
            operationsManagerId: this.selectedOperationsManager.id,
            dateFrom: this.dateFrom,
            dateTo: this.dateTo,
            statuses: this.selectedStatusesToFilter
          }))
      )
      .subscribe();
  }

  get dateFromStringFormat(): string {
    return moment(this.dateFrom).format('YYYY-MM-DD');
  }

  get dateToStringFormat(): string {
    return moment(this.dateTo).format('YYYY-MM-DD');
  }

  // Show dueDate if it has a valid value, valid value is a date different of null or default value
  showDueDate(value: any): any {
    const f2 = new Date(value);
    const f1 = new Date('2000-01-01');
    return (f2 < f1) ? '-' : f2;
  }

  ngClassWorkOrderStatus(statusId: any): string {
    return WORK_ORDER_STATUS.find(item => item.key === statusId).value;
  }

  getWorkOrderStatus(statusId: any): string {
    return (WORK_ORDER_STATUS.find(item => item.key === statusId).value).replace(/-/g, ' ');
  }

  getStatusesToFilter(): void {
    WORK_ORDER_STATUS.forEach(status => {
      this.statusToFilter.push({ id: status.key, value: status.value.replace(/-/g, ' ') });
    });
    this.statusToFilter.splice((this.statusToFilter.indexOf(s => s.value === 'draft') + 1), 1);
  }

  onStatusChange($event): void {
    this.selectedStatusesToFilter = '';

    if ($event.value[0]) {
      $event.value.forEach(element => {
        this.selectedStatusesToFilter = this.selectedStatusesToFilter.concat(element + '_');
      });
      this.selectedStatusesToFilter = this.selectedStatusesToFilter.substring(0, this.selectedStatusesToFilter.length - 1);
    }

    this.publicDailyReportService.getElements(
      'ReadDailyReportByOperationsManager', '',
      this.sort.active,
      this.sort.direction,
      this.paginator.pageIndex,
      this.paginator.pageSize,
      {
        operationsManagerId: this.selectedOperationsManager.id,
        dateFrom: this.dateFrom,
        dateTo: this.dateTo,
        statuses: this.selectedStatusesToFilter
      });
  }

  hasImages(attachmentsCount): any {
    if (attachmentsCount > 0) { return 'yes'; } else { return 'no'; }
  }

  isEmpty(obj): boolean {
    return (obj && (Object.keys(obj).length === 0));
  }

  openNewTapPublicWorkOrder(workOrder): void {
    const urlPublicWorkOrder = 'http://' + window.location.host + '/work-orders/' + workOrder.guid;
    window.open(urlPublicWorkOrder, '_blank');
  }

  ngOnDestroy(): void {
  }

}

export class DailyWoReportDataSource extends DataSource<any>
{
  constructor(private publicDailyReportService: PublicDailyReportService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.publicDailyReportService.allElementsChanged;
  }

  disconnect(): void {
  }

}
