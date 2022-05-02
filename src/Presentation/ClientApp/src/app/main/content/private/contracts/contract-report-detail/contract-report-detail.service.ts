import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { ContractReportDetailsModel } from '@app/core/models/contract/contract-report-detail.model';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ContractNoteBaseModel } from '@app/core/models/contract/contract-note-base.model';

@Injectable({
  providedIn: 'root'
})
export class ContractReportDetailService extends BaseListService<ContractReportDetailsModel> {

  contractReportDetail: ContractReportDetailsModel;
  onContractReportDetailChanged: BehaviorSubject<ContractReportDetailsModel> = new BehaviorSubject<ContractReportDetailsModel>(null);

  contractItems: any[] = [];
  onContractItemsChanged: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  contractExpenses: any[] = [];
  onContractExpensesChanged: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'contracts', http);
  }

  getDetails(contractId: number): Promise<any> {
    const params = new HttpParams()
      .set('id', contractId.toString());
    return this.getContractReportData('getReportDetails', params);
  }

  getPublicDetails(contractGuid: string): Promise<any> {
    const params = new HttpParams()
      .set('guid', contractGuid);
    return this.getContractReportData('publicGetReportDetails', params);
  }

  private getContractReportData(action: string, params: HttpParams): Promise<any> {
    return new Promise((resolve, reject) => {
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

  addContractNote(note: ContractNoteBaseModel, action = 'AddNote'): Observable<any> {
    return this.create(note, action);
  }

  getContractNotes(contractId: number, action = 'ReadAllNotes'): Observable<any> {
    return this.http.get(`${this.fullUrl}/${action}?contractId=${contractId}`);
  }

}
