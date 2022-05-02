import { VendorBaseModel } from './vendor-base.model';


export class VendorGridModel extends VendorBaseModel {

    email: string;
    phone: string;
    fullAddress: string;

    /**
     *
     */
    constructor(vendor: VendorGridModel) {
        super(vendor);
        this.phone = vendor.phone || '';
        this.email = vendor.email || '';
        this.fullAddress = vendor.fullAddress || '';
    }
}
