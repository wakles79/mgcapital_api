import { EntityModel } from '@app/core/models/common/entity.model';

export class RoleBaseModel extends EntityModel {
  name: string;
  level: number;
  type: number;

  constructor(role: RoleBaseModel) {
    super(role);

    this.name = role.name || '';
    this.level = role.level || 0;
    this.type = role.type || 0;
  }
}
