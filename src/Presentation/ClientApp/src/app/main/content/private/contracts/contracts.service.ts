import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ContractOfficeSpaceModel } from '@app/core/models/contract/contract-office-space.model';
import { ContractItemCSVModel } from '@app/core/models/contract-item/contract-item-csv.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ContractsService extends BaseListService<ContractBaseModel> {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'contracts', http);
  }

  saveContractItem(contractItem: any, action = 'addContractItem'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, contractItem, { observe: 'response' })
      .pipe(map((out: any) => {
        return out;
      }));
  }

  updateContractItem(contractItem: any, action = 'updateContractItem'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, contractItem, { observe: 'response' })
      .pipe(map((out: any) => {
        return out;
      }));
  }

  getContractItem(id: number, action = 'getContractItem'): Observable<any> {
    return this.http.get(`${this.fullUrl}/${action}/${id}`);
  }

  getAllContractItems(
    contractId: number,
    filter = 'description', sortField = 'ASC',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    params: { [key: string]: any } = {},
    action = 'readAllContractItems'): Observable<any> {

    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.fullUrl}/${action}/` + contractId, {
      params: queryParams
    });
  }

  getAllRevenueSquareFeet(contractId: number, action = 'ReadAllRevenuesSquareFeet'): Observable<any> {
    const queryParams = new HttpParams()
      .set('contractId', contractId.toString());

    return this.http.get(`${this.fullUrl}/${action}`, {
      params: queryParams
    });
  }

  saveContractExpense(contractExpense: any, action = 'addContractExpense'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, contractExpense, { observe: 'response' })
      .pipe(map((out: any) => {
        return out;
      }));
  }

  updateContractExpense(contractExpense: any, action = 'updateContractExpense'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, contractExpense, { observe: 'response' })
      .pipe(map((out: any) => {
        return out;
      }));
  }

  getContractExpense(id: number, action = 'getContractExpense'): Observable<any> {
    return this.http.get(`${this.fullUrl}/${action}/${id}`);
  }

  getAllContractExpenses(
    contractId: number,
    filter = 'description', sortField = 'ASC',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    params: { [key: string]: any } = {},
    action = 'readAllContractExpenses'): Observable<any> {

    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.fullUrl}/${action}/` + contractId, {
      params: queryParams
    });
  }

  getAllAsListByBuilding(buildingId: number, date: any = null, action = 'ReadAllByBuildingCbo'): Observable<any> {
    const params = new HttpParams()
      .set('buildingId', buildingId.toString())
      .set('date', date === null ? null : date.toString());
    return this.http.get(`${this.fullUrl}/${action}`, { params: params });
  }

  getAllAsListByBuildingWithContract(buildingId: number, contractId: number = null, date: any = null, action = 'ReadAllByBuildingCbo'): Observable<any> {
    const params = new HttpParams()
      .set('buildingId', buildingId.toString())
      .set('date', date === null ? null : date.toString())
      .set('contractId', contractId.toString());
    return this.http.get(`${this.fullUrl}/${action}`, { params: params });
  }

  getAllAsListByCustomer(customerId: number, date: any = null, action = 'ReadAllByCustomerCbo'): Observable<any> {
    const params = new HttpParams()
      .set('customerId', customerId.toString())
      .set('date', date.toString());
    return this.http.get(`${this.fullUrl}/${action}`, { params: params });
  }

  getOfficeSpace(id: number, action = 'GetOfficeSpaceById'): Observable<any> {
    return this.http.get(`${this.fullUrl}/${action}/${id}`);
  }

  addOfficeSpace(officeSpace: ContractOfficeSpaceModel, action = 'AddOfficeSpace'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, officeSpace, { observe: 'response' })
      .pipe(map((out: any) => {
        return out;
      }));
  }

  updateOfficeSpace(officeSpace: ContractOfficeSpaceModel, action = 'UpdateOfficeSpace'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, officeSpace, { observe: 'response' })
      .pipe(map((out: any) => {
        return out;
      }));
  }

  getAllOfficeSpaces(
    contractId: number,
    filter = '', sortField = 'ASC',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    params: { [key: string]: any } = {},
    action = 'ReadAllOfficeSpaces'): Observable<any> {

    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.fullUrl}/${action}/` + contractId, {
      params: queryParams
    });

  }

  // Contract Revenue
  deleteContractItem(id: number, updatePrepopulatedItems: boolean = false, action: string = 'DeleteContractItem'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}?id=${id}&updatePrepopulated=${updatePrepopulatedItems.toString()}`, { observe: 'response' })
        .subscribe((response: any) => {
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  updateContractRevenueOrder(previousId, previousPosition, nextId, nextPosition, action = 'UpdateContractRevenueOrder'): Observable<any> {
    const body: {
      previousContractItemId: number,
      previousContractItemOrder: number,
      nextContractItemId: number,
      nextContractItemOrder: number
    } = {
      previousContractItemId: previousId,
      previousContractItemOrder: previousPosition,
      nextContractItemId: nextId,
      nextContractItemOrder: nextPosition
    };

    return this.update(body, action);
  }

  deleteContractExpense(id: number, action: string = 'DeleteContractExpense'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}?id=${id}`, { observe: 'response' })
        .subscribe((response: any) => {
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  validateAvailabilityContractNumber(contractNumber: string, contractId: number = -1, action = 'ValidateContractNumber'): Observable<any> {
    const queryParams = new HttpParams()
      .set('contractNumber', contractNumber)
      .set('contractId', contractId.toString());

    return this.http.get(`${this.fullUrl}/${action}`, { params: queryParams });
  }

  deleteContract(id: number, action: string = 'DeleteContract'): Promise<any> {
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

  getActiveContractBuilding(buildingId: number, contractId = -1, action = 'GetActiveContractFromBuilding'): Observable<any> {
    const queryParams = new HttpParams()
      .set('buildingId', buildingId.toString())
      .set('contractId', contractId.toString());

    return this.http.get(`${this.fullUrl}/${action}`, {
      params: queryParams
    });
  }

  createElementCSV(elementData): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(elementData, 'AddContractCSV')
        .subscribe(response => {
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  createElementItemCSV(elementData): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(elementData, 'AddItemCSV')
        .subscribe(response => {
          this.getElements();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  createContractItemCSV(elementData: ContractItemCSVModel): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(elementData, 'AddContractItemCSV')
        .subscribe(response => {
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  createElementExpenseCSV(elementData): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(elementData, 'AddContractExpenseCsv')
        .subscribe(response => {
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  addContractItemCsv(resource: ContractItemCSVModel, action = 'AddContractItemCSV'): Observable<any> {
    return this.http.post<{ errorCode: number, message: string }>(`${this.fullUrl}/${action}`, resource);
  }

  addContractExpenseCsv(resource, action = 'AddContractExpenseCsv'): Observable<any> {
    return this.http.post<{ errorCode: number, message: string }>(`${this.fullUrl}/${action}`, resource);
  }

  exportCsv(
    action = 'ExportContractToCsv',
    filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 100,
    params: { [key: string]: string } = {}
  ): Promise<any> {
    let extendedParams: { [key: string]: string } = {};
    extendedParams = this.extendParams(params, this.filterBy);

    // Default data source filter params
    let queryParams = new HttpParams()
      .set('filter', this.searchText ? this.searchText : '')
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, extendedParams);

    this.loadingSubject.next(true);
    return new Promise((resolve, reject) => {
      this.http.get(`${this.fullUrl}/${action}`, { params: queryParams, observe: 'response', responseType: 'text' })
        .subscribe((response: any) => {
          this.loadingSubject.next(false);
          resolve(response);
        },
          (error) => {
            this.loadingSubject.next(false);
            reject(error.error);
          });
    });
  }

  // Activity Log Notes
  getActivityLogNotes(id: number, action = 'ReadAllActivityLogNotes'): Observable<any> {
    const params = new HttpParams()
      .set('id', id.toString());
    return this.http.get(`${this.fullUrl}/${action}/`, { params: params });
  }

  deleteActivityLogNote(id: number, action = 'RemoveActivityLogNote'): Observable<any> {
    return this.http.delete(`${this.fullUrl}/${action}?id=${id}`);
  }
}
