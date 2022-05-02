export class AddressModel {
  addressId: number;
  addressLine1: string;
  addressLine2: string;
  city: string;
  state: string;
  zipCode: string;
  countryCode: string;
  latitude: number;
  longitude: number;
  fullAddress: string;
  entityId: number;
  type: string;
  name: string;
  default: boolean;

  constructor(init?: Partial<AddressModel>) {
    Object.assign(this, init);
  }
}
