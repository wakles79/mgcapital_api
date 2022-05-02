import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Injectable, Inject } from '@angular/core';
import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';
import { Observable } from 'rxjs';
import { ContractsService } from './contracts.service';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class ContractsResolver implements Resolve<ContractBaseModel>
{
  constructor(private contractService: ContractsService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.contractService.validateModuleAccess(ApplicationModule.Budgets);

    this.contractService.viewName = 'contract-list';
    this.contractService.loadSessionFilter();
    return this.contractService.getElements();
  }
}
