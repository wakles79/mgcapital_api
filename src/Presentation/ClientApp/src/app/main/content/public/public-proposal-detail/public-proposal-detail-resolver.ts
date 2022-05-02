import { ProposalDetailModel } from '@app/core/models/proposal/proposal-detail.model';
import { ProposalDetailService } from '../../private/proposals/proposal-detail/proposal-detail.service';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PublicProposalDetailResolver implements Resolve<ProposalDetailModel>{

  constructor(private proposalDetailService: ProposalDetailService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    const proposalGuid = route.params.guid;
    return this.proposalDetailService.getPublicDetails(proposalGuid);
  }

}
