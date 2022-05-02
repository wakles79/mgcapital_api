import { EntityModel } from '@app/core/models/common/entity.model';

export class BudgetSettingsBaseModel extends EntityModel {
  companyId: number;
  minimumProfitMarginPercentage: number;
  federalInsuranceContributionsAct: number;
  medicare: number;
  federalUnemploymentTaxAct: number;
  stateUnemploymentInsurance: number;
  workersCompensation: number;
  generalLedger: number;

  constructor(budgetSettingsBaseModel: BudgetSettingsBaseModel) {
    super(budgetSettingsBaseModel);

    this.companyId = budgetSettingsBaseModel.companyId || 0;
    this.minimumProfitMarginPercentage = budgetSettingsBaseModel.minimumProfitMarginPercentage || 0;
    this.federalInsuranceContributionsAct = budgetSettingsBaseModel.federalInsuranceContributionsAct || 0;
    this.medicare = budgetSettingsBaseModel.medicare || 0;
    this.federalUnemploymentTaxAct = budgetSettingsBaseModel.federalUnemploymentTaxAct || 0;
    this.stateUnemploymentInsurance = budgetSettingsBaseModel.stateUnemploymentInsurance || 0;
    this.workersCompensation = budgetSettingsBaseModel.workersCompensation || 0;
    this.generalLedger = budgetSettingsBaseModel.generalLedger || 0;
  }
}
