import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { ProposalBaseModel } from '@app/core/models/proposal/proposal-base.model';
import { ProposalsService } from './proposals.service';
import { Observable } from 'rxjs';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class ProposalsResolver implements Resolve<ProposalBaseModel> {

  constructor(private proposalService: ProposalsService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.proposalService.validateModuleAccess(ApplicationModule.Proposals);
    return this.proposalService.getElements();
  }

}
