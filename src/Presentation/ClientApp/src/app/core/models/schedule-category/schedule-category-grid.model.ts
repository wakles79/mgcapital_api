import { ScheduleCategoryBaseModel } from './schedule-category-base.model';

export class ScheduleCategoryGridModel extends ScheduleCategoryBaseModel {
  scheduleCategoryName: string;
  subcategories: number;

  constructor(scheduleGrid: ScheduleCategoryGridModel){
    super(scheduleGrid);

    this.subcategories = scheduleGrid.subcategories || 0;
    this.scheduleCategoryName = scheduleGrid.scheduleCategoryName || '';
  }
}
