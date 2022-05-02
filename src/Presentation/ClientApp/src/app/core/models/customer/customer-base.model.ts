import { CompanyEntity } from '@core/models/common/company-entity.model';

export class CustomerBaseModel extends CompanyEntity {

  code: string; //
  name: string; //
  statusId: number;
  isGenericAccount: boolean; //
  notes: string; //
  minimumProfitMargin: number;
  poNumberRequired: boolean; //
  creditLimit: number; //
  crHoldFlag: string; //
  creditTerms: string; //
  showPricesOnShipper: boolean; //
  insuredUpTo: number; //
  insurerName: string; //
  financeCharges: boolean; //
  gracePeriodInDays: number;
  groupIds: number[];
  customerSince: string;
  statusName: string;

  /**
   *
   */
  constructor(customer: CustomerBaseModel) {
    super(customer);
    this.code = customer.code || '';
    this.name = customer.name || '';
    this.statusId = customer.statusId || 0;
    this.isGenericAccount = customer.isGenericAccount || false;
    this.notes = customer.notes || '';
    this.minimumProfitMargin = customer.minimumProfitMargin || 0;
    this.poNumberRequired = customer.poNumberRequired || false;
    this.creditLimit = customer.creditLimit || 0;
    this.crHoldFlag = customer.crHoldFlag || '';
    this.creditTerms = customer.creditTerms || '';
    this.showPricesOnShipper = customer.showPricesOnShipper || false;
    this.insuredUpTo = customer.insuredUpTo || 0;
    this.insurerName = customer.insurerName || '';
    this.financeCharges = customer.financeCharges || false;
    this.gracePeriodInDays = customer.gracePeriodInDays || 0;
    this.customerSince = customer.customerSince || '';
    this.statusName = customer.statusName || '';
    this.groupIds = customer.groupIds ? customer.groupIds.slice() : [];
  }
}
