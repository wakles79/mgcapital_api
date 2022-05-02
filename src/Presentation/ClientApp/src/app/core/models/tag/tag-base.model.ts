import { EntityModel } from '@app/core/models/common/entity.model';

export class TagBaseModel extends EntityModel {
  description: string;
  type: number;
  hexColor: string;

  constructor(tag) {
    super(tag);

    this.description = tag.description || '';
    this.type = tag.type || 0;
    this.hexColor = tag.hexColor || '#000000';
  }
}
