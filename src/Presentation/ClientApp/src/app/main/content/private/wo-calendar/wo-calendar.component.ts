import { Component, OnInit, ViewEncapsulation, OnDestroy, AfterViewInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { CalendarSequenceSummaryResultComponent } from '@app/core/modules/calendar-sequence-summary-result/calendar-sequence-summary-result/calendar-sequence-summary-result.component';
import { WoActivityLogDialogComponent } from '@app/core/modules/work-order-dialog/wo-activity-log-dialog/wo-activity-log-dialog.component';
import { WorkOrderSequencesDialogComponent } from '@app/core/modules/work-order-dialog/work-order-sequences-dialog/work-order-sequences-dialog.component';
import { WorkOrderSharedFormComponent } from '@app/core/modules/work-order-form/work-order-form/work-order-form.component';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { CalendarEvent } from 'angular-calendar';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { CalendarItemFormComponent } from './calendar-item-form/calendar-item-form.component';
import { WorkOrderCalendarGridModel } from '@app/core/models/work-order/work-order-calendar-grid.model';
import { MatTableDataSource } from '@angular/material/table';
import { WoCalendarService } from './wo-calendar.service';
import { WorkOrdersService } from '../work-orders/work-orders.service';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import { WorkOrderBaseModel, WorkOrderSourceCode } from '@app/core/models/work-order/work-order-base.model';
import { FormGroup } from '@angular/forms';
import { WorkOrderScheduleSetting } from '@app/core/models/work-order/work-order-schedule-setting.model';
import { WorkOrderUpdateModel } from '@app/core/models/work-order/work-order-update.model';
import { WORK_ORDER_STATUS } from '@app/core/models/work-order/work-order-status.model';
import { WORK_ORDER_TYPES } from '@app/core/models/work-order/work-order-type.model';
import { CALENDAR_MONTH } from '@app/core/models/calendar/calendar-periodicity-enum';
import { isSameDay, isSameMonth } from 'date-fns';
import { CalendarItemFrequencyCreateModel } from '@app/core/models/calendar/calendar-item-frequency-create.model';
import { CalendarItemFrequencySummaryModel } from '@app/core/models/calendar/calendar-item-frequency-summary.model';

@Component({
  selector: 'app-wo-calendar',
  templateUrl: './wo-calendar.component.html',
  styleUrls: ['./wo-calendar.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class WoCalendarComponent implements OnInit, OnDestroy, AfterViewInit {

  @ViewChild('woPaginator') paginator: MatPaginator;
  @ViewChild('unscheduledPaginator') unscheduledPaginator: MatPaginator;

  itemFormDialog: MatDialogRef<CalendarItemFormComponent>;

  calendarSequenceSummaryResultDialog: MatDialogRef<
    CalendarSequenceSummaryResultComponent
  >;
  workOrderFormDialog: MatDialogRef<WorkOrderSharedFormComponent>;
  workOrderActivityLogDialog: MatDialogRef<WoActivityLogDialogComponent>;
  workOrderSequencesDialog: MatDialogRef<WorkOrderSequencesDialogComponent>;
  confirmDialog: MatDialogRef<FuseConfirmDialogComponent>;

  loading$ = new BehaviorSubject<boolean>(false);

  viewFilters = false;

  calendarView = 'month';
  today: Date = new Date();
  viewDate: Date = new Date();
  selectedDay: Date = new Date();

  refresh: Subject<any> = new Subject();
  events: CalendarEvent[] = [];

  workOrderSubscription: Subscription;
  loadingList$ = this._calendarService.loadingSubject;
  unscheduledWorkOrders: WorkOrderCalendarGridModel[] = [];
  workOrdersOfMonth: WorkOrderCalendarGridModel[] = [];
  scheduledWorkOrders: WorkOrderCalendarGridModel[] = [];
  workOrdersOfDay: WorkOrderCalendarGridModel[] = [];

  dataSource = new MatTableDataSource<WorkOrderCalendarGridModel>(
    this.workOrdersOfMonth
  );
  displayedColumns: string[] = [
    'number',
    'location',
    'description',
    'category',
    'clientApproved',
    'dateSubmitted',
    'dueDate',
    'status',
    'buttons'
  ];

  unscheduledDataSource = new MatTableDataSource<WorkOrderCalendarGridModel>(
    this.workOrdersOfMonth
  );
  unscheduledDisplayedColumns: string[] = [
    'number',
    'location',
    'description',
    'category',
    'clientApproved',
    'dateSubmitted',
    'dueDate',
    'status',
    'buttons'
  ];

  get workOrderCount(): any {
    return this._calendarService.elementsCount;
  }

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  // Yearly Calendar View
  months: { month: number; name: string; events: number }[] = [];

  // Summary View
  selectedDateSummaryText = 'Events';

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  constructor(
    private _calendarService: WoCalendarService,
    private _workOrderService: WorkOrdersService,
    private epochPipe: FromEpochPipe,
    private _snackBar: MatSnackBar,
    private _dialog: MatDialog,
    private _datePipe: DatePipe
  ) {
    this.viewDate = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), 1);

    this.dataSource = new MatTableDataSource<WorkOrderCalendarGridModel>();
    this.dataSource.paginator = this.paginator;

    this.unscheduledDataSource = new MatTableDataSource<
      WorkOrderCalendarGridModel
    >();
    this.unscheduledDataSource.paginator = this.unscheduledPaginator;

    if (this._calendarService.filterBy['DateFrom']) {
      const date = moment(this._calendarService.filterBy['DateFrom']).toDate();
      this.viewDate = new Date(date.getFullYear(), date.getMonth(), 1);
    }

    this.workOrderSubscription = this._calendarService.allElementsChanged.subscribe(
      (result: WorkOrderCalendarGridModel[]) => {

        this.events = [];
        this.workOrdersOfMonth = result;
        this.scheduledWorkOrders = this.workOrdersOfMonth.filter(
          w => !w.unscheduled && w.scheduleDate
        );

        this.dataSource = new MatTableDataSource<WorkOrderCalendarGridModel>(
          this.scheduledWorkOrders
        );
        this.dataSource.paginator = this.paginator;

        const unscheduledWo = this.workOrdersOfMonth.filter(
          w => w.unscheduled || !w.scheduleDate
        );
        this.unscheduledDataSource = new MatTableDataSource<
          WorkOrderCalendarGridModel
        >(unscheduledWo);
        this.unscheduledDataSource.paginator = this.unscheduledPaginator;
        this.getMonths();

        this.workOrdersOfMonth.forEach(e => {
          if (!e.unscheduled && e.epochScheduleDate > 0) {
            const eventDate: Date =
              e.scheduleDate !== null ? e.scheduleDate : e.dueDate;
            const eventEpochDate: number =
              e.scheduleDate !== null ? e.epochScheduleDate : e.epochDueDate;

            const date = this.convertUTCToLocalTime(eventDate, eventEpochDate);

            e.scheduleDate = date;

            this.events.push({
              title: `#${e.originWorkOrderId ? e.clonePath : e.number}, ${e.description
                }`,
              color: {
                primary: e.colorName,
                secondary: e.colorName
              },
              start: new Date(date),
              draggable: false,
              resizable: {
                beforeStart: true,
                afterEnd: true
              }
            });

            const month = date.getMonth();
            if (month < this.months.length) {
              this.months[month].events++;
            }
          }
        });
      }
    );
  }

  ngOnInit(): void { }

  ngAfterViewInit(): void {
    // merge(this.paginator.page)
    //   .pipe(
    //     tap(() => this._calendarService.getElements(
    //       'readall', '',
    //       '',
    //       '',
    //       this.paginator.pageIndex,
    //       this.paginator.pageSize))
    //   )
    //   .subscribe();
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  // Header Actions
  changeCalendarView(view: string): void {
    this.calendarView = view;
    this.loadEvents();
  }
  lastDate(): void {
    switch (this.calendarView) {
      case 'day':
        this.viewDate.setDate(this.viewDate.getDate() - 1);
        break;
      case 'week':
        this.viewDate.setDate(this.viewDate.getDate() - 7);
        break;
      case 'month':
        this.viewDate.setMonth(this.viewDate.getMonth() - 1);
        break;
      case 'year':
        this.viewDate.setFullYear(this.viewDate.getFullYear() - 1);
        break;
    }

    this.viewDate = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), this.viewDate.getDate());
    this.loadEvents();
  }
  nextDate(): void {
    switch (this.calendarView) {
      case 'day':
        this.viewDate.setDate(this.viewDate.getDate() + 1);
        break;
      case 'week':
        this.viewDate.setDate(this.viewDate.getDate() + 7);
        break;
      case 'month':
        this.viewDate.setMonth(this.viewDate.getMonth() + 1);
        break;
      case 'year':
        this.viewDate.setFullYear(this.viewDate.getFullYear() + 1);
        break;
    }

    this.viewDate = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), this.viewDate.getDate());
    this.loadEvents();
  }
  newItem(): void {
    this.workOrderFormDialog = this._dialog.open(WorkOrderSharedFormComponent, {
      panelClass: 'work-order-form-dialog',
      data: {
        action: 'new',
        source: WorkOrderSourceCode.Other, //EMail MG-23
      }
    });

    this.workOrderFormDialog.afterClosed().subscribe((response: FormGroup) => {
      if (!response) {
        return;
      }

      const wo = response.getRawValue();
      const scheduleSettings = wo.scheduleSettings
        ? new WorkOrderScheduleSetting(wo.scheduleSettings)
        : null;
      if (scheduleSettings) {
        this.loading$.next(true);

        this._workOrderService
          .create(scheduleSettings, 'AddScheduleSettings')
          .subscribe(
            (settingsResult: any) => {
              const settingsId = settingsResult['body'].id;
              response.controls['workOrderScheduleSettingId'].setValue(
                settingsId
              );
              let dates: Date[] = [];
              dates = this.calculateScheduleDates(scheduleSettings);

              if (dates.length > 0) {
                const workOrders: any[] = [];
                let index = 0;

                dates.forEach(d => {
                  let unscheduled = false;
                  if (
                    !scheduleSettings.scheduleDate &&
                    scheduleSettings.frequency > 2 &&
                    scheduleSettings.frequency < 6
                  ) {
                    unscheduled = true;
                  } else if (
                    index > 0 &&
                    scheduleSettings.frequency > 2 &&
                    scheduleSettings.frequency < 6
                  ) {
                    unscheduled = true;
                  }

                  response.controls['dueDate'].setValue(null);
                  response.controls['statusId'].setValue(0);
                  response.controls['scheduleDate'].setValue(d);
                  response.controls['unscheduled'].setValue(unscheduled);
                  workOrders.push(response.getRawValue());
                  index++;
                });
                this.saveWorkOrders(workOrders, settingsId);
              }
            },
            error => {
              this.loading$.next(false);
              this._snackBar.open('Oops, there was an error', 'close', {
                duration: 1000
              });
            }
          );
      } else {
        // Create single work Order
        this._workOrderService
          .createElement(response.getRawValue())
          .then(
            () => {
              this._snackBar.open(
                'Service orders created successfully!!!',
                'close',
                { duration: 1000 }

              );

              this._calendarService.getElements();
            },
            () =>
              this._snackBar.open('Oops, there was an error', 'close', {
                duration: 1000
              })
          )
          .catch(() =>
            this._snackBar.open('Oops, there was an error', 'close', {
              duration: 1000
            })
          );
      }
    });
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
    const endDate = new Date(endMonth < startMonth ? today.getFullYear() + 1 : today.getFullYear(), endMonth, 1);
    const currentDate = new Date(startDate.getFullYear(), startDate.getMonth(), startDate.getDate());

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

  async saveWorkOrders(workOrders: any[], scheduleSettingId): Promise<any> {
    for (let index = 0; index < workOrders.length; index++) {
      try {
        const result = await this._workOrderService
          .createFromCalendar(workOrders[index])
          .toPromise();
      } catch (error) {
        console.log(error);
      }
    }

    this.loading$.next(false);
    this.editWorkOrderSequence(scheduleSettingId);
  }

  async saveCalendarWorkOrders(workOrders: WorkOrderBaseModel[]): Promise<any> {
    const summary: { woNumber: string; success: boolean; date: Date }[] = [];
    for (let index = 0; index < workOrders.length; index++) {
      try {
        const result = await this._workOrderService
          .createFromCalendar(workOrders[index])
          .toPromise();
        // console.log(JSON.stringify(result));
        summary.push({
          woNumber: result.number,
          success: true,
          date: this.convertUTCToLocalTime(
            result.scheduleDate,
            result.epochScheduleDate
          )
        });
      } catch (error) {
        // console.log(JSON.stringify(error));
        summary.push({
          woNumber: '0',
          success: false,
          date: workOrders[index].scheduleDate
        });
      }
    }

    this._calendarService.loadingSubject.next(false);
    this._snackBar.open('Items added successfully', 'close', {
      duration: 1000
    });
    this.editWorkOrderSequence(workOrders[0].calendarItemFrequencyId);
  }

  async saveCalendarWorkOdersToClone(workOrders: WorkOrderUpdateModel[]): Promise<any> {
    for (let index = 0; index < workOrders.length; index++) {
      try {
        const result = await this._workOrderService
          .createFromCalendar(workOrders[index])
          .toPromise();
      } catch (error) { }
    }

    this.loading$.next(false);
    this.editWorkOrderSequence(workOrders[0].calendarItemFrequencyId);
  }

  // Aux
  convertUTCToLocalTime(dateToValidate: any, epochDate: number): any {
    const possibleDate: any = new Date(dateToValidate);
    const dateToCompare = new Date('2000-01-01');

    if (possibleDate < dateToCompare) {
      return null;
    } else {
      return new Date(this.epochPipe.transform(epochDate));
    }
  }
  ngClassWorkOrderStatus(statusId: any, isExpired: any): string {
    return WORK_ORDER_STATUS.find(item => item.key === statusId).value + (isExpired === 1 ? ' overdue' : '');
  }
  workOrderStatus(statusId: any): string {
    return WORK_ORDER_STATUS.find(item => item.key === statusId).value.replace(
      /-/g,
      ' '
    );
  }
  workOrderType(type: number): string {
    return WORK_ORDER_TYPES.find(item => item.key === type).value;
  }
  getMonths(): void {
    this.months = [];
    // tslint:disable-next-line: forin
    for (const month in CALENDAR_MONTH) {
      if (typeof CALENDAR_MONTH[month] === 'number') {
        this.months.push({
          month: CALENDAR_MONTH[month] as any,
          name: month,
          events: 0
        });
      }
    }
  }

  // CalendarEvents
  loadEvents(): void {
    const filterBy = this._calendarService.filterBy;

    let from = new Date(
      this.viewDate.getFullYear(),
      this.viewDate.getMonth(),
      1
    );
    let to = new Date(
      this.viewDate.getFullYear(),
      this.viewDate.getMonth() + 1,
      0
    );

    switch (this.calendarView) {
      case 'day':
        break;
      case 'week':
        break;
      case 'month':
        break;
      case 'year':
        from = new Date(this.viewDate.getFullYear(), 0, 1);
        to = new Date(this.viewDate.getFullYear(), 11, 31);
        break;
    }

    filterBy['DateFrom'] = moment(from).format('YYYY-MM-DD');
    filterBy['DateTo'] = moment(to).format('YYYY-MM-DD');
    this._calendarService.onFilterChanged.next(filterBy);
  }
  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    if (isSameMonth(date, this.viewDate)) {
      this.selectedDay = date;
      this.selectedDateSummaryText = `Events of ${this._datePipe.transform(
        this.selectedDay,
        'mediumDate'
      )}`;
      this.workOrdersOfDay = this.scheduledWorkOrders.filter(
        w =>
          new Date(w.scheduleDate).getDate() === date.getDate() &&
          new Date(w.scheduleDate).getMonth() === date.getMonth()
      );
    }
  }
  monthClicked(month: number): void {
    this.selectedDay = new Date(this.viewDate.getFullYear(), month - 1, 1);
    this.selectedDateSummaryText = `Events of ${this._datePipe.transform(
      this.selectedDay,
      'MMMM'
    )} ${this.selectedDay.getFullYear()}`;
    this.workOrdersOfDay = this.scheduledWorkOrders.filter(
      w => new Date(w.scheduleDate).getMonth() === this.selectedDay.getMonth()
    );
  }

  // Filter
  displayFilters(): void {
    this.viewFilters = this.viewFilters ? false : true;
  }

  // List Options
  editWorkOrder(id: number): void {
    this.loading$.next(true);
    this._workOrderService.get(id, 'update').subscribe(
      (workOrderData: any) => {
        this.loading$.next(false);

        if (!workOrderData) {
          this._snackBar.open('Oops, there was an error', 'close', {
            duration: 1000
          });
          return;
        }

        const workOrderUpdateObj = new WorkOrderUpdateModel(workOrderData);
        this.workOrderFormDialog = this._dialog.open(
          WorkOrderSharedFormComponent,
          {
            panelClass: 'work-order-form-dialog',
            data: {
              workOrder: workOrderUpdateObj,
              action: 'edit'
            }
          }
        );
        this.workOrderFormDialog.afterClosed().subscribe(response => {
          if (!response) {
            return;
          }

          const actionType: string = response[0];
          const formData: FormGroup = response[1];
          const updatedWorkOrderObj = new WorkOrderUpdateModel(
            formData.getRawValue()
          );
          switch (actionType) {
            /**
             * Save
             */
            case 'save':
              this.loading$.next(true);
              this._workOrderService
                .updateElement(updatedWorkOrderObj)
                .then(
                  () => {
                    this.loading$.next(false);
                    this._snackBar.open(
                      'Work Order updated successfully!!!',
                      'close',
                      { duration: 1000 }
                    );
                    this._calendarService.getElements();
                  },
                  () => {
                    this.loading$.next(false);
                    this._snackBar.open('Oops, there was an error', 'close', {
                      duration: 1000
                    });
                  }
                )
                .catch(() => {
                  this.loading$.next(false);
                  this._snackBar.open('Oops, there was an error', 'close', {
                    duration: 1000
                  });
                });

              break;
            /**
             * Delete
             */
            case 'delete':
              this.removeWorkOrder(id);

              break;

            /*
             * Close,is necessary check if work order status changed for refresh the list of work orders
             */
            case 'close':
              break;
          }

          this.workOrderFormDialog = null;
        });
      },
      error => {
        this.loading$.next(false);
        this._snackBar.open('Oops, there was an error', 'close', {
          duration: 1000
        });
      }
    );
  }
  editWorkOrderSequence(calendarItemFrequencyId: number): void {
    this.workOrderSequencesDialog = this._dialog.open(
      WorkOrderSequencesDialogComponent,
      {
        panelClass: 'work-order-sequences-dialog',
        data: {
          calendarItemFrequencyId: calendarItemFrequencyId
        }
      }
    );

    this.workOrderSequencesDialog.afterClosed().subscribe(() => {
      this._calendarService.getElements();

      this.workOrderSequencesDialog = null;
    });
  }
  viewWorkOrderPublicDetails(workOrder: WorkOrderCalendarGridModel): void {
    const urlPublicWorkOrder =
      'http://' + window.location.host + '/work-orders/' + workOrder.guid;
    window.open(urlPublicWorkOrder, '_blank');
  }
  viewWorkOrderActivity(workOrder: WorkOrderCalendarGridModel): void {
    this.workOrderActivityLogDialog = this._dialog.open(
      WoActivityLogDialogComponent,
      {
        panelClass: 'work-order-activity-log-dialog',
        data: {
          id: workOrder.id,
          number: workOrder.originWorkOrderId
            ? workOrder.clonePath
            : workOrder.number
        }
      }
    );
  }
  cloneWorkOrder(id: number): void {
    this.loading$.next(true);
    this._workOrderService.getWorkOrderForCloning(id).subscribe(
      (workOrderData: any) => {
        this.loading$.next(false);
        if (!workOrderData) {
          this._snackBar.open('Oops, there was an error', 'close', {
            duration: 1000
          });
          return;
        }

        const workOrderUpdateObj = new WorkOrderUpdateModel(workOrderData);
        const initDate = new Date(0);
        const todayDate = new Date(
          this.today.getFullYear(),
          this.today.getMonth(),
          this.today.getDate(),
          18,
          0,
          0
        );
        const miliseconds = todayDate.getTime() - initDate.getTime();

        workOrderUpdateObj.dueDate = new Date(
          this.today.getFullYear(),
          this.today.getMonth(),
          this.today.getDate(),
          18,
          0,
          0
        );
        workOrderUpdateObj.epochDueDate = miliseconds / 1000;
        workOrderUpdateObj.statusId = 1;
        workOrderUpdateObj.sendRequesterNotifications = false;
        workOrderUpdateObj.sendPropertyManagersNotifications = false;
        workOrderUpdateObj.notes = [];
        workOrderUpdateObj.scheduleDate = null;
        workOrderUpdateObj.snoozeDate = null;

        this.workOrderFormDialog = this._dialog.open(
          WorkOrderSharedFormComponent,
          {
            panelClass: 'work-order-form-dialog',
            data: {
              workOrder: workOrderUpdateObj,
              action: 'new',
              workOrderFromClone: true,
              source: WorkOrderSourceCode.Other // EMail MG-23
            }
          }
        );

        this.workOrderFormDialog.afterClosed().subscribe(response => {

          if (!response) {
            return;
          }

          if (response[0] === 'close') {
            return;
          }

          this.loading$.next(true);

          const wo = response.getRawValue();

          const setStatusByStandBy: boolean = response.get('setStatusByStandBy').value ? response.get('setStatusByStandBy').value : false;
          const scheduleSettings = wo.scheduleSettings ? new WorkOrderScheduleSetting(wo.scheduleSettings) : null;
          if (scheduleSettings) {
            this._workOrderService
              .create(scheduleSettings, 'AddScheduleSettings')
              .subscribe((settingsResult: any) => {
                const settingsId = settingsResult['body'].id;
                response.controls['workOrderScheduleSettingId'].setValue(
                  settingsId
                );
                let dates: Date[] = [];
                dates = this._workOrderService.calculateScheduleDates(scheduleSettings);
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
                  console.log('no date generated');
                  this.loading$.next(false);
                }
              }, error => {
                this.loading$.next(false);
                this._snackBar.open('Oops, there was an error', 'close', {
                  duration: 1000
                });
              }
              );
          } else {
            this._workOrderService
              .createElement(response.getRawValue())
              .then(
                () => {
                  this.loading$.next(false);
                  this._snackBar.open(
                    'Work Order created successfully!!!',
                    'close',
                    { duration: 1000 }
                  );
                  this._calendarService.getElements();
                },
                () => {
                  this.loading$.next(false);
                  this._snackBar.open('Oops, there was an error', 'close', {
                    duration: 1000
                  });
                }
              )
              .catch(() => {
                this.loading$.next(false);
                this._snackBar.open('Oops, there was an error', 'close', {
                  duration: 1000
                });
              });
          }
          this.workOrderFormDialog = null;
        });
      },
      error => {
        this.loading$.next(false);
        this._snackBar.open('Oops, there was an error', 'close', {
          duration: 1000
        });
      }
    );
  }
  cloneWorkOrderFrequency(id: number): void {
    this.loading$.next(true);
    this._workOrderService.getWorkOrderForCloning(id).subscribe(
      (workOrderData: any) => {
        this.loading$.next(false);
        if (!workOrderData) {
          this._snackBar.open('Oops, there was an error', 'close', {
            duration: 1000
          });
          return;
        }

        const workOrderUpdateObj = new WorkOrderUpdateModel(workOrderData);
        workOrderUpdateObj.statusId = 1;
        workOrderUpdateObj.sendRequesterNotifications = false;
        workOrderUpdateObj.sendPropertyManagersNotifications = false;
        workOrderUpdateObj.notes = [];

        this.itemFormDialog = this._dialog.open(CalendarItemFormComponent, {
          panelClass: 'calendar-item-form-dialog',
          data: {
            action: 'new',
            workOrder: workOrderUpdateObj
          }
        });

        this.itemFormDialog
          .afterClosed()
          .subscribe((calendarItem: CalendarItemFrequencyCreateModel) => {
            if (!calendarItem) {
              return;
            }

            this.loading$.next(true);
            this._calendarService
              .createCalendarItem(calendarItem)
              .then(
                result => {
                  const summary = new CalendarItemFrequencySummaryModel(
                    result['body']
                  );
                  const workOrders: WorkOrderUpdateModel[] = [];

                  for (let index = 0; index < summary.quantity; index++) {
                    const newWo = new WorkOrderUpdateModel(
                      calendarItem.workOrder
                    );

                    newWo.dueDate = null;
                    newWo.scheduleDate = summary.addedDates[index];
                    newWo.calendarItemFrequencyId = summary.id;

                    workOrders.push(newWo);
                  }

                  if (workOrders.length > 0) {
                    this.saveCalendarWorkOdersToClone(workOrders);
                  }
                },
                () => {
                  this.loading$.next(false);
                  this._snackBar.open('Oops, there was an error', 'close', {
                    duration: 1000
                  });
                }
              )
              .catch(() => {
                this.loading$.next(false);
                this._snackBar.open('Oops, there was an error', 'close', {
                  duration: 1000
                });
              });
          });
      },
      error => {
        this.loading$.next(false);
        this._snackBar.open('Oops, there was an error', 'close', {
          duration: 1000
        });
      }
    );
  }
  removeWorkOrder(id: number): void {
    this.confirmDialog = this._dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialog.componentInstance.confirmMessage =
      'Are you sure you want to delete?';

    this.confirmDialog.afterClosed().subscribe(result => {
      if (result) {
        this.loadingList$.next(true);
        this._workOrderService
          .deleteWorkOrderById(id, 'deleteWO')
          .then(
            () => {
              this.loadingList$.next(false);
              this._snackBar.open(
                'Work Order deleted successfully!!!',
                'close',
                { duration: 1000 }
              );
              this._calendarService.getElements();
            },
            error => {
              this.loadingList$.next(false);
              this._snackBar.open(error, 'close', { duration: 1000 });
            }
          )
          .catch(error => {
            this.loadingList$.next(false);
            this._snackBar.open(error, 'close', { duration: 1000 });
          });
      }
      this.confirmDialog = null;
    });
  }

}
