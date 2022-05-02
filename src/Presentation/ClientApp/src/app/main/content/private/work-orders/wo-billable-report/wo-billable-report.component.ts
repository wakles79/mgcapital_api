import { DataSource } from '@angular/cdk/table';
import { DatePipe } from '@angular/common';
import { AfterViewInit, Component, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { WorkOrderTaskAttachmentModel } from '@app/core/models/work-order-task/work-order-task-attachment.model';
import { WorkOrderTaskModel } from '@app/core/models/work-order/work-order-task.model';
import { WoTaskBillingFormComponent } from '@app/core/modules/work-order-form/wo-task-billing-form/wo-task-billing-form.component';
import { WorkOrderTaskFormComponent } from '@app/core/modules/work-order-form/work-order-task-form/work-order-task-form.component';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { merge, Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { WoBillableReportService } from './wo-billable-report.service';
import * as moment from 'moment';
import { WorkOrderTaskUpdateModel } from '@app/core/models/work-order-task/work-order-task-update.model';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';

@Component({
  selector: 'app-wo-billable-report',
  templateUrl: './wo-billable-report.component.html',
  styleUrls: ['./wo-billable-report.component.scss']
})
export class WoBillableReportComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  dialogRef: any;
  loading$ = this.woBillableReportService.loadingSubject.asObservable();
  get workOrderTaskCount(): any { return this.woBillableReportService.elementsCount; }
  searchInput: FormControl;

  dateFrom: any;
  dateTo: any;
  selectedDatesSubscription: Subscription;

  dataSource: WorkOrderBillableReportDataService | null;
  displayedColumns = [
    'number', 'buildingName', 'buildingBillingInfo',
    'buildingNoteToBilling', 'taskName', 'workOrderRequestedDate', /* 'taskRequestedDate',*/
    'workOrderCompletedDate', 'taskNote', 'billingNote', 'closingNotes', 'serviceName', /* 'servicePrice', 'serviceQuantity', */
    'serviceTotal', 'buttons'];

  billableReportCsv: any;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  workOrderTaskForm: MatDialogRef<WorkOrderTaskFormComponent>;

  constructor(
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private datePipe: DatePipe,
    private epochPipe: FromEpochPipe,
    private woBillableReportService: WoBillableReportService,
    private _fuseSidebarService: FuseSidebarService
  ) {
    this.searchInput = new FormControl(this.woBillableReportService.searchText);

    this.dateFrom = this.woBillableReportService.dateFrom;
    this.dateTo = this.woBillableReportService.dateTo;

    this.selectedDatesSubscription =
      this.woBillableReportService.onDatesChanges.subscribe((dates: any) => {
        this.dateFrom = dates.dateFrom;
        this.dateTo = dates.dateTo;
      });
  }

  ngOnInit(): void {
    this.searchInput.valueChanges
    .pipe(
      debounceTime(300),
      distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.woBillableReportService.searchTextChanged.next(searchText);
      });
    this.dataSource = new WorkOrderBillableReportDataService(this.woBillableReportService);
  }

  ngAfterViewInit(): void {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.woBillableReportService.getElements(
          'ReadBillingReport', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize, {}))
      )
      .subscribe();
  }

  openNewTapPublicworkOrder(workOrderTask): void {
    const urlPublicWorkOrder = window.location.protocol + '//' + window.location.host + '/work-orders/' + workOrderTask.workOrderGuid;
    window.open(urlPublicWorkOrder, '_blank');
  }

  // Return a date if it has a valid value and if the task is checked
  // Valid value is a date different of null or default value
  getValidCompletedDate(workOrderTask: any): any {

    if (!workOrderTask.isTaskChecked) {
      return '-';
    }
    const completedDate = workOrderTask.completedDate;

    let possibleDate: any = new Date(completedDate);
    const dateToCompare = new Date('2000-01-01');

    if (possibleDate < dateToCompare) {
      return '-';
    }
    else {
      possibleDate = this.epochPipe.transform(workOrderTask.epochTaskCompletedDate);
      return possibleDate; // this.datePipe.transform(possibleDate, 'MMMM dd');
    }
  }

  editTaskBillingInformation(workOrderTask): void {

    this.woBillableReportService.get(workOrderTask.id, 'updateTask')
      .subscribe((taskData: any) => {
        if (taskData) {
          const taskUpdateObj = new WorkOrderTaskModel(taskData);

          this.dialogRef = this.dialog.open(WoTaskBillingFormComponent, {
            panelClass: 'task-billing-form-dialog',
            data: {
              task: taskUpdateObj,
              action: 'edit'
            }
          });

          this.dialogRef.afterClosed()
            .subscribe(formData => {
              if (!formData) {
                return;
              }

              const updatedTaskObj = new WorkOrderTaskModel(formData.getRawValue());
              updatedTaskObj.id = workOrderTask.id;
              updatedTaskObj.workOrderId = workOrderTask.workOrderId;
              this.woBillableReportService.updateElement(updatedTaskObj, 'updateTask')
                .then(
                  () => this.snackBar.open('Work Order Task updated successfully!!!', 'close', { duration: 1000 }),
                  () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
                .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

            });
        } else {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      },
        (error) => {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });

  }

  exportReportToCsv(): void {
    this.woBillableReportService.exportReportToCsv()
      .then(
        (csvFile) => {
          this.downloadFile(csvFile);
        },
        (error) => {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      );
  }

  downloadFile(data: any): void {
    const csvData = data;
    const a = document.createElement('a');
    a.setAttribute('style', 'display:none;');
    document.body.appendChild(a);
    const blob = new Blob([csvData.body], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    a.href = url;
    a.download = 'BillableReport' + '_' + this.datesBillableReportString + '.csv';
    a.click();
  }

  exportReportToPDF(): void {
    this.woBillableReportService.loadingSubject.next(true);
    this.woBillableReportService.getDocumentReportUrl().subscribe((response: string) => {
      this.woBillableReportService.loadingSubject.next(false);
      window.open(response, '_blank');
    },
      (error) => {
        this.woBillableReportService.loadingSubject.next(false);
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

  get datesBillableReportString(): string {
    return moment(this.dateFrom).format('YYYY-MM-DD') + '_' + moment(this.dateTo).format('YYYY-MM-DD');
  }

  ngOnDestroy(): void {
  }

  editWorkOrderTask(id: number): void {
    this.woBillableReportService.loadingSubject.next(true);
    this.woBillableReportService.getTask(id)
      .subscribe((data) => {
        this.woBillableReportService.loadingSubject.next(false);

        if (!data) {
          this.snackBar.open('Error getting task info', 'close', { duration: 3000 });
          return;
        }

        const taskUpdate = new WorkOrderTaskUpdateModel(data);

        this.workOrderTaskForm = this.dialog.open(WorkOrderTaskFormComponent, {
          panelClass: 'wo-task-form-dialog',
          data: {
            action: 'edit',
            task: taskUpdate,
            newWo: false,
            showDates: true
          }
        });

        this.workOrderTaskForm.afterClosed()
          .subscribe((dialogResult: { form: FormGroup, files: any[] }) => {
            if (!dialogResult) {
              return;
            }

            this.verifyUpdateTask(dialogResult.form.getRawValue(), dialogResult.files);
            // this.woBillableReportService.loadingSubject.next(true);
            // this.woBillableReportService.update(dialogResult.form.getRawValue(), 'UpdateTask')
            //   .subscribe(() => {
            //     this.woBillableReportService.loadingSubject.next(false);
            //     this.snackBar.open('Work Order task updated successfully', 'close', { duration: 3000 });
            //     this.woBillableReportService.getElements();
            //   }, (error) => {
            //     this.woBillableReportService.loadingSubject.next(false);
            //     this.snackBar.open('Error updating task', 'close', { duration: 3000 });
            //   });

          });
      }, (error) => {
        this.woBillableReportService.loadingSubject.next(false);
        this.snackBar.open('Error getting task info', 'close', { duration: 3000 });
      });
  }

  verifyUpdateTask(task, files: { description: string, fileName: string, file: File }[]): void {
    this.woBillableReportService.loadingSubject.next(true);
    if (files.length > 0) {
      const uploadFiles: File[] = [];
      files.forEach(f => {
        uploadFiles.push(f.file);
      });

      this.woBillableReportService.uploadFile(uploadFiles)
        .subscribe((response: any) => {
          if (response.status === 200 || response.status === 206) {
            for (let i = 0; i < response.body.length; i++) {
              const fileName = response.body[i].fileName;
              const description = files.find(f => f.fileName === fileName).description;

              const attachemnt = new WorkOrderTaskAttachmentModel({
                id: 0,
                description: description,
                blobName: response.body[i].blobName,
                fullUrl: response.body[i].fullUrl,
                title: fileName,
                imageTakenDate: response.body[i].imageTakenDate,
                workOrderTaskId: 0
              });
              task.attachments.push(attachemnt);
            }

            this.updateTask(task);
          }
        }, (error) => {
          this.woBillableReportService.loadingSubject.next(false);
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });
    } else {
      this.updateTask(task);
    }
  }
  updateTask(task): void {
    this.woBillableReportService.update(task, 'UpdateTask')
      .subscribe(() => {
        this.woBillableReportService.loadingSubject.next(false);
        this.snackBar.open('Work Order task updated successfully', 'close', { duration: 3000 });
        this.woBillableReportService.getElements();
      }, (error) => {
        this.woBillableReportService.loadingSubject.next(false);
        this.snackBar.open('Error updating task', 'close', { duration: 3000 });
      });
  }

  viewTicket(ticketId: number): void {
    const url = window.location.protocol + '//' + window.location.host + '/app/inbox/ticket-detail/' + ticketId;
    window.open(url, '_blank');
  }

    /**
     * Toggle sidebar
     *
     * @param name
     */
    toggleSidebar(name): void
    {
        this._fuseSidebarService.getSidebar(name).toggleOpen();
    }
}

export class WorkOrderBillableReportDataService extends DataSource<any>
{
  constructor(private woBillableReportService: WoBillableReportService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.woBillableReportService.allElementsChanged;
  }

  disconnect(): void {
  }
}
