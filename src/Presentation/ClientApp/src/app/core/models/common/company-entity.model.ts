import { EntityModel } from '@core/models/common/entity.model';

export class CompanyEntity extends EntityModel {
  guid: string;
  constructor(entity: CompanyEntity) {
    super(entity);
    this.guid = entity.guid;
  }
}
