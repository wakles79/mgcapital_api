import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { WorkOrderScheduleSetting } from '@app/core/models/work-order/work-order-schedule-setting.model';
import { WorkOrderUpdateModel } from '@app/core/models/work-order/work-order-update.model';
import { WorkOrderSequencesDialogComponent } from '@app/core/modules/work-order-dialog/work-order-sequences-dialog/work-order-sequences-dialog.component';
import { WorkOrderSharedFormComponent } from '@app/core/modules/work-order-form/work-order-form/work-order-form.component';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';
import { BehaviorSubject } from 'rxjs';
import { WorkOrdersService } from './work-orders.service';

@Component({
  selector: 'app-work-orders',
  templateUrl: './work-orders.component.html',
  styleUrls: ['./work-orders.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class WorkOrdersComponent implements OnInit {

  dialogRef: any;
  loadingList$ = new BehaviorSubject<boolean>(false);
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  workOrderSequencesDialog: MatDialogRef<WorkOrderSequencesDialogComponent>;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private workOrderService: WorkOrdersService,
    private router: Router,
    private route: ActivatedRoute,
    private _fuseSidebarService: FuseSidebarService
  ) {
  }


  ngOnInit(): void {
    // See https://stackoverflow.com/questions/47455734/how-get-query-parameters-from-url-in-angular-5/51808539#51808539

    this.route.queryParamMap.subscribe((map: any) => {
      const action = map.params['action'];
      if (action === 'add') {
        this.newWorkOrder();
      }
      else if (action === 'edit') {
        const workOrderId = map.params['woId'];
        if (workOrderId) {
          this.editWorkOrder(workOrderId);
        }
      }
    });
  }

  newWorkOrder(): void {
    this.dialogRef = this.dialog.open(WorkOrderSharedFormComponent, {
      panelClass: 'work-order-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.dialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        const wo = response.getRawValue();
        const scheduleSettings = wo.scheduleSettings ? new WorkOrderScheduleSetting(wo.scheduleSettings) : null;
        if (scheduleSettings) {
          this.workOrderService.loadingSubject.next(true);

          this.workOrderService.create(scheduleSettings, 'AddScheduleSettings')
            .subscribe((settingsResult: any) => {
              const settingsId = settingsResult['body'].id;
              response.controls['workOrderScheduleSettingId'].setValue(settingsId);
              let dates: Date[] = [];
              dates = this.calculateScheduleDates(scheduleSettings);

              if (dates.length > 0) {
                const workOrders: any[] = [];
                let index = 0;

                dates.forEach(d => {
                  let unscheduled = false;
                  if (!scheduleSettings.scheduleDate && (scheduleSettings.frequency > 2 && scheduleSettings.frequency < 6)) {
                    unscheduled = true;
                  } else if (index > 0 && (scheduleSettings.frequency > 2 && scheduleSettings.frequency < 6)) {
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

            }, (error) => {
              this.workOrderService.loadingSubject.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            });
        } else {
          // Create single work Order
          this.workOrderService.createElement(response.getRawValue())
            .then(
              () => this.snackBar.open('Service orders created successfully!!!', 'close', { duration: 1000 }),
              () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
            .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
        }
      });
  }

  editWorkOrder(workOrderId): void {
    this.workOrderService.get(workOrderId, 'update')
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

                  if (workOrderData.statusId !== workOrderUpdateObj.statusId) {
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
          const currentDailyDate = new Date(settings.startDate.getFullYear(), settings.startDate.getMonth(), settings.startDate.getDate());
          while (currentDailyDate <= settings.endDate) {
            dates.push(new Date(currentDailyDate.getFullYear(), currentDailyDate.getMonth(), currentDailyDate.getDate()));
            currentDailyDate.setDate(currentDailyDate.getDate() + 1);
          }
        }
        break;
      case 1:
        // Parse days to string
        const strDays: string[] = [];
        settings.days.forEach(d => { strDays.push(`${d}`); });
        const currentDate = new Date(settings.startDate.getFullYear(), settings.startDate.getMonth(), settings.startDate.getDate());
        while (currentDate <= settings.endDate) {
          const dayWeek = currentDate.getDay();
          const match = strDays.find(d => d === dayWeek.toString());
          if (match) {
            dates.push(new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate()));
          }
          currentDate.setDate(currentDate.getDate() + 1);
        }
        break;
      case 2:
        dates = this.calculateMonthlyDates(settings.ordinal, settings.startValue, settings.endValue, settings.days);
        break;
      case 3:
        dates = this.calculateQuarterlyScheduleDates(settings.frequency, settings.startDate, settings.endDate, settings.excludedScheduleDates);
        if (settings.scheduleDate) {
          if (dates.length > 0) {
            dates[0] = settings.scheduleDate;
          }
        }
        break;
      case 4:
        dates = this.calculateQuarterlyScheduleDates(settings.frequency, settings.startDate, settings.endDate, settings.excludedScheduleDates);
        if (settings.scheduleDate) {
          if (dates.length > 0) {
            dates[0] = settings.scheduleDate;
          }
        }
        break;
      case 5:
        dates = this.calculateQuarterlyScheduleDates(settings.frequency, settings.startDate, settings.endDate, settings.excludedScheduleDates);
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

  private calculateMonthlyDates(ordinal: number, startMonth: number, endMonth: number, days: number[]): Date[] {
    startMonth = startMonth - 1;
    endMonth = endMonth - 1;

    // Parse days to string
    const strDays: string[] = [];
    days.forEach(d => { strDays.push(`${d}`); });

    const dates: Date[] = [];
    const today = new Date();
    const startDate = new Date(today.getFullYear(), startMonth, 1);
    const endDate = new Date(endMonth < startMonth ? today.getFullYear() + 1 : today.getFullYear(), endMonth, 1);
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

  private getMonthWeekRange(ordinal: number, month: number, year: number): Date[] {
    const range: Date[] = [];
    const startWeek = new Date(year, month, 1);
    const endWeek = new Date(year, month, 1);
    // set date to the las day of week
    endWeek.setDate(
      endWeek.getDate() + (7 - (endWeek.getDay() + 1))
    );

    switch (ordinal) {
      case 1:
        range[0] = startWeek;
        range[1] = endWeek;
        break;
      case 2:
        // go to the second week
        const secondStartWeek = new Date(endWeek.getFullYear(), endWeek.getMonth(), endWeek.getDate() + 1);
        const secondEndWeek = new Date(endWeek.getFullYear(), endWeek.getMonth(), endWeek.getDate() + 1);
        secondEndWeek.setDate(
          secondEndWeek.getDate() + (7 - (secondEndWeek.getDay() + 1))
        );
        range[0] = secondStartWeek;
        range[1] = secondEndWeek;
        break;
      case 3:
        // go to the third week
        const thirdStartWeek = new Date(endWeek.getFullYear(), endWeek.getMonth(), endWeek.getDate() + 8);
        const thirdEndWeek = new Date(endWeek.getFullYear(), endWeek.getMonth(), endWeek.getDate() + 8);
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
          lastStartWeek.getDate() - (7 - (7 - (lastStartWeek.getDay())))
        );
        range[0] = lastStartWeek;
        range[1] = lastEndWeek;
        break;
    }
    return range;
  }

  private calculateQuarterlyScheduleDates(selectedScheduleFrequency: number, startDate: Date, endDate: Date, excluded: Date[]): Date[] {
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
      const curentMonth = new Date(startDate.getFullYear(), startDate.getMonth(), 1);
      while (curentMonth <= endDate) {
        const match = excluded.find(d => d.getFullYear() === curentMonth.getFullYear() && d.getMonth() === curentMonth.getMonth());
        if (!match) {
          dates.push(new Date(curentMonth.getFullYear(), curentMonth.getMonth(), 1));
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
        const result = await this.workOrderService.createFromCalendar(workOrders[index]).toPromise();
      } catch (error) {
      }
    }

    this.displayWorkOrderSequence(scheduleSettingId);
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
