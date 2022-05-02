import * as moment from 'moment';

export class DataSourceRequestModel {
  filter: any;
  sortField: any;
  sortOrder: string;
  pageNumber: any;
  pageSize: any;
  params: { [key: string]: string };

  constructor(request) {
    this.filter = request.filter || '';
    this.sortField = request.sortField || '';
    this.sortOrder = request.sortOrder || '';
    this.pageNumber = request.pageNumber || 0;
    this.pageSize = request.pageSize || 20;
    this.params = request.params || {};
  }
}
