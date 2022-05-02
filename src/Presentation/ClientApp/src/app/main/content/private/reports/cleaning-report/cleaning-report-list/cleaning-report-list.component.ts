import { DataSource } from '@angular/cdk/table';
import { AfterViewInit, Component, Input, OnDestroy, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { CleaningReportCreateModel } from '@app/core/models/reports/cleaning-report/cleaning.report.create.model';
import { CleaningReportGridModel } from '@app/core/models/reports/cleaning-report/cleaning.report.grid.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { merge, Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { CleaningReportFormComponent } from '../cleaning-report-form/cleaning-report-form.component';
import { CleaningReportService } from '../cleaning-report.service';
import * as moment from 'moment';

@Component({
  selector: 'app-cleaning-report-list',
  templateUrl: './cleaning-report-list.component.html',
  styleUrls: ['./cleaning-report-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class CleaningReportListComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @Input() readOnly: boolean;

  reports: CleaningReportGridModel[];
  report: CleaningReportGridModel;

  dataSource: CleaningReportsDataSource | any;
  displayedColumns = ['number', 'dateOfService', 'companyName', 'location', 'preparedFor', 'to', 'cleaningItems', 'findingItems', 'status', 'buttons'];

  allReportsChangedSubscription: Subscription;
  onReportDataChangedSubscription: Subscription;

  dialogRef: any;
  cleaningReportForm: FormGroup;
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  searchInput: FormControl;

  onlyPendingReply = false;
  get totalPendingReply(): number { return this.cleaningReportService.pendingToReplyCount; }
  currentFilters: { [key: string]: any };

  get reportsCount(): number { return this.cleaningReportService.elementsCount; }

  constructor(
    private cleaningReportService: CleaningReportService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {
    this.searchInput = new FormControl(this.cleaningReportService.searchText);

    this.allReportsChangedSubscription =
      this.cleaningReportService.allElementsChanged.subscribe(data => {
        this.reports = data;
      });

    this.onReportDataChangedSubscription =
      this.cleaningReportService.elementChanged.subscribe(data => {
        this.report = data;
      });

    this.currentFilters = this.cleaningReportService.filterBy;
    this.cleaningReportService.onFilterChanged.subscribe((value) => {
      this.currentFilters = value;
    });
  }

  ngOnInit(): void {
    this.searchInput.valueChanges
    .pipe(
      debounceTime(300),
      distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.cleaningReportService.searchTextChanged.next(searchText);
      });
    this.dataSource = new CleaningReportsDataSource(this.cleaningReportService);
  }

  ngAfterViewInit(): void {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => {
      this.paginator.pageIndex = 0;
    }
    );

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.cleaningReportService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize))
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.allReportsChangedSubscription.unsubscribe();
    this.onReportDataChangedSubscription.unsubscribe();
  }

  editElement(report): void {
    this.dialogRef = this.dialog.open(CleaningReportFormComponent, {
      panelClass: 'cleaning-report-form-dialog',
      data: {
        action: 'edit',
        report: report
      }
    });

    this.dialogRef.afterClosed().subscribe((response: FormGroup) => {
      if (!response) {
        return;
      }

      const actionType: string = response[0];
      const formData: FormGroup = response[1];
      const currentReport = new CleaningReportCreateModel(formData.getRawValue());

      if (actionType === 'save') {
        this.cleaningReportService.updateElement(currentReport)
          .then(
            () => this.snackBar.open('Cleaning Report updated successfully!!!', 'close', { duration: 1000 }),
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 })
          ).catch(
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 })
          );
      }
      else if (actionType === 'delete') {
        this.removeElement(currentReport);
      }
    });
  }

  removeElement(report): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.cleaningReportService.delete(report)
          .then(
            () => this.snackBar.open('Cleaning Report deleted successfully!!!', 'close', { duration: 1000 }),
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 })
          ).catch(
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 })
          );
      }
      this.confirmDialogRef = null;
    });
  }

  quantityIndicatorClass(lastCommentDirection: number): string {
    if (lastCommentDirection < 0) {
      return 'yellow-600';
    }
    else if (lastCommentDirection > 0) {
      return 'green-600';
    }

    return '';
  }

  filterOnlyPedingReply(): void {

    // Resets dates firstly
    const dateFrom = moment(moment().add(-10, 'y').toDate()).format('YYYY-MM-DD');
    const dateTo = moment(moment().add().toDate()).format('YYYY-MM-DD');

    this.currentFilters['dateFrom'] = moment(dateFrom).format('YYYY-MM-DD');
    this.currentFilters['dateTo'] = moment(dateTo).format('YYYY-MM-DD');
    this.cleaningReportService.onDatesChanges.next({ 'dateFrom': dateFrom, 'dateTo': dateTo });

    // Sets comment direction
    this.currentFilters['commentDirection'] = this.onlyPendingReply ? 'null' : '-1';

    // Toggle only-pending flag
    this.onlyPendingReply = !this.onlyPendingReply;
    this.cleaningReportService.onFilterChanged.next(this.currentFilters);

  }


}

export class CleaningReportsDataSource extends DataSource<any>
{
  constructor(private cleaningReportService: CleaningReportService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.cleaningReportService.allElementsChanged;
  }

  disconnect(): void {
  }
}
