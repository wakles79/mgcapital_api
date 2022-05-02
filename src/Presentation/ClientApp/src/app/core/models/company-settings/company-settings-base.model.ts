import { CompanyEntity } from '@core/models/common/company-entity.model';

export class CompanySettingsBaseModel extends CompanyEntity {

  companyId: number;
  minimumProfitMarginPercentage: number;
  federalInsuranceContributionsAct: number;
  medicare: number;
  federalUnemploymentTaxAct: number;
  stateUnemploymentInsurance: number;
  workersCompensation: number;
  generalLedger: number;
  stateTax: number;
  totalProfitAmount: number;
  totalProfitRatio: number;

  logoBlobName: string;
  logoFullUrl: string;

  freshdeskEmail: string;
  freshdeskDefaultAgentId: string;
  freshdeskDefaultApiKey: string;

  gmailEnabled: boolean;
  gmailEmail: string;

  emailSignature: string;


  constructor(companySettingsBaseModel: CompanySettingsBaseModel) {
    super(companySettingsBaseModel);

    this.companyId = companySettingsBaseModel.companyId || 0;
    this.minimumProfitMarginPercentage = companySettingsBaseModel.minimumProfitMarginPercentage || 0;
    this.totalProfitAmount = companySettingsBaseModel.totalProfitAmount || 0;
    this.totalProfitRatio = companySettingsBaseModel.totalProfitRatio || 0;
    this.federalInsuranceContributionsAct = companySettingsBaseModel.federalInsuranceContributionsAct || 0;
    this.medicare = companySettingsBaseModel.medicare || 0;
    this.federalUnemploymentTaxAct = companySettingsBaseModel.federalUnemploymentTaxAct || 0;
    this.stateUnemploymentInsurance = companySettingsBaseModel.stateUnemploymentInsurance || 0;
    this.workersCompensation = companySettingsBaseModel.workersCompensation || 0;
    this.generalLedger = companySettingsBaseModel.generalLedger || 0;
    this.stateTax = companySettingsBaseModel.stateTax || 0;

    this.logoBlobName = companySettingsBaseModel.logoBlobName || '';
    this.logoFullUrl = companySettingsBaseModel.logoFullUrl || '';

    this.freshdeskEmail = companySettingsBaseModel.freshdeskEmail || '';
    this.freshdeskDefaultAgentId = companySettingsBaseModel.freshdeskDefaultAgentId || '';
    this.freshdeskDefaultApiKey = companySettingsBaseModel.freshdeskDefaultApiKey || '';

    this.gmailEnabled = companySettingsBaseModel.gmailEnabled || false;
    this.gmailEmail = companySettingsBaseModel.gmailEmail || '';
    this.emailSignature = companySettingsBaseModel.emailSignature;
  }

}

export enum AccessType {
  'Full',
  'ReadOnly',
  'None'
}

export enum ApplicationModule {
  'Dashboard',
  'Inbox',
  'WorkOrder',
  'DailyReport',
  'BillableReport',
  'CleaningReport',
  'Inspections',
  'Calendar',
  'Scheduled',
  'Buildings',
  'Proposals',
  'Budgets',
  'Revenues',
  'Expenses',
  'CompanySettings',
  'Services',
  'ContractServicesCatalog',
  'ExpensesTypesCatalog',
  'Users',
  'Contacts',
  'ManagementCo',
  'EmailActivity',
  'Roles',
  'ScheduledWorkOrderCategories',
  'BudgetSettings',
  'WorkOrderServicesCatalog',
  'AddWorkOrder',
}
