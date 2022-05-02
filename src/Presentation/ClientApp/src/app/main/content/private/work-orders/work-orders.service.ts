import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { WorkOrderBaseModel } from '@app/core/models/work-order/work-order-base.model';
import { BaseListService } from '@app/core/services/base-list.service';
import { BehaviorSubject, Observable, from } from 'rxjs';
import * as moment from 'moment';
import { WorkOrderScheduleSetting } from '@app/core/models/work-order/work-order-schedule-setting.model';

@Injectable({
  providedIn: 'root'
})
export class WorkOrdersService extends BaseListService<WorkOrderBaseModel> {

  onPublicWorkOrderDetailsChange: BehaviorSubject<any> = new BehaviorSubject<any>([]);

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'workorders', http);
  }

  getServices(
    action = 'readallcbo', filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    params: { [key: string]: string } = {}): Observable<any> {

    // Default data source filter params
    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.apiBaseUrl}api/services/${action}`, {
      params: queryParams
    });
  }

  uploadFile(filesToUpload: any): Observable<any> {
    const formData: FormData = new FormData();
    // tslint:disable-next-line: prefer-for-of
    for (let i = 0; i < filesToUpload.length; i++) {
      formData.append('woImageAttachment', filesToUpload[i], filesToUpload[i].name);
    }
    return this.http.post(`${this.fullUrl}/uploadattachments`, formData, { observe: 'response' });
  }

  uploadFileWithName(filesToUpload: { name: string, file: File }[]): Observable<any> {
    const formData: FormData = new FormData();
    // tslint:disable-next-line: prefer-for-of
    for (let i = 0; i < filesToUpload.length; i++) {
      formData.append('woImageAttachment', filesToUpload[i].file, filesToUpload[i].name);
    }
    return this.http.post(`${this.fullUrl}/uploadattachments`, formData, { observe: 'response' });
  }

  deleteAttachmentByBlobName(blobName): Observable<any> {
    return this.http.delete(`${this.fullUrl}/DeleteAttachmentByBlobName`, blobName);
  }

  getWorkOrderForCloning(workOrderId: any): Observable<any> {
    return this.http.get(`${this.fullUrl}/${'getforcloning'}?id=${workOrderId}`);
  }

  getWorkOrderPublic(workOrderGuid: any): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.get(`${this.fullUrl}/${'publicget'}?guid=${workOrderGuid}`, { observe: 'response' }).
        subscribe((response: any) => {
          if (response.status === 200) {
            this.onPublicWorkOrderDetailsChange.next(response.body);
          }
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  getStatusLog(
    workOrderGuid = null, workOrderId = null,
    params: { [key: string]: string } = {}): Observable<any> {

    // Default data source filter params
    let queryParams = new HttpParams()
      .set('workOrderGuid', workOrderGuid)
      .set('workOrderId', workOrderId)
      .set('pageNumber', '0')
      .set('pageSize', '9999'); // HACK: As long as it needs

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.apiBaseUrl}api/workOrderStatusLog/readAll`, {
      params: queryParams
    });
  }

  getActivityLog(
    workOrderGuid = null, workOrderId = null,
    params: { [key: string]: string } = {}): Observable<any> {

    // Default data source filter params
    let queryParams = new HttpParams()
      .set('entityGuid', workOrderGuid)
      .set('entityId', workOrderId)
      .set('pageNumber', '0')
      .set('pageSize', '9999'); // HACK: As long as it needs

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.apiBaseUrl}api/workOrderActivityLog/readAll`, {
      params: queryParams
    });
  }

  deleteWorkOrder(elementToDelete, action = 'delete'): Promise<any> {
    const element = this.allElementsToList.find(e => e.id === elementToDelete.id);
    const elementIndex = this.allElementsToList.indexOf(element);

    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}/?id=${element.id}`, { observe: 'response' })
        .subscribe((response: any) => {
          if (response.status === 200) {
            this.allElementsToList.splice(elementIndex, 1);
            this.allElementsChanged.next(this.allElementsToList);
            this.elementsCount = this.allElementsToList.length;
          }
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  deleteWorkOrderById(id: number, action = 'delete'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}/?id=${id}`, { observe: 'response' })
        .subscribe((response: any) => {
          if (response.status === 200) {

          }
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  createWorkOrdenFromPreCalendar(elementData): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(elementData, 'AddWorkOrdenFromPreCalendar')
        .subscribe(response => {
          this.getElements();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  updateDueDate(id: number, dueDate: Date, action = 'UpdateSnoozeDate'): Observable<any> {

    const queryParams = new HttpParams()
      .set('id', id.toString())
      .set('snoozeDate', moment(dueDate).format('YYYY-MM-DD'));


    return this.http.post(`${this.fullUrl}/${action}`, null, {
      params: queryParams
    });
  }

  createFromCalendar(resource, action = 'add'): Observable<any> {
    return this.http.post<any>(`${this.fullUrl}/${action}`, resource);
  }

  getAllTasks(woId: number, action = 'ReadAllWorkOrderTasks'): Observable<any> {
    const queryParams = new HttpParams()
      .set('workOrderId', woId.toString());

    return this.http.get(`${this.fullUrl}/${action}`, {
      params: queryParams
    });
  }

  getTask(id: any, action = 'GetWorkOrderTask'): Observable<any> {
    const queryParams = new HttpParams()
      .set('id', id);

    return this.http.get(`${this.fullUrl}/${action}`, {
      params: queryParams
    });
  }

  deleteTask(id: any, action = 'DeleteTask'): Observable<any> {
    const httpParams = new HttpParams()
      .set('id', id);
    return this.http.delete(`${this.fullUrl}/${action}`, { params: httpParams });
  }

  updateCompleteTaskStatus(id: number, completed: boolean, quantityExecuted: number, hoursExecuted: number, completedDate: Date, action = 'UpdateTaskStatus'): Observable<any> {
    const resource = {
      id: id,
      isComplete: completed,
      quantityExecuted: quantityExecuted,
      hoursExecuted: hoursExecuted,
      completedDate: completedDate
    };
    return this.http.post<any>(`${this.fullUrl}/${action}`, resource);
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
}
