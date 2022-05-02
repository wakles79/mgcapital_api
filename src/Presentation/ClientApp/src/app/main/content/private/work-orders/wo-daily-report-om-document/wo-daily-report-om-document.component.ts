import { DataSource } from '@angular/cdk/table';
import { DatePipe } from '@angular/common';
import { AfterViewInit, Component, OnDestroy, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { WORK_ORDER_STATUS } from '@app/core/models/work-order/work-order-status.model';
import { ShareUrlDialogComponent } from '@app/core/modules/share-url-dialog/share-url-dialog/share-url-dialog.component';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { AuthService } from '@app/core/services/auth.service';
import { fuseAnimations } from '@fuse/animations';
import * as moment from 'moment';
import { merge, Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { DailyWoReportByOperationsManagerService } from '../wo-daily-report-operations-manager.service';

@Component({
  selector: 'app-wo-daily-report-om-document',
  templateUrl: './wo-daily-report-om-document.component.html',
  styleUrls: ['./wo-daily-report-om-document.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class WoDailyReportOmDocumentComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  dialogRef: any;
  loading$ = this.woDailyReportService.loadingSubject.asObservable();
  get workOrdersCount(): any { return this.woDailyReportService.elementsCount; }
  searchInput: FormControl;

  selectedOperationsManager: any = {};
  selectedOMSubscription: Subscription;

  dateFrom: any;
  dateTo: any;
  selectedDatesSubscription: Subscription;

  loggedUser: any;
  dataSource: DailyWoReportDataSource | null;
  displayedColumns = ['number', 'locationName', 'description', 'dateSubmitted', 'dueDate', 'statusName', 'hasImages', 'buttons'];

  constructor(
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private datePipe: DatePipe,
    private authService: AuthService,
    private woDailyReportService: DailyWoReportByOperationsManagerService,
    private epochPipe: FromEpochPipe,
  ) {
    this.searchInput = new FormControl(this.woDailyReportService.searchText);

    this.selectedOperationsManager = this.woDailyReportService.selectedOperationsManager;
    this.selectedOMSubscription =
      this.woDailyReportService.onSelectedOperationsManagerChange.subscribe((operationsManager: any) => {
        this.selectedOperationsManager = operationsManager;
      });

    this.dateFrom = this.woDailyReportService.dateFrom;
    this.dateTo = this.woDailyReportService.dateTo;

    this.selectedDatesSubscription =
      this.woDailyReportService.onDatesChanges.subscribe((dates: any) => {
        this.dateFrom = dates.dateFrom;
        this.dateTo = dates.dateTo;
      });

  }

  get urlToCopy(): string {
    return  window.location.protocol + '//' + window.location.host + '/work-orders/daily-report/' +
      this.loggedUser.employeeGuid + '/' +
      this.selectedOperationsManager.id + '/' +
      this.selectedOperationsManager.guid + '/' +
      moment(this.dateFrom).format('YYYY-MM-DD') + '/' + moment(this.dateTo).format('YYYY-MM-DD');
  }

  ngOnInit(): void {
    this.loggedUser = this.authService.currentUser;

    this.searchInput.valueChanges
    .pipe(
      debounceTime(300),
      distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.woDailyReportService.searchTextChanged.next(searchText);
      });
    this.dataSource = new DailyWoReportDataSource(this.woDailyReportService);
  }

  ngAfterViewInit(): void {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.woDailyReportService.getElements(
          'ReadDailyReportByOperationsManager', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize, {}))
      )
      .subscribe();
  }

  shareDocument(): void {
    this.dialogRef = this.dialog.open(ShareUrlDialogComponent, {
      panelClass: 'share-url-form-dialog',
      data: {
        urlToCopy: this.urlToCopy
      }
    });
  }

  openNewTap(): void {
    window.open(this.urlToCopy, '_blank');
  }

  getValidDateFromUTCToLocal(dateToValidate: any, epochDate: any): any {
    const possibleDate: any = new Date(dateToValidate);
    const dateToCompare = new Date('2000-01-01');

    if (possibleDate < dateToCompare) {
      return '-';
    }
    else {
      return new Date(this.epochPipe.transform(epochDate));
    }
  }

  openNewTapPublicWorkOrder(workOrder): void {
    const urlPublicWorkOrder =  window.location.protocol + '//' + window.location.host + '/work-orders/' + workOrder.guid;
    window.open(urlPublicWorkOrder, '_blank');
  }

  // Show dueDate if it has a valid value, valid value is a date different of null or default value
  showDueDate(value: any): any {
    const f2 = new Date(value);
    const f1 = new Date('2000-01-01');
    return (f2 < f1) ? '-' : this.datePipe.transform(f2, 'MMMM dd');
  }

  ngClassWorkOrderStatus(statusId: any): any {
    return WORK_ORDER_STATUS.find(item => item.key === statusId).value;
  }

  workOrderStatus(statusId: any): string {
    return (WORK_ORDER_STATUS.find(item => item.key === statusId).value).replace(/-/g, ' ');
  }

  hasImages(attachmentsCount): any {
    if (attachmentsCount > 0) { return 'yes'; } else { return 'no'; }
  }

  isEmpty(obj): any {
    return (obj && (Object.keys(obj).length === 0));
  }

  ngOnDestroy(): void {
    this.selectedOMSubscription.unsubscribe();
  }

}
export class DailyWoReportDataSource extends DataSource<any>
{
  constructor(private woDailyReportService: DailyWoReportByOperationsManagerService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.woDailyReportService.allElementsChanged;
  }

  disconnect(): void {
  }
}

