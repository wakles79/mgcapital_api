import { ListItemModel } from '@core/models/common/list-item.model';
import { ScheduleSubcategoryBaseModel } from './schedule-subcategory-base.model';

export class ListScheduleSubCategoryModel extends ScheduleSubcategoryBaseModel {
    categoryName: string;

    constructor(schedule: ListScheduleSubCategoryModel) {
        super(schedule);

        this.categoryName = schedule.categoryName || '';
    }
}
