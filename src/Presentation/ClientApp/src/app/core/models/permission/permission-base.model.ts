import { EntityModel } from '@app/core/models/common/entity.model';
import { ApplicationModuleEnum } from '@app/core/models/permission/application-module-enum';
import { PermissionActionTypeEnum } from '@app/core/models/permission/permission-action-type-enum';

export class PermissionBaseModel extends EntityModel {
  name: string;
  module: ApplicationModuleEnum;
  type: PermissionActionTypeEnum;

  constructor(permission) {
    super(permission);
  }
}
