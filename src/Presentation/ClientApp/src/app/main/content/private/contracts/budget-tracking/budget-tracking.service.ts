import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { ContractTrackingDetailModel } from '@app/core/models/contract/contract-tracking-detail.model';
import { BehaviorSubject } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class BudgetTrackingService extends BaseListService<ContractTrackingDetailModel> {

  budget: ContractTrackingDetailModel;
  onBudgetChanged: BehaviorSubject<ContractTrackingDetailModel> = new BehaviorSubject<ContractTrackingDetailModel>(null);

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'contracts', http);
  }

  getDetails(budgetId: number, datefrom = null, dateTo = null): Promise<any> {
    const httpParams = new HttpParams()
      .set('id', budgetId.toString())
      .set('dateFrom', datefrom)
      .set('dateTo', dateTo);

    return this.getBudgetData(httpParams);
  }

  private getBudgetData(httpParams: HttpParams, action: string = 'GetReportTrackingDetails'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.get(`${this.fullUrl}/${action}`, { params: httpParams })
        .subscribe((response: any) => {
          resolve(response);

          this.budget = response;
          this.onBudgetChanged.next(this.budget);

        }, (error) => { reject(error); });
    });
  }

}
