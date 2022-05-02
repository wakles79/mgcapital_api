import { Resolve, RouterStateSnapshot, ActivatedRouteSnapshot, Route } from '@angular/router';
import { ProposalDetailModel } from '@app/core/models/proposal/proposal-detail.model';
import { ProposalDetailService } from './proposal-detail.service';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class ProposalDetailResolver implements Resolve<ProposalDetailModel> {

  constructor(private proposalDetailService: ProposalDetailService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.proposalDetailService.validateModuleAccess(ApplicationModule.Proposals);
    const id = route.params.id;
    return this.proposalDetailService.getDetails(id);
  }

}
