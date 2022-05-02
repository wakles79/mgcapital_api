import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { ExpenseTypeBaseModel } from '@app/core/models/expense-type/expense-type-base.model';
import { Observable } from 'rxjs';
import { ExpensesTypesService } from './expenses-types.service';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class ExpensesTypesResolver implements Resolve<ExpenseTypeBaseModel>
{
  constructor(private expenseTypeService: ExpensesTypesService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.expenseTypeService.validateModuleAccess(ApplicationModule.ExpensesTypesCatalog);
    return this.expenseTypeService.onFilterChanged.next({});
  }
}
