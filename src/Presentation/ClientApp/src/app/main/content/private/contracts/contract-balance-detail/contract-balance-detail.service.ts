import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ContractReportDetailsModel } from '@app/core/models/contract/contract-report-detail.model';
import { BaseListService } from '@app/core/services/base-list.service';
import { BehaviorSubject, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ContractBalanceDetailService extends BaseListService<ContractReportDetailsModel> {

  contractReportDetail: ContractReportDetailsModel;
  onContractReportDetailChanged: BehaviorSubject<ContractReportDetailsModel> = new BehaviorSubject<ContractReportDetailsModel>(null);

  contractItems: any[] = [];
  onContractItemsChanged: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  contractExpenses: any[] = [];
  onContractExpensesChanged: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  onDatesChangesFilter: Subject<any> = new Subject();
  id: any;

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'contracts', http);

    this.onDatesChangesFilter.subscribe((dates: any) => {
      this.dateFrom = dates['dateFrom'];
      this.dateTo = dates['dateTo'];
      this.getDetails(this.id);
    });
  }

  getDetails(contractId: number): Promise<any> {
    const params = new HttpParams()
      .set('id', contractId.toString())
      .set('dateTo', this.dateTo.toString())
      .set('dateFrom', this.dateFrom.toString());
    this.id = contractId;
    // .set('dateFrom', this.filterBy.dateFrom)
    return this.getContractReportData('getReportDetailsBalances', params);
  }

  private getContractReportData(action: string, params: HttpParams): Promise<any> {
    return new Promise((resolve, reject) => {

      let extendedParams: { [key: string]: string } = {};
      extendedParams = this.extendParams(params, this.filterBy);

      this.http.get(`${this.fullUrl}/${action}`, { params: params })
        .subscribe((response: any) => {
          resolve(response);

          this.contractReportDetail = response;
          this.onContractReportDetailChanged.next(this.contractReportDetail);

          this.contractItems = this.contractReportDetail.contractItems || [];
          this.onContractItemsChanged.next(this.contractItems);

          this.contractExpenses = this.contractReportDetail.contractExpenses || [];
          this.onContractExpensesChanged.next(this.contractExpenses);

        }, (error) => { reject(error); });
    });
  }

  getElements(
    action = 'readall',
    filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    params: { [key: string]: any } = {}
  ): any {
    if (!filter || filter === '') {
      filter = this.searchText;
    }

    this.loadingSubject.next(true);

    let extendedParams: { [key: string]: string } = {};
    extendedParams = this.extendParams(params, this.filterBy);

    return this.getAll(action, filter, sortField, sortOrder, pageNumber, pageSize, extendedParams)
      .subscribe((response: { count: number, payload: ContractReportDetailsModel[] }) => {
        this.allElementsToList = response.payload;
        this.elementsCount = response.count;
        this.allElementsChanged.next(this.allElementsToList);
      },
        (error) => {
          this.loadingSubject.next(false);
        },
        () => {
          this.loadingSubject.next(false);
        }
      );
  }

}
