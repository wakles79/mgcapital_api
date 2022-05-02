import { ListItemModel } from '@core/models/common/list-item.model';

export class ListContactModel extends ListItemModel {
  email: string;
  phone: string;
  fullAddress: string;
  fullName: string = name;
}
