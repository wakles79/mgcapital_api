import { UserBaseModel } from './user-base.model';


export class UserGridModel extends UserBaseModel {

    departmentName: string;
    roleName: string;

    /**
     *
     */
    constructor(user: UserGridModel) {
        super(user);
        this.departmentName = user.departmentName || '';
        this.roleName = user.roleName || '';
    }
}
