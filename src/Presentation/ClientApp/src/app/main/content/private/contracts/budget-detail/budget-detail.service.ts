import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { ContractDetailModel } from '@app/core/models/contract/contract-detail.model';
import { BehaviorSubject, Observable } from 'rxjs';
import { ContractItemGridModel } from '@app/core/models/contract-item/contract-item-grid.model';
import { ContractExpenseGridModel } from '@app/core/models/contract-expense/contract-expense-grid.model';
import { ContractNoteGridModel } from '@app/core/models/contract/contract-note-grid.model';
import { ContractActivityLogGridModel } from '@app/core/models/contract/contract-activity-log-grid.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ContractNoteBaseModel } from '@app/core/models/contract/contract-note-base.model';

@Injectable({
  providedIn: 'root'
})
export class BudgetDetailService extends BaseListService<any> {

  budgetDetail: ContractDetailModel;
  onbudgetDetailChanged: BehaviorSubject<ContractDetailModel> = new BehaviorSubject<ContractDetailModel>(null);

  estimatedRevenue: ContractItemGridModel[] = [];
  onEstimatedRevenueChanged: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  estimatedExpenses: ContractExpenseGridModel[] = [];
  onEstimatedExpensesChanged: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  notes: ContractNoteGridModel[] = [];
  onNotesChanged: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  activityLog: ContractActivityLogGridModel[] = [];
  onActivityLogChanged: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'contracts', http);
  }

  getDetails(id: number): Promise<any> {
    const params = new HttpParams()
      .set('id', id.toString());

    return this.getBudgetDetails('GetBudgetDetails', params);
  }

  private getBudgetDetails(action: string, params: HttpParams): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.get(`${this.fullUrl}/${action}`, { params: params })
        .subscribe((response: any) => {
          resolve(response);

          this.budgetDetail = response.budgetDetail;
          this.onbudgetDetailChanged.next(this.budgetDetail);

          this.estimatedRevenue = response.estimatedRevenues;
          this.onEstimatedRevenueChanged.next(this.estimatedRevenue);

          this.estimatedExpenses = response.estimatedExpenses;
          this.onEstimatedExpensesChanged.next(this.estimatedExpenses);

          this.notes = response.notes;
          this.onNotesChanged.next(this.notes);

          this.activityLog = response.activityLog;
          this.onActivityLogChanged.next(this.activityLog);

        }, (error) => { reject(error); });
    });
  }

  // Notes
  addContractNote(note: ContractNoteBaseModel, action = 'AddNote'): Observable<any> {
    return this.create(note, action);
  }

  getContractNotes(contractId: number, action = 'ReadAllNotes'): Observable<any> {
    return this.http.get(`${this.fullUrl}/${action}?contractId=${contractId}`);
  }

}
