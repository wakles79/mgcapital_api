import { EntityModel } from '../common/entity.model';

export class ScheduleSubcategoryBaseModel extends EntityModel {
    name: string;
    scheduleSettingCategoryId: number;
    rate: number;
    rateType: number;
    periodicity: string;
    status: boolean;


    constructor(scheduleCategory: ScheduleSubcategoryBaseModel) {
        super(scheduleCategory);

        this.id = scheduleCategory.id || 0;
        this.name = scheduleCategory.name || '';
        this.scheduleSettingCategoryId = scheduleCategory.scheduleSettingCategoryId || -1;
        this.rate = scheduleCategory.rate || 0;
        this.rateType = scheduleCategory.rateType || 0;
        this.periodicity = scheduleCategory.periodicity || '';
        this.status = scheduleCategory.status || false;
    }
}
