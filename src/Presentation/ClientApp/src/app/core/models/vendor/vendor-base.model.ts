import { CompanyEntity } from '@core/models/common/company-entity.model';

export class VendorBaseModel extends CompanyEntity {
  code: string;
  vendorTypeId: number;
  isPerson: boolean;
  ssn: string;
  fein: string;
  federalId: string;
  is1099: boolean;
  isSensitiveAccount: boolean;
  name: string;
  termsDaysOrProx: string;
  termsDiscPercent: number;
  termsDiscDays: number;
  termsNet: number;
  accountNumber: string;
  defaultGLAccountNumber1: string;
  defaultGLAccountNumber2: string;
  groupIds: number[];

  /**
   *
   */
  constructor(vendor: VendorBaseModel) {
    super(vendor);
    this.code = vendor.code || '';
    this.vendorTypeId = vendor.vendorTypeId || 0;
    this.isPerson = vendor.isPerson || false;
    this.ssn = vendor.ssn || '';
    this.fein = vendor.fein || '';
    this.federalId = vendor.federalId || '';
    this.is1099 = vendor.is1099 || false;
    this.isSensitiveAccount = vendor.isSensitiveAccount || false;
    this.name = vendor.name || '';
    this.termsDaysOrProx = vendor.termsDaysOrProx || '';
    this.termsDiscPercent = vendor.termsDiscPercent || 0;
    this.termsDiscDays = vendor.termsDiscDays || 0;
    this.defaultGLAccountNumber2 = vendor.defaultGLAccountNumber2 || '';
    this.termsNet = vendor.termsNet || 0;
    this.defaultGLAccountNumber1 = vendor.defaultGLAccountNumber1 || '';
    this.accountNumber = vendor.accountNumber || '';
    this.groupIds = vendor.groupIds ? vendor.groupIds.slice() : [];
  }
}
