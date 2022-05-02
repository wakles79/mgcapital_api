import {ListItemModel} from '@core/models/common/list-item.model';

export class ListCleaningReportsModel extends  ListItemModel {
  location: string;
  epochCreatedDate: number;
  createdDate: Date;
  number: number;
  customerContactId: number;
  from: string;
  to: string;
}
