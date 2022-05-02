import { PermissionBaseModel } from './permission-base.model';

export class PermissionAssignmentModel extends PermissionBaseModel {

  fullName: string;
  isAssigned: boolean;

  constructor(permissionAssignmentModel: PermissionAssignmentModel) {
    super(permissionAssignmentModel);

    this.fullName = permissionAssignmentModel.fullName || '';
    this.isAssigned = permissionAssignmentModel.isAssigned || false;
  }
}
