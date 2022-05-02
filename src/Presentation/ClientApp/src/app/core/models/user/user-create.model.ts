import { UserBaseModel } from './user-base.model';
import { RegisterInterface } from '../account/register.interface';

export class UserCreateModel extends UserBaseModel implements RegisterInterface {

    /**
     *
     */
    constructor(user: UserCreateModel) {
        super(user);
    }    
}
