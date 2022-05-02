import { DataSource } from '@angular/cdk/table';
import { DatePipe } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild, ViewEncapsulation, OnDestroy, AfterViewInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { WorkOrderSourceCode } from '@app/core/models/work-order/work-order-base.model'; // MG-23
import { WorkOrderScheduleSetting } from '@app/core/models/work-order/work-order-schedule-setting.model';
import { WORK_ORDER_STATUS } from '@app/core/models/work-order/work-order-status.model';
import { WorkOrderUpdateModel } from '@app/core/models/work-order/work-order-update.model';
import { WoActivityLogDialogComponent } from '@app/core/modules/work-order-dialog/wo-activity-log-dialog/wo-activity-log-dialog.component';
import { WorkOrderSequencesDialogComponent } from '@app/core/modules/work-order-dialog/work-order-sequences-dialog/work-order-sequences-dialog.component';
import { WorkOrderSharedFormComponent } from '@app/core/modules/work-order-form/work-order-form/work-order-form.component';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { AuthService } from '@app/core/services/auth.service';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { BehaviorSubject, merge, Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { WorkOrderListModel } from '../../../../../core/models/work-order/work-order-list.model';
import { WorkOrdersService } from '../work-orders.service';

@Component({
  selector: 'app-work-order-list',
  templateUrl: './work-order-list.component.html',
  styleUrls: ['./work-order-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class WorkOrderListComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  // Level Role
  roleLevelLoggedUser: number;

  get workOrdersCount(): any { return this.workOrderService.elementsCount; }
  workOrders: WorkOrderListModel[];
  workOrder: WorkOrderListModel;
  dataSource: WorkOrdersDataSource | null;
  displayedColumns = ['number', 'locationName', 'description', 'requesterFullName', /* 'assignedEmployeeFullName',*/
    'dateSubmitted', 'dueDate', 'statusName', 'buttons'];
  selectedWorkOrders: any[];
  checkboxes: {};
  get workOrderCount(): any { return this.workOrderService.elementsCount; }

  onWorkOrdersChangedSubscription: Subscription;
  onselectedWorkOrdersChangedSubscription: Subscription;
  onWorkOrderDataChangedSubscription: Subscription;

  loading$ = this.workOrderService.loadingSubject.asObservable();
  loadingList$ = new BehaviorSubject<boolean>(false);

  dialogRef: any;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  searchInput: FormControl;

  @Input() readOnly: boolean;

  private today = new Date();

  workOrderSequencesDialog: MatDialogRef<WorkOrderSequencesDialogComponent>;

  constructor(
    private workOrderService: WorkOrdersService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private datePipe: DatePipe,
    private epochPipe: FromEpochPipe,
    private authService: AuthService
  ) {
    this.searchInput = new FormControl(this.workOrderService.searchText);

    this.onWorkOrdersChangedSubscription =
      this.workOrderService.allElementsChanged.subscribe(workOrders => {
        this.workOrders = workOrders;
      });

    this.onselectedWorkOrdersChangedSubscription =
      this.workOrderService.selectedElementsChanged.subscribe(selectedWorkOrders => {
        for (const guid in this.checkboxes) {
          if (!this.checkboxes.hasOwnProperty(guid)) {
            continue;
          }

          this.checkboxes[guid] = selectedWorkOrders.includes(guid);
        }
        this.selectedWorkOrders = selectedWorkOrders;
      });

    this.onWorkOrderDataChangedSubscription =
      this.workOrderService.elementChanged.subscribe(workOrder => {
        this.workOrder = workOrder;
      });

  }

  ngOnInit(): void {
    this.roleLevelLoggedUser = this.authService.currentUser.roleLevel;
    this.searchInput.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.workOrderService.searchTextChanged.next(searchText);
      });
    this.dataSource = new WorkOrdersDataSource(this.workOrderService);
  }

  ngAfterViewInit(): void {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.workOrderService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize))
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.onWorkOrdersChangedSubscription.unsubscribe();
    this.onselectedWorkOrdersChangedSubscription.unsubscribe();
    this.onWorkOrderDataChangedSubscription.unsubscribe();
  }

  editWorkOrder(workOrder): void {
    if (this.roleLevelLoggedUser <= 20) {
      this.workOrderService.get(workOrder.id, 'update')
        .subscribe((workOrderData: any) => {
          if (workOrderData) {
            const workOrderUpdateObj = new WorkOrderUpdateModel(workOrderData);
            this.dialogRef = this.dialog.open(WorkOrderSharedFormComponent, {
              panelClass: 'work-order-form-dialog',
              data: {
                workOrder: workOrderUpdateObj,
                action: 'edit'
              }
            });

            this.dialogRef.afterClosed()
              .subscribe(response => {
                if (!response) {
                  return;
                }
                const actionType: string = response[0];
                const formData: FormGroup = response[1];
                const updatedWorkOrderObj = new WorkOrderUpdateModel(formData.getRawValue());
                switch (actionType) {
                  /**
                   * Save
                   */
                  case 'save':
                    this.workOrderService.updateElement(updatedWorkOrderObj)
                      .then(
                        () => this.snackBar.open('Work Order updated successfully!!!', 'close', { duration: 1000 }),
                        () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
                      .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

                    break;
                  /**
                   * Delete
                   */
                  case 'delete':

                    this.deleteWorkOrder(workOrderUpdateObj);

                    break;

                  /*
                  * Close,is necessary check if work order status changed for refresh the list of work orders
                  */
                  case 'close':

                    if (workOrder.statusId !== workOrderUpdateObj.statusId) {
                      this.workOrderService.getElements();
                    }

                    break;
                }
              });
          } else {
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          }
        },
          (error) => {
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
    }
  }

  cloneWorkOrder(workOrder): void {
    this.workOrderService.getWorkOrderForCloning(workOrder.id)
      .subscribe((workOrderData: any) => {
        if (workOrderData) {
          const workOrderUpdateObj = new WorkOrderUpdateModel(workOrderData);

          const initDate = new Date(0);
          const todayDate = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate(), 18, 0, 0);
          const miliseconds = todayDate.getTime() - initDate.getTime();

          workOrderUpdateObj.dueDate = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate(), 18, 0, 0);
          workOrderUpdateObj.epochDueDate = (miliseconds / 1000);
          workOrderUpdateObj.statusId = 1;
          workOrderUpdateObj.sendRequesterNotifications = false;
          workOrderUpdateObj.sendPropertyManagersNotifications = false;
          workOrderUpdateObj.notes = [];

          workOrderUpdateObj.scheduleDate = null;
          workOrderUpdateObj.snoozeDate = null;

          this.dialogRef = this.dialog.open(WorkOrderSharedFormComponent, {
            panelClass: 'work-order-form-dialog',
            data: {
              workOrder: workOrderUpdateObj,
              action: 'new',
              workOrderFromClone: true,
              workOrderSourceCode: WorkOrderSourceCode.Cloned // MG-23
            }
          });

          this.dialogRef.afterClosed()
            .subscribe(response => {
              if (!response) {
                return;
              }

              if (response[0] === 'close') {
                return;
              }

              const wo = response.getRawValue();
              const setStatusByStandBy: boolean = response.get('setStatusByStandBy').value ? response.get('setStatusByStandBy').value : false;
              const scheduleSettings = wo.scheduleSettings ? new WorkOrderScheduleSetting(wo.scheduleSettings) : null;
              if (scheduleSettings) {
                this.workOrderService.loadingSubject.next(true);
                this.workOrderService.create(scheduleSettings, 'AddScheduleSettings')
                  .subscribe((settingsResult: any) => {
                    const settingsId = settingsResult['body'].id;
                    response.controls['workOrderScheduleSettingId'].setValue(
                      settingsId
                    );
                    let dates: Date[] = [];
                    dates = this.workOrderService.calculateScheduleDates(scheduleSettings);

                    if (dates.length > 0) {
                      const workOrders: any[] = [];
                      let index = 0;
                      dates.forEach(d => {
                        let unscheduled = false;
                        if (!scheduleSettings.scheduleDate && scheduleSettings.frequency > 2 && scheduleSettings.frequency < 6) {
                          unscheduled = true;
                        } else if (index > 0 && scheduleSettings.frequency > 2 && scheduleSettings.frequency < 6) {
                          unscheduled = true;
                        }

                        response.controls['dueDate'].setValue(setStatusByStandBy ? d : null);
                        response.controls['statusId'].setValue(setStatusByStandBy ? 1 : 0);
                        response.controls['scheduleDate'].setValue(d);
                        response.controls['unscheduled'].setValue(setStatusByStandBy ? false : unscheduled);
                        workOrders.push(response.getRawValue());
                        index++;
                      });
                      this.saveWorkOrders(workOrders, settingsId);
                    } else {
                      this.workOrderService.loadingSubject.next(false);
                    }
                  });
              } else {
                this.workOrderService.createElement(response.getRawValue())
                  .then(
                    () => this.snackBar.open('Work Order created successfully!!!', 'close', { duration: 1000 }),
                    () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
                  .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
              }
            });
        } else {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      },
        (error) => {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });
  }

  openNewTapPublicWorkOrder(workOrder): void {
    const urlPublicWorkOrder = window.location.protocol + '//' + window.location.host + '/work-orders/' + workOrder.guid;
    window.open(urlPublicWorkOrder, '_blank');
  }

  showActivity(workOrder: any): void {
    this.dialogRef = this.dialog.open(WoActivityLogDialogComponent, {
      panelClass: 'work-order-activity-log-dialog',
      data: {
        id: workOrder.id,
        number: workOrder.originWorkOrderId ? workOrder.clonePath : workOrder.number
      }
    });
  }

  /**
   * Delete WorkOrder
   */
  deleteWorkOrder(workOrder): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadingList$.next(true);
        this.workOrderService.deleteWorkOrder(workOrder, 'deleteWO')
          .then(
            () => {
              this.loadingList$.next(false);
              this.snackBar.open('Work Order deleted successfully!!!', 'close', { duration: 1000 });
            },
            (error) => {
              this.loadingList$.next(false);
              this.snackBar.open(error, 'close', { duration: 1000 });
            }
          ).catch(
            (error) => {
              this.loadingList$.next(false);
              this.snackBar.open(error, 'close', { duration: 1000 });
            }
          );
      }
      this.confirmDialogRef = null;
    });

  }

  onSelectedChange(workOrderGuid): void {
    this.workOrderService.toggleSelectedElement(workOrderGuid);
  }

  // Show dueDate if it has a valid value, valid value is a date different of null or default value
  showDueDate(value: any): any {
    const f2 = new Date(value);
    const f1 = new Date('2000-01-01');
    return (f2 < f1) ? '-' : this.datePipe.transform(f2, 'MMMM dd');
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

  ngClassWorkOrderStatus(statusId: any, isExpired: any): string {
    return WORK_ORDER_STATUS.find(item => item.key === statusId).value + (isExpired === 1 ? ' overdue' : '');
  }

  workOrderStatus(statusId: any): string {
    return (WORK_ORDER_STATUS.find(item => item.key === statusId).value).replace(/-/g, ' ');
  }

  // Schedule
  calculateScheduleDates(settings: WorkOrderScheduleSetting): Date[] {
    let dates: Date[] = [];
    // to prevent duplicates
    settings.days.sort();

    switch (settings.frequency) {
      case 0:
        let isValid = true;
        if (!settings.endDate) {
          dates.push(settings.startDate);
          isValid = false;
        }
        if (isValid) {
          const currentDailyDate = new Date(
            settings.startDate.getFullYear(),
            settings.startDate.getMonth(),
            settings.startDate.getDate()
          );
          while (currentDailyDate <= settings.endDate) {
            dates.push(
              new Date(
                currentDailyDate.getFullYear(),
                currentDailyDate.getMonth(),
                currentDailyDate.getDate()
              )
            );
            currentDailyDate.setDate(currentDailyDate.getDate() + 1);
          }
        }
        break;
      case 1:
        // Parse days to string
        const strDays: string[] = [];
        settings.days.forEach(d => {
          strDays.push(`${d}`);
        });
        const currentDate = new Date(
          settings.startDate.getFullYear(),
          settings.startDate.getMonth(),
          settings.startDate.getDate()
        );
        while (currentDate <= settings.endDate) {
          const dayWeek = currentDate.getDay();
          const match = strDays.find(d => d === dayWeek.toString());
          if (match) {
            dates.push(
              new Date(
                currentDate.getFullYear(),
                currentDate.getMonth(),
                currentDate.getDate()
              )
            );
          }
          currentDate.setDate(currentDate.getDate() + 1);
        }
        break;
      case 2:
        dates = this.calculateMonthlyDates(
          settings.ordinal,
          settings.startValue,
          settings.endValue,
          settings.days
        );
        break;
      case 3:
        dates = this.calculateQuarterlyScheduleDates(
          settings.frequency,
          settings.startDate,
          settings.endDate,
          settings.excludedScheduleDates
        );
        if (settings.scheduleDate) {
          if (dates.length > 0) {
            dates[0] = settings.scheduleDate;
          }
        }
        break;
      case 4:
        dates = this.calculateQuarterlyScheduleDates(
          settings.frequency,
          settings.startDate,
          settings.endDate,
          settings.excludedScheduleDates
        );
        if (settings.scheduleDate) {
          if (dates.length > 0) {
            dates[0] = settings.scheduleDate;
          }
        }
        break;
      case 5:
        dates = this.calculateQuarterlyScheduleDates(
          settings.frequency,
          settings.startDate,
          settings.endDate,
          settings.excludedScheduleDates
        );
        if (settings.scheduleDate) {
          if (dates.length > 0) {
            dates[0] = settings.scheduleDate;
          }
        }
        break;
      case 6:
        if (settings.scheduleDate) {
          dates.push(settings.scheduleDate);
        }
        break;
    }
    return dates;
  }

  private calculateMonthlyDates(
    ordinal: number,
    startMonth: number,
    endMonth: number,
    days: number[]
  ): Date[] {
    startMonth = startMonth - 1;
    endMonth = endMonth - 1;

    // Parse days to string
    const strDays: string[] = [];
    days.forEach(d => {
      strDays.push(`${d}`);
    });

    const dates: Date[] = [];
    const today = new Date();
    const startDate = new Date(today.getFullYear(), startMonth, 1);
    const endDate = new Date(
      endMonth < startMonth ? today.getFullYear() + 1 : today.getFullYear(),
      endMonth,
      1
    );
    const currentDate = startDate;

    while (currentDate <= endDate) {
      const monthDay = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate());
      const monthEnd = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);

      let monthMatches = 0;
      if (ordinal < 4) {
        while (monthMatches < ordinal) {
          const dayWeek = monthDay.getDay();
          const match = strDays.find(d => d === dayWeek.toString());
          if (match) {
            monthMatches++;
            if (monthMatches === ordinal) {
              const newDate = new Date(monthDay.getFullYear(), monthDay.getMonth(), monthDay.getDate());
              dates.push(newDate);
            }
          }
          monthDay.setDate(monthDay.getDate() + 1);
        }
      } else if (ordinal === 4) {
        while (monthMatches < 1) {
          const dayWeek = monthEnd.getDay();
          const match = strDays.find(d => d === dayWeek.toString());
          if (match) {
            const newDate = new Date(monthEnd.getFullYear(), monthEnd.getMonth(), monthEnd.getDate());
            dates.push(newDate);
            monthMatches++;
          }
          monthEnd.setDate(monthEnd.getDate() - 1);
        }
      }

      currentDate.setMonth(currentDate.getMonth() + 1);
    }

    return dates;
  }

  private getMonthWeekRange(
    ordinal: number,
    month: number,
    year: number
  ): Date[] {
    const range: Date[] = [];
    const startWeek = new Date(year, month, 1);
    const endWeek = new Date(year, month, 1);
    // set date to the las day of week
    endWeek.setDate(endWeek.getDate() + (7 - (endWeek.getDay() + 1)));

    switch (ordinal) {
      case 1:
        range[0] = startWeek;
        range[1] = endWeek;
        break;
      case 2:
        // go to the second week
        const secondStartWeek = new Date(
          endWeek.getFullYear(),
          endWeek.getMonth(),
          endWeek.getDate() + 1
        );
        const secondEndWeek = new Date(
          endWeek.getFullYear(),
          endWeek.getMonth(),
          endWeek.getDate() + 1
        );
        secondEndWeek.setDate(
          secondEndWeek.getDate() + (7 - (secondEndWeek.getDay() + 1))
        );
        range[0] = secondStartWeek;
        range[1] = secondEndWeek;
        break;
      case 3:
        // go to the third week
        const thirdStartWeek = new Date(
          endWeek.getFullYear(),
          endWeek.getMonth(),
          endWeek.getDate() + 8
        );
        const thirdEndWeek = new Date(
          endWeek.getFullYear(),
          endWeek.getMonth(),
          endWeek.getDate() + 8
        );
        thirdEndWeek.setDate(
          thirdEndWeek.getDate() + (7 - (thirdEndWeek.getDay() + 1))
        );
        range[0] = thirdStartWeek;
        range[1] = thirdEndWeek;
        break;
      case 4:
        const lastEndWeek = new Date(year, month + 1, 0);
        const lastStartWeek = new Date(year, month + 1, 0);
        lastStartWeek.setDate(
          lastStartWeek.getDate() - (7 - (7 - lastStartWeek.getDay()))
        );
        range[0] = lastStartWeek;
        range[1] = lastEndWeek;
        break;
    }
    return range;
  }

  private calculateQuarterlyScheduleDates(
    selectedScheduleFrequency: number,
    startDate: Date,
    endDate: Date,
    excluded: Date[]
  ): Date[] {
    const dates: Date[] = [];

    let period = 12;

    if (selectedScheduleFrequency === 3) {
      // quarterly
      period = 3;
    } else if (selectedScheduleFrequency === 4) {
      // semi anually
      period = 6;
    }

    try {
      const curentMonth = new Date(
        startDate.getFullYear(),
        startDate.getMonth(),
        1
      );
      while (curentMonth <= endDate) {
        const match = excluded.find(
          d =>
            d.getFullYear() === curentMonth.getFullYear() &&
            d.getMonth() === curentMonth.getMonth()
        );
        if (!match) {
          dates.push(
            new Date(curentMonth.getFullYear(), curentMonth.getMonth(), 1)
          );
        }
        curentMonth.setMonth(curentMonth.getMonth() + period);
      }
    } catch (error) {
      console.log(error);
    }
    return dates;
  }

  async saveWorkOrders(workOrders: any[], scheduleSettingId): Promise<void> {
    for (let index = 0; index < workOrders.length; index++) {
      try {
        const result = await this.workOrderService
          .createFromCalendar(workOrders[index])
          .toPromise();
      } catch (error) {
        console.log(error);
      }
    }

    this.workOrderService.loadingSubject.next(false);
    this.displayWorkOrderSequence(scheduleSettingId);
  }

  displayWorkOrderSequence(calendarItemFrequencyId: number): void {
    this.workOrderSequencesDialog = this.dialog.open(WorkOrderSequencesDialogComponent, {
      panelClass: 'work-order-sequences-dialog',
      data: {
        calendarItemFrequencyId: calendarItemFrequencyId
      }
    });
    this.workOrderSequencesDialog.afterClosed().subscribe(() => {
      this.workOrderService.getElements();
      this.workOrderSequencesDialog = null;
    });
  }

  viewTicket(ticketId: number): void {
    const url = window.location.protocol + '//' + window.location.host + '/app/inbox/ticket-detail/' + ticketId;
    window.open(url, '_blank');
  }

}

export class WorkOrdersDataSource extends DataSource<any>
{
  constructor(private workOrderService: WorkOrdersService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.workOrderService.allElementsChanged;
  }

  disconnect(): void {
  }
}
