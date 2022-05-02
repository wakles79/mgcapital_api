import { DataSourceRequestModel } from './data-source-request.model';

export class ListDataSourceRequestModel extends DataSourceRequestModel {

  filterApiUrl: string;
  elementId: any;

  constructor(request) {
    super(request);
    this.filterApiUrl = request.filterApiUrl || '';
    this.elementId = request.elementId || '';
  }
}
