import { Injectable, Inject } from '@angular/core';
import { DataService } from '@app/core/services/data.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { EntityModel } from '@core/models/common/entity.model';
import * as moment from 'moment';
import { BehaviorSubject, Subject, Subscription, Observable } from 'rxjs';
import { AccessType, ApplicationModule } from '../models/company-settings/company-settings-base.model';
import { PermissionAssignmentModel } from '../models/permission/permission-assignment.model';

export class BaseListService<ElementModel extends EntityModel> extends DataService {

  allElementsChanged: BehaviorSubject<any> = new BehaviorSubject([]);
  elementChanged: BehaviorSubject<any> = new BehaviorSubject([]);
  loadingSubject = new BehaviorSubject<boolean>(false);
  elementsCount = 0;

  selectedElementsChanged: BehaviorSubject<any> = new BehaviorSubject([]);
  selectedElements: number[] = [];

  allElementsToList: ElementModel[] = [];
  searchText: string;
  searchTextChanged: Subject<any> = new Subject();

  viewName = null;
  onFilterChanged: Subject<any> = new Subject();
  filterBy: { [key: string]: any } = {};

  dateFrom = moment().add(-1, 'd').toDate();
  dateTo = moment().toDate();
  onDatesChanges: Subject<any> = new Subject();

  selectedCustomer: any = {};
  onCustomerChanged: Subject<any> = new Subject();

