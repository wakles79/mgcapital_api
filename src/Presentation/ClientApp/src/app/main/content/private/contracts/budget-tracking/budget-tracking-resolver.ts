import { Injectable } from '@angular/core';
import { ContractReportDetailsModel } from '@app/core/models/contract/contract-report-detail.model';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { BudgetTrackingService } from './budget-tracking.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BudgetTrackingResolver implements Resolve<ContractReportDetailsModel> {

  constructor(private budgetTrackingService: BudgetTrackingService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    const budgetId = route.params.id;
    return this.budgetTrackingService.getDetails(budgetId);
  }

}
