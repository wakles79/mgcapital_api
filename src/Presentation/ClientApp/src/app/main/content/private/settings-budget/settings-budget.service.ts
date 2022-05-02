import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { BudgetSettingsBaseModel } from '@app/core/models/budget-settings/budget-settings-base.model';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SettingsBudgetService extends BaseListService<BudgetSettingsBaseModel> {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, '', http);
  }

  loadSettings(): void { }

}
