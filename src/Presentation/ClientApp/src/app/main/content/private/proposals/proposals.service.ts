import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ProposalBaseModel } from '@app/core/models/proposal/proposal-base.model';
import { BaseListService } from '@app/core/services/base-list.service';

@Injectable({
  providedIn: 'root'
})
export class ProposalsService extends BaseListService<ProposalBaseModel>  {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient
  ) {
    super(apiBaseUrl, 'proposals', http);
  }

}
