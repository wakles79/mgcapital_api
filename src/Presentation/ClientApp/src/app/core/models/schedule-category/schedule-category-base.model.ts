import { CompanyEntity } from '../common/company-entity.model';
import { ExpenseTypeBaseModel } from '../expense-type/expense-type-base.model';

export class ScheduleCategoryBaseModel extends CompanyEntity {
    scheduleCategoryType: number;
    description: string;
    status: boolean;
    color: number;

    constructor(schedule: ScheduleCategoryBaseModel) {
        super(schedule);

        this.scheduleCategoryType = schedule.scheduleCategoryType || 0;
        this.description = schedule.description || '';
        this.status = schedule.status || false;
        this.color = schedule.color || null;
    }
}
