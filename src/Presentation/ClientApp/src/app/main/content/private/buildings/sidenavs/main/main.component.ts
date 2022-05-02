import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { AuthService } from '@app/core/services/auth.service';
import { fuseAnimations } from '@fuse/animations';
import { Subject, Subscription } from 'rxjs';
import { CustomersService } from '../../../customers/customers.service';
import { BuildingsService } from '../../buildings.service';

@Component({
  selector: 'app-main-sidebar',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss'],
  animations: fuseAnimations
})
export class MainComponent implements OnInit, OnDestroy {

  filterActive: string;
  filterBy: { [key: string]: string } = {};

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  customers: ListItemModel[] = [];
  filteredCustomers: Subject<any[]> = new Subject<any[]>();
  listCustomersSubscription: Subscription;

  constructor(
    private buildingService: BuildingsService,
    private authService: AuthService,
    public snackBar: MatSnackBar,
    private customerService: CustomersService,
  ) {
    this.filterActive = 'all';
    this.filterBy = {};
  }

  ngOnInit(): void {
    this.getCustomers();
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();

    if (this.listCustomersSubscription && !this.listCustomersSubscription.closed) {
      this.listCustomersSubscription.unsubscribe();
    }
  }

  changeFilter(filter: any): void {
    if (filter === 'all') {
      this.filterActive = 'all';
      this.filterBy['isAvailable'] = '';
      this.filterBy['isActive'] = '';
      this.buildingService.onFilterChanged.next(this.filterBy);
    }

    if (filter === 'active') {
      this.filterActive = 'active';
      this.filterBy['isAvailable'] = '';
      this.filterBy['isActive'] = '1';
      this.buildingService.onFilterChanged.next(this.filterBy);
    }

    if (filter === 'inactive') {
      this.filterActive = 'inactive';
      this.filterBy['isAvailable'] = '';
      this.filterBy['isActive'] = '0';
      this.buildingService.onFilterChanged.next(this.filterBy);
    }

    if (filter === 'notAvailable') {
      this.filterActive = 'notAvailable';
      this.filterBy['isAvailable'] = '0';
      this.filterBy['isActive'] = '';
      this.buildingService.onFilterChanged.next(this.filterBy);
    }

  }

  /** CUSTOMERS */
  getCustomers(): void {
    const idCustomer = null;

    if (this.listCustomersSubscription && this.listCustomersSubscription.closed) {
      this.listCustomersSubscription.unsubscribe();
    }

    this.listCustomersSubscription = this.customerService.getAllAsList('readallcbo', '', 0, 99999, idCustomer)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.customers = response.payload;
        this.filteredCustomers.next(this.customers);
      });
  }

  changeCustomer(customerId: number): void {
    this.filterBy['customerId'] = customerId.toString();

    this.buildingService.onFilterChanged.next(this.filterBy);
  }
}