  permissions: PermissionAssignmentModel[] = [];
  onPermissionsChanged = new Subject<PermissionAssignmentModel[]>();

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    componentUrl: string, http: HttpClient) {
    super(apiBaseUrl, componentUrl, http);

    this.searchTextChanged.subscribe((searchText: string) => {
      this.searchText = searchText;
      this.getElements();
    });

    this.onFilterChanged.subscribe((filterBy: { [key: string]: string }) => {
      this.filterBy = filterBy;
      this.getElements();

      this.saveFiltersLocally(this.filterBy);
    });

    this.onDatesChanges.subscribe((dates: any) => {
      this.dateFrom = dates['dateFrom'];
      this.dateTo = dates['dateTo'];
    });

    this.onCustomerChanged.subscribe((element: any) => {
      this.selectedCustomer = element;
    });
  }

  getElements(
    action = 'readall',
    filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    params: { [key: string]: any } = {}
  ): any {
    if (!filter || filter === '') {
      filter = this.searchText;
    }

    this.loadingSubject.next(true);

    let extendedParams: { [key: string]: string } = {};
    extendedParams = this.extendParams(params, this.filterBy);

    return this.getAll(action, filter, sortField, sortOrder, pageNumber, pageSize, extendedParams)
      .subscribe((response: { count: number, payload: ElementModel[] }) => {
        this.allElementsToList = response.payload;
        this.elementsCount = response.count;
        this.allElementsChanged.next(this.allElementsToList);
      },
        (error) => {
          this.loadingSubject.next(false);
        },
        () => {
          this.loadingSubject.next(false);
        }
      );
  }

  extendParams(
    params: { [key: string]: any },
    extraParams: { [key: string]: any } = {}): { [key: string]: any } {
    if (extraParams) {
      for (const key in extraParams) {
        if (extraParams.hasOwnProperty(key)) {
          const value = extraParams[key];
          params[key] = value;
        }
      }
    }
    return params;
  }

  updateElement(element, action = 'update'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.update(element, action)
        .subscribe(response => {
          this.getElements();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  createElement(elementData): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(elementData)
        .subscribe(response => {
          this.getElements();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  delete(elementToDelete, action = 'delete'): Promise<any> {
    const element = this.allElementsToList.find(e => e.id === elementToDelete.id);

    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}?id=${element.id}`, { observe: 'response' })
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

  deleteElement(elementToDelete, action = 'delete'): void {
    const element = this.allElementsToList.find(e => e.id === elementToDelete.id);
    const elementIndex = this.allElementsToList.indexOf(element);

    this.delete(element.id, action)
      .then((r: any) => {
        this.allElementsToList.splice(elementIndex, 1);
        this.allElementsChanged.next(this.allElementsToList);
        this.elementsCount = this.allElementsToList.length;
      });
  }
  /**
   * Toggle selected element by id
   * @param id
   */
  toggleSelectedElement(guid): void {
    // First, check if we already have that element as selected...
    if (this.selectedElements.length > 0) {
      const index = this.selectedElements.indexOf(guid);

      if (index !== -1) {
        this.selectedElements.splice(index, 1);

        // Trigger the next event
        this.selectedElementsChanged.next(this.selectedElements);

        return;
      }
    }
    // If we don't have it, push as selected
    this.selectedElements.push(guid);

    // Trigger the next event
    this.selectedElementsChanged.next(this.selectedElements);
  }

  /**
   * Toggle select all
   */
  toggleSelectAll(): void {
    if (this.selectedElements.length > 0) {
      this.deselectElements();
    }
    else {
      this.getSelectedElements();
    }
  }

  getSelectedElements(filterParameter?, filterValue?): void {
    this.selectedElements = [];

    // If there is no filter, select all
    if (filterParameter === undefined || filterValue === undefined) {
      this.allElementsToList.map(element => {
        this.selectedElements.push(element.id);
      });
    }
    // Trigger the next event
    this.selectedElementsChanged.next(this.selectedElements);
  }

  deselectElements(): void {
    this.selectedElements = [];

    // Trigger the next event
    this.selectedElementsChanged.next(this.selectedElements);
  }

  deleteSelectedElements(): void {
    for (const elementId of this.selectedElements) {
      const element = this.allElementsToList.find(_element =>
        _element.id === elementId
      );
      this.delete(element.id);
      const elementIndex = this.allElementsToList.indexOf(element);
      this.allElementsToList.splice(elementIndex, 1);
    }
    this.allElementsChanged.next(this.allElementsToList);
    this.deselectElements();
  }

  /** Validate Module Access */
  validateModuleAccess(appModule: ApplicationModule, controller: string = 'CompanySettings', action: string = 'GetModuleAccess'): any {
    const pars = new HttpParams()
      .set('module', appModule.toString());

    localStorage.setItem('readOnly', 'validating');
    return this.http.get(`${this.apiBaseUrl}api/${controller}/${action}`, { params: pars })
      .subscribe((result: AccessType) => {
        switch (result) {
          case AccessType.None:
            localStorage.removeItem('readOnly');
            break;
          case AccessType.Full:
            localStorage.removeItem('readOnly');
            break;
          case AccessType.ReadOnly:
            localStorage.setItem('readOnly', 'true');
            break;
        }
      });
  }

  getModulePermissions(appModule: ApplicationModule, controller: string = 'CompanySettings', action: string = 'GetModulePermissions'): void {
    const pars = new HttpParams()
      .set('module', appModule.toString());

    this.http.get(`${this.apiBaseUrl}api/${controller}/${action}`, { params: pars })
      .subscribe((permissions: any) => {
        this.permissions = permissions;
        this.onPermissionsChanged.next(permissions);
      });

  }

  // Load Session Filter
  loadSessionFilter(): void {
    this.filterBy = {};
    const filters: any[] = JSON.parse(localStorage.getItem('filters'));
    const index = filters.findIndex(v => v.view === this.viewName);
    if (index >= 0) {
      this.filterBy = filters[index].filter;
    }
  }

  // Save filter to local storage
  saveFiltersLocally(filterBy: { [key: string]: string }): void {
    if (this.viewName != null) {
      const newFilters = { view: this.viewName, filter: filterBy };
      const filters: any[] = JSON.parse(localStorage.getItem('filters'));
      const index = filters.findIndex(v => v.view === this.viewName);
      if (index >= 0) {
        filters[index] = newFilters;
      } else {
        filters.push(newFilters);
      }
      localStorage.setItem('filters', JSON.stringify(filters));
    }
  }
}
