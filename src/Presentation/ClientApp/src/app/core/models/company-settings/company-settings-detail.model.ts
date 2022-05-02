import { CompanySettingsBaseModel } from '@app/core/models/company-settings/company-settings-base.model';

export class CompanySettingsDetailModel extends CompanySettingsBaseModel {

  companyName: string;

  constructor(companySettingsDetailModel: CompanySettingsDetailModel) {
    super(companySettingsDetailModel);

    this.companyName = companySettingsDetailModel.companyName || '';
  }

}
