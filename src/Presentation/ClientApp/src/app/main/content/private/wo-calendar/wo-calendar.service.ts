import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { CalendarItemFrequencyCreateModel } from '@app/core/models/calendar/calendar-item-frequency-create.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WoCalendarService extends BaseListService<any> {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'calendar', http);
  }

  createCalendarItem(item: CalendarItemFrequencyCreateModel, action = 'AddCalendarItemFrequency'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(item, action)
        .subscribe(response => {
          // this.getElements();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  getWorkOrdersBySequence(calendarItemFrequencyId: number, action = 'ReadWorkOrderBySequence'): Observable<any> {
    const pars = new HttpParams()
      .set('calendarItemFrequencyId', calendarItemFrequencyId.toString());

    return this.http.get(`${this.fullUrl}/${action}`, { params: pars });
  }
}
