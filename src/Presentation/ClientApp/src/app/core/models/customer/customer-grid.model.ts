import { CustomerBaseModel } from './customer-base.model';


export class CustomerGridModel extends CustomerBaseModel {

    email: string;
    phone: string;
    fullAddress: string;
    contactsTotal: number;

    /**
     *
     */
    constructor(customer: CustomerGridModel) {
        super(customer);
        this.phone = customer.phone || '';
        this.email = customer.email || '';
        this.fullAddress = customer.fullAddress || '';
        this.contactsTotal = customer.contactsTotal || 0;
    }
}
