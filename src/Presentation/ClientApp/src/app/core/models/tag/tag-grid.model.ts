import { EntityModel } from '@app/core/models/common/entity.model';

export class TagGridModel extends EntityModel {
  epochCreatedDate: number;
  epochUpdatedDate: number;
  description: string;
  referenceCount: number;
  hexColor: string;

  constructor(tagGrid) {
    super(tagGrid);

    this.epochCreatedDate = tagGrid.epochCreatedDate || 0;
    this.epochUpdatedDate = tagGrid.epochUpdatedDate || 0;
    this.description = tagGrid.description || '';
    this.referenceCount = tagGrid.referenceCount || 0;
    this.hexColor = tagGrid.hexColor || '#000000';
  }
}
