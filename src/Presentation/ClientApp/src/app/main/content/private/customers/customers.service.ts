import { Inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { CustomersBaseService } from '@app/core/services/customers.service';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { CustomerBaseModel } from '@app/core/models/customer/customer-base.model';
import { CustomerGridModel } from '@app/core/models/customer/customer-grid.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AccessType, ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';


@Injectable({
  providedIn: 'root'
})
export class CustomersService extends CustomersBaseService implements Resolve<any> {

  onCustomersChanged: BehaviorSubject<any> = new BehaviorSubject([]);
  onSelectedCustomersChanged: BehaviorSubject<any> = new BehaviorSubject([]);
  onCustomerDataChanged: BehaviorSubject<any> = new BehaviorSubject([]);
  onSearchTextChanged: Subject<any> = new Subject();
  onFilterChanged: Subject<any> = new Subject();
  loadingSubject = new BehaviorSubject<boolean>(false);

  customersCount = 0;
  customers: CustomerGridModel[];
  customer: CustomerBaseModel;
  selectedCustomers: string[] = [];

  searchText: string;
  filterBy: string;

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, http);
  }

  /**
   * The Customers App Main Resolver
   * @param {ActivatedRouteSnapshot} route
   * @param {RouterStateSnapshot} state
   * @returns {Observable<any> | Promise<any> | any}
   */
  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {
    this.validateModuleAccess(ApplicationModule.ManagementCo);
    return new Promise((resolve, reject) => {

      Promise.all([
        this.getCustomers()
      ]).then(
        ([files]) => {

          this.onSearchTextChanged.subscribe(searchText => {
            this.searchText = searchText;
            this.getCustomers();
          });

          this.onFilterChanged.subscribe(filter => {
            this.filterBy = filter;
            this.getCustomers();
          });

          resolve();

        },
        reject
      );
    });
  }

  getCustomers(
    filter = '', sortField = '', sortOrder = '', pageNumber = 0, pageSize = 100
  ): Promise<any> {
    if (!filter || filter === '') {
      filter = this.searchText;
    }
    this.loadingSubject.next(true);
    return new Promise((resolve, reject) => {
      this.getAll('readall', filter, sortField, sortOrder, pageNumber, pageSize)
        .subscribe(
          (response: any) => {

            this.customers = response.payload;
            this.customersCount = response.count;

            // if (this.searchText && this.searchText !== '') {
            //   this.customers = FuseUtils.filterArrayByString(this.customers, this.searchText);
            // }

            this.customers = this.customers.map(customer => {
              return new CustomerGridModel(customer);
            });

            this.onCustomersChanged.next(this.customers);
            resolve(this.customers);
          },
          (error) => {
            this.loadingSubject.next(false);
            resolve(error);
          },
          () => {
            this.loadingSubject.next(false);
          });
    }
    );
  }

  /**
   * Toggle selected customer by id
   * @param id
   */
  toggleSelectedCustomer(guid): void {
    // First, check if we already have that todo as selected...
    if (this.selectedCustomers.length > 0) {
      const index = this.selectedCustomers.indexOf(guid);

      if (index !== -1) {
        this.selectedCustomers.splice(index, 1);

        // Trigger the next event
        this.onSelectedCustomersChanged.next(this.selectedCustomers);

        // Return
        return;
      }
    }

    // If we don't have it, push as selected
    this.selectedCustomers.push(guid);

    // Trigger the next event
    this.onSelectedCustomersChanged.next(this.selectedCustomers);
  }

  /**
   * Toggle select all
   */
  toggleSelectAll(): void {
    if (this.selectedCustomers.length > 0) {
      this.deselectCustomers();
    }
    else {
      this.selectCustomers();
    }
  }

  selectCustomers(filterParameter?, filterValue?): void {
    this.selectedCustomers = [];

    // If there is no filter, select all todos
    if (filterParameter === undefined || filterValue === undefined) {
      this.selectedCustomers = [];
      this.customers.map(customer => {
        this.selectedCustomers.push(customer.guid);
      });
    }
    else {
      /* this.selectedCustomers.push(...
           this.customers.filter(todo => {
               return todo[filterParameter] === filterValue;
           })
       );*/
    }

    // Trigger the next event
    this.onSelectedCustomersChanged.next(this.selectedCustomers);
  }

  updateCustomer(customer): Promise<any> {
    return new Promise((resolve, reject) => {
      this.update(customer)
        .subscribe(response => {
          this.getCustomers();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  updateCustomerData(customerData): Promise<any> {
    return new Promise((resolve, reject) => {
      this.update(customerData)
        .subscribe(response => {
          this.getCustomers();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  createCustomer(customerData): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(customerData)
        .subscribe(response => {
          this.getCustomers();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  deselectCustomers(): void {
    this.selectedCustomers = [];

    // Trigger the next event
    this.onSelectedCustomersChanged.next(this.selectedCustomers);
  }

  deleteCustomer(customer): Promise<any> {
    const customerIndex = this.customers.indexOf(customer);
    return this.delete(customer.guid)
      .then((r: any) => {
        this.customers.splice(customerIndex, 1);
        this.onCustomersChanged.next(this.customers);
      });
  }

  deleteSelectedCustomers(): void {
    for (const customerGuid of this.selectedCustomers) {
      const customer = this.customers.find(_customer => {
        return _customer.guid === customerGuid;
      });
      this.delete(customer.guid);
      const customerIndex = this.customers.indexOf(customer);
      this.customers.splice(customerIndex, 1);
    }
    this.onCustomersChanged.next(this.customers);
    this.deselectCustomers();
  }

  exportCsv(
    action = 'CustomerReportCsv',
    filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 100,
    params: { [key: string]: string } = {}
  ): Promise<any> {
    if (!filter || filter === '') {
      filter = this.searchText;
    }

    const queryParams = new HttpParams()
      .set('filter', this.searchText ? this.searchText : '')
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

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
    }
    );
  }

  deleteByGuid(guid: string, action = 'DeleteByGuid'): Observable<any> {
    const pars = new HttpParams()
      .set('guid', guid);

    return this.http.delete(`${this.fullUrl}/${action}`, { params: pars });
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
}
