import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { CleaningReportGridModel } from '@app/core/models/reports/cleaning-report/cleaning.report.grid.model';
import { BaseListService } from '@app/core/services/base-list.service';
import * as moment from 'moment';

@Injectable({
  providedIn: 'root'
})
export class CleaningReportService extends BaseListService<CleaningReportGridModel> {

  pendingToReplyCount: number;
  repliedCount: number;

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'cleaningreports', http);

    this.filterBy = {
      'statusid': 'null',
      'dateFrom': moment(this.dateFrom).format('YYYY-MM-DD'),
      'dateTo': moment(this.dateTo).format('YYYY-MM-DD')
    };

  }


  getElements(
    action = 'readall',
    filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    params: { [key: string]: any } = {}
  ): any {
    if (!filter || filter === '') {
      filter = this.searchText;
    }

    this.loadingSubject.next(true);

    let extendedParams: { [key: string]: string } = {};
    extendedParams = this.extendParams(params, this.filterBy);

    return this.getAll(action, filter, sortField, sortOrder, pageNumber, pageSize, extendedParams)
      .subscribe((response: { count: number, payload: CleaningReportGridModel[], pendingToReplyCount: number, repliedCount: number }) => {
        this.allElementsToList = response.payload;
        this.elementsCount = response.count;
        this.allElementsChanged.next(this.allElementsToList);
        this.pendingToReplyCount = response.pendingToReplyCount;
        this.repliedCount = response.repliedCount;
      },
        (error) => {
          this.loadingSubject.next(false);
        },
        () => {
          this.loadingSubject.next(false);
        }
      );
  }
}
