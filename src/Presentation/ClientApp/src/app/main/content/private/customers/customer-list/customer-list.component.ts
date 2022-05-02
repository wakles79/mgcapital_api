import { DataSource } from '@angular/cdk/table';
import { AfterViewInit, Component, OnInit, ViewEncapsulation, OnDestroy, ViewChild, TemplateRef, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { CustomerBaseModel } from '@app/core/models/customer/customer-base.model';
import { CustomerGridModel } from '@app/core/models/customer/customer-grid.model';
import { CustomerFormComponent } from '@app/core/modules/customer-form/customer-form.component';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { merge, Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { CustomersService } from '../customers.service';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class CustomerListComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;


  get customersCount(): any { return this.customersService.customersCount; }
  customers: CustomerGridModel[];
  customer: CustomerBaseModel;
  dataSource: CustomersDataSource | null;
  displayedColumns = ['checkbox', 'code', 'name', 'phone', 'address', 'contactsTotal', 'buttons'];
  selectedCustomers: any[];
  checkboxes: {};

  onCustomersChangedSubscription: Subscription;
  onSelectedCustomersChangedSubscription: Subscription;
  onCustomerDataChangedSubscription: Subscription;

  loading$ = this.customersService.loadingSubject.asObservable();

  dialogRef: any;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  @Input()
  searchInput: FormControl;
  @Input() readOnly: boolean;

  constructor(
    private customersService: CustomersService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {
    this.onCustomersChangedSubscription =
      this.customersService.onCustomersChanged.subscribe(customers => {

        this.customers = customers;

        this.checkboxes = {};
        customers.map(customer => {
          this.checkboxes[customer.guid] = false;
        });
      });

    this.onSelectedCustomersChangedSubscription =
      this.customersService.onSelectedCustomersChanged.subscribe(selectedCustomers => {
        for (const guid in this.checkboxes) {
          if (!this.checkboxes.hasOwnProperty(guid)) {
            continue;
          }

          this.checkboxes[guid] = selectedCustomers.includes(guid);
        }
        this.selectedCustomers = selectedCustomers;
      });

    this.onCustomerDataChangedSubscription =
      this.customersService.onCustomerDataChanged.subscribe(customer => {
        this.customer = customer;
      });

  }

  ngOnInit(): void {
    this.searchInput.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.customersService.onSearchTextChanged.next(searchText);
      });
    this.dataSource = new CustomersDataSource(this.customersService);
  }

  ngAfterViewInit(): void {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.customersService.getCustomers(
          '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize))
      )
      .subscribe();

  }

  ngOnDestroy(): void {
    this.onCustomersChangedSubscription.unsubscribe();
    this.onSelectedCustomersChangedSubscription.unsubscribe();
    this.onCustomerDataChangedSubscription.unsubscribe();
  }

  editCustomer(customer): void {
    this.customersService.get(customer.guid, 'update')
      .subscribe((customerData: any) => {
        if (customerData) {
          const customerUpdateObj = new CustomerBaseModel(customerData);
          this.dialogRef = this.dialog.open(CustomerFormComponent, {
            panelClass: 'customer-form-dialog',
            data: {
              customer: customerUpdateObj,
              action: 'edit'
            }
          });

          this.dialogRef.afterClosed()
            .subscribe(response => {
              if (!response) {
                return;
              }
              const actionType: string = response[0];
              const formData: FormGroup = response[1];
              const updatedCustomerObj = new CustomerBaseModel(formData.getRawValue());
              switch (actionType) {
                /**
                 * Save
                 */
                case 'save':

                  this.customersService.updateCustomer(updatedCustomerObj)
                    .then(
                      () => this.snackBar.open('Customer updated successfully!!!', 'close', { duration: 1000 }),
                      () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
                    .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

                  break;
                /**
                 * Delete
                 */
                case 'delete':

                  this.deleteCustomer(customerUpdateObj);

                  break;
              }
            });
        } else {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      },
        (error) => {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });

  }


  /**
   * Delete Customer
   */
  deleteCustomer(customer): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.customersService.deleteCustomer(customer).then(
          () => this.snackBar.open('Proposal delete successfully!!!', 'close', { duration: 1000 }),
          () => this.snackBar.open('Oops,  This customer is not allowed to remove', 'close', { duration: 3000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      }
      this.confirmDialogRef = null;
    });

  }

  onSelectedChange(customerGuid): void {
    this.customersService.toggleSelectedCustomer(customerGuid);
  }

}

export class CustomersDataSource extends DataSource<any>
{
  constructor(private customersService: CustomersService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.customersService.onCustomersChanged;
  }

  disconnect(): void {
  }
}
