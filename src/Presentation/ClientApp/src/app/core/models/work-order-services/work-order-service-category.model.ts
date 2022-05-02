import { EntityModel } from '@app/core/models/common/entity.model';

export class WorkOrderServiceCategoryModel extends EntityModel {
  name: string;
  constructor(category) {
    super(category);

    this.name = category.name || '';
  }
}
