import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { ExpenseBaseModel } from '@app/core/models/expense/expense-base.model';
import { Observable } from 'rxjs';
import { ExpensesService } from './expenses.service';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class ExpensesRevolver implements Resolve<ExpenseBaseModel>{

  constructor(private expenseService: ExpensesService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.expenseService.validateModuleAccess(ApplicationModule.Expenses);
    return this.expenseService.getElements();
  }

}
