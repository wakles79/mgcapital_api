import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ProposalDetailModel } from '@app/core/models/proposal/proposal-detail.model';
import { BaseListService } from '@app/core/services/base-list.service';
import { BehaviorSubject, Observable } from 'rxjs';
import { ProposalSendEmailModel } from '@app/core/models/proposal/proposal-send-email.model';

@Injectable({
  providedIn: 'root'
})
export class ProposalDetailService extends BaseListService<ProposalDetailModel> {

  proposalDetail: ProposalDetailModel;
  onProposalDetailChanged: BehaviorSubject<ProposalDetailModel> = new BehaviorSubject<ProposalDetailModel>(null);

  proposalServices: any[] = [];
  onProposalServicesChanged: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient
  ) {
    super(apiBaseUrl, 'proposals', http);
  }

  getDetails(proposalId: number): Promise<any> {
    const params = new HttpParams()
      .set('id', proposalId.toString());
    return this.getProposalDetailDate('getReportDetails', params);
  }

  getPublicDetails(proposalGuid: string): Promise<any> {
    const params = new HttpParams()
      .set('guid', proposalGuid);
    return this.getProposalDetailDate('publicGetReportDetails', params);
  }

  private getProposalDetailDate(action: string, params: HttpParams): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.get(`${this.fullUrl}/${action}`, { params: params })
        .subscribe((response: any) => {
          resolve(response);

          this.proposalDetail = response;
          this.onProposalDetailChanged.next(this.proposalDetail);

          this.proposalServices = this.proposalDetail.proposalServices || [];
          this.onProposalServicesChanged.next(this.proposalServices);

        }, (error) => { reject(error); });
    });
  }

  setStatus(id: number, status: number, billToName: string, billToEmail: string, billTo = -1, action = 'changeStatus'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.post(`${this.fullUrl}/${action}?id=` + id + `&status=` + status + `&billToName=` + billToName + `&billToEmail=` + billToEmail + `&billTo=` + billTo, { observe: 'response' })
        .subscribe(response => {
          resolve(response);
          this.getDetails(id);
        },
          (error) => {
            reject(error);
          });
    });
  }

  sendByEmailProposalLink(proposalToSend: ProposalSendEmailModel): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.post(`${this.fullUrl}/sendProposalReport`, proposalToSend, { observe: 'response' })
        .subscribe((response: any) => {
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  getProposalService(id: number): Observable<any> {
    return this.get(id, 'GetProposalService');
  }

  addProposalService(proposalService: any): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(proposalService, 'AddProposalService')
        .subscribe(response => {
          resolve(response);
          this.getDetails(proposalService.proposalId);
        },
          (error) => {
            reject(error);
          });
    });

  }

  updateProposalService(proposalService: any): Promise<any> {
    return new Promise((resolve, reject) => {
      this.update(proposalService, 'UpdateProposalService')
        .subscribe(response => {
          resolve(response);
          this.getDetails(proposalService.proposalId);
        },
          (error) => {
            reject(error);
          });
    });
  }

  getAllProposalServices(
    contractId: number,
    filter = 'id', sortField = 'ASC',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    params: { [key: string]: any } = {},
    action = 'ReadAllProposalServices'
  ): Observable<any> {

    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.fullUrl}/${action}/` + contractId, {
      params: queryParams
    });
  }

}
