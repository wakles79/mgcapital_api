import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { ExpenseGridModel } from '@app/core/models/expense/expense-grid.model';

@Injectable({
  providedIn: 'root'
})
export class ExpensesService extends BaseListService<ExpenseGridModel> {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient
  ) {
    super(apiBaseUrl, 'expenses', http);
  }

  deleteExpense(id: number, action = 'delete'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}?id=${id}`, { observe: 'response' })
        .subscribe((response: any) => {
          if (response.status === 200) {
            this.getElements();
          }
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  createElementCSV(elementData): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(elementData, 'AddCSV')
        .subscribe(response => {
          // this.getElements();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  deleteSelectedElements(): void {
    for (const elementId of this.selectedElements) {
      const element = this.allElementsToList.find(_element =>
        _element.id === elementId
      );
      this.delete(element);
      const elementIndex = this.allElementsToList.indexOf(element);
      this.allElementsToList.splice(elementIndex, 1);
    }
    this.allElementsChanged.next(this.allElementsToList);
    this.deselectElements();
  }

  getSelectedElements(filterParameter?, filterValue?): void {
    this.selectedElements = [];

    // If there is no filter, select all
    if (filterParameter === undefined || filterValue === undefined) {
      this.selectedElements = [];
      this.allElementsToList.map(element => {
        this.selectedElements.push(element.id);
      });
    }
    // Trigger the next event
    this.selectedElementsChanged.next(this.selectedElements);
  }

  /**
   * Toggle selected element by id
   * @param id
   */
  toggleSelectedElement(id): void {
    // First, check if we already have that element as selected...
    if (this.selectedElements.length > 0) {
      const index = this.selectedElements.indexOf(id);

      if (index !== -1) {
        this.selectedElements.splice(index, 1);

        // Trigger the next event
        this.selectedElementsChanged.next(this.selectedElements);

        return;
      }
    }
    // If we don't have it, push as selected
    this.selectedElements.push(id);

    // Trigger the next event
    this.selectedElementsChanged.next(this.selectedElements);
  }
}
