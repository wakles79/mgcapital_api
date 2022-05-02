import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BuildingBaseModel } from '@app/core/models/building/building-base.model';
import { ContactBaseModel } from '@app/core/models/contact/contact-base.model';
import { ListUserModel } from '@app/core/models/user/list-users.model';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { ContactsService } from '../../contacts/contacts.service';
import { CustomersService } from '../../customers/customers.service';
import { UsersBaseService } from '../../users/users-base.service';
import { BuildingsProfileService } from '../buildings-profile.service';
import { Location } from '@angular/common';
import { CustomerBaseModel } from '@app/core/models/customer/customer-base.model';

@Component({
  selector: 'app-building-profile-detail',
  templateUrl: './building-profile-detail.component.html',
  styleUrls: ['./building-profile-detail.component.scss']
})
export class BuildingProfileDetailComponent implements OnInit {

  loading$ = new BehaviorSubject<boolean>(false);
  action: string;
  buildingForm: FormGroup;
  buildingAddress = '';
  customer: CustomerBaseModel;
  subcontractors: any[] = [];
  roleLevelLoggedUser: any;
  public dialogRef: MatDialogRef<BuildingProfileDetailComponent>;

  operationsManagers: ListUserModel[] = [];
  filteredOperationsManagers: Subject<any[]> = new Subject<any[]>();
  listOperationsManagersSubscription: Subscription;

  listCustomersSubscription: Subscription;

  selectedSupervisors: any[] = [];
  supervisors: any[] = [];
  filteredSupervisors: any[] = [];
  filteredManager: any[] = [];
  listSupervisorsSubscription: Subscription;

  contacts: ContactBaseModel[] = [];
  filteredCustomers: Subject<any[]> = new Subject<any[]>();
  building: BuildingBaseModel;

  buildingDataChangedSubscription: Subscription;

  contador: number;
  hola: string;
  dataString: string;

  Magment: any[];
  constructor(
    private BuildDetailService: BuildingsProfileService,
    private location: Location,
    private customerService: CustomersService) {
    this.loading$.next(true);
    this.buildingDataChangedSubscription = this.BuildDetailService.onBuildingDetailChanged
      .subscribe((BuildingDetailData) => {
        this.building = BuildingDetailData;
        if (this.building.address !== {}) {
          this.buildingAddress = this.building.address.fullAddress;
        }
        this.supervisors = this.building.employees;
        this.loading$.next(false);
      });

  }

  goBack(): void {
    this.location.back();
  }

  ngOnInit(): void {
    this.getContacts();
    this.filterEmployees();
    this.getcustomers();
  }

  filterEmployees(): void {
    this.supervisors.forEach(element => {
      if (element.type === 1) {
        this.filteredSupervisors.push(element);
      }
    });
    this.supervisors.forEach(element => {
      if (element.type === 2) {
        this.filteredManager.push(element);
      }
    });
  }

  getContacts(): void {
    this.BuildDetailService.getContactsByBuilding(this.building.id)
      .subscribe((contacts: ContactBaseModel[]) => {
        this.contacts = contacts;
      });
  }

  getcustomers(): void {
    const idCustomer = this.building.customerId;
    this.listCustomersSubscription = this.customerService.get(idCustomer)
      .subscribe((costumer: CustomerBaseModel) => {
        this.customer = costumer;
      });
  }

}
