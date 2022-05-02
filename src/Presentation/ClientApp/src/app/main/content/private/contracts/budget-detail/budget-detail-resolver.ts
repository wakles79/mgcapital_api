import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { BudgetDetailService } from './budget-detail.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class BudgetDetailResolver implements Resolve<any>
{
  constructor(private budgetService: BudgetDetailService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    const contractId = route.params.id;
    this.budgetService.validateModuleAccess(ApplicationModule.Budgets);

    return this.budgetService.getDetails(contractId);
  }
}
