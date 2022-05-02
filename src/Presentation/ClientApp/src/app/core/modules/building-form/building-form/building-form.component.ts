import { Component, Inject, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, AbstractControl, Validators } from '@angular/forms';
import { fuseAnimations } from '@fuse/animations';
import { BuildingBaseModel } from '@app/core/models/building/building-base.model';
import { AddressModel } from '@app/core/models/common/address.model';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { forkJoin, from, Subject, Subscription } from 'rxjs';
import { ListUserModel } from '@app/core/models/user/list-users.model';
import { ContactBaseModel } from '@app/core/models/contact/contact-base.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { AddressFormComponent } from '../../contact-form/address-form/address-form.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DomSanitizer } from '@angular/platform-browser';
import { AuthService } from '@app/core/services/auth.service';
import { ContactsService } from '@app/main/content/private/contacts/contacts.service';
import { BuildingsService } from '@app/main/content/private/buildings/buildings.service';
import { CustomersService } from '@app/main/content/private/customers/customers.service';
import { UsersBaseService } from '@app/main/content/private/users/users-base.service';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { ContactFormComponent } from '../../contact-form/contact-form/contact-form.component';
import { AssignContactParameters } from '@app/core/models/building/object-parameters/assign-contact.model';
import { BuildingUpdateEmployeesModel } from '@app/core/models/building/building-update-employees.model';
import { SearchContactFormComponent } from '../../contact-form/search-contact-form/search-contact-form.component';

@Component({
  selector: 'app-building-form',
  templateUrl: './building-form.component.html',
  styleUrls: ['./building-form.component.scss'],
  encapsulation: ViewEncapsulation.None

})
export class BuildingFormComponent implements OnInit, OnDestroy {

  dialogTitle: string;
  buildingForm: FormGroup;
  action: string;

  roleLevelLoggedUser: number;

  building: BuildingBaseModel;
  address: AddressModel;

  customers: ListItemModel[] = [];
  filteredCustomers: Subject<any[]> = new Subject<any[]>();
  listCustomersSubscription: Subscription;
  assignedCustomer: ListItemModel;

  selectedOperationsManagers: any[] = [];
  operationsManagers: ListUserModel[] = [];
  filteredOperationsManagers: Subject<any[]> = new Subject<any[]>();
  listOperationsManagersSubscription: Subscription;

  inspectors: ListUserModel[] = [];
  filteredInspectors: Subject<any[]> = new Subject<any[]>();
  listInspectorsSubscription: Subscription;

  selectedSupervisors: any[] = [];
  supervisors: any[] = [];
  filteredSupervisors: Subject<any[]> = new Subject<any[]>();
  listSupervisorsSubscription: Subscription;

  subcontractors: any[] = [];

  // represent the list of contacts "property manager" of the selected building
  contacts: ContactBaseModel[] = [];

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  buildingAddress = '';

  dialogCustomerRef: any;

  fileUrl: any;

  invalidCode: boolean = false;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<BuildingFormComponent>,
    public addressDialogRef: MatDialogRef<AddressFormComponent>,
    public contactDialogRef: MatDialogRef<ContactFormComponent>,
    public searchContactDialogRef: MatDialogRef<SearchContactFormComponent>,
    public snackBar: MatSnackBar,
    private buildingService: BuildingsService,
    private customerService: CustomersService,
    private contactService: ContactsService,
    private usersService: UsersBaseService,
    private authService: AuthService,
    private sanitizer: DomSanitizer
  ) {
    this.action = data.action;

    if (this.action === 'edit') {

      this.dialogTitle = 'Edit Building';
      this.building = data.building;
      this.buildingForm = this.createWOUpdateForm();
      if (this.building.address !== {}) {
        this.buildingAddress = this.building.address.fullAddress;
      }

      // this.downloadPDFFile();
    }
    else {
      this.dialogTitle = 'New Building';
      this.buildingForm = this.createWOCreateForm();
    }
  }

  ngOnInit(): void {
    this.buildingCodeCtrl.valueChanges
    .pipe(
      debounceTime(300),
      distinctUntilChanged())
      .pipe(takeUntil(this._onDestroy))
      .subscribe(value => {
        if (value) {
          this.buildingService.validateBuildingCodeAvailability(value, this.building ? this.building.id : -1)
            .subscribe((result: string) => {
              if (result === 'Existing') {
                this.invalidCode = true;
                this.buildingCodeCtrl.setErrors({ 'incorrect': true });
              } else {
                this.invalidCode = false;
                this.buildingCodeCtrl.setErrors(null);
              }
            }, (error) => {
              console.log('Error');
            });
        }
      });

    if (this.action === 'edit') {
      this.getcustomers();

      this.customerIdCtrl.valueChanges
        .pipe(takeUntil(this._onDestroy))
        .subscribe(() => {
          this.filterCustomers();
        });

      /*     this.getOperationsManagers()
            .then(() => {
              this.getSupervisors();
            }).catch(); */

      this.operationsManagerIdCtrl.valueChanges
        .pipe(takeUntil(this._onDestroy))
        .subscribe(() => {
          this.filterOperationsManagers();
        });

      this.inspectorIdCtrl.valueChanges
        .pipe(takeUntil(this._onDestroy))
        .subscribe(() => {
          this.filterInspectors();
        });

      this.supervisorsCtrl.valueChanges
        .pipe(takeUntil(this._onDestroy))
        .subscribe(() => {
          this.filterSupervisors();
        });

      this.getContacts();
    }
    this.roleLevelLoggedUser = this.authService.currentUser.roleLevel;
    this.setPermissions();

    this.getEmployees();
  }

  setPermissions(): void {
    if (this.roleLevelLoggedUser > 10) {
      this.buildingForm.get('name').disable();
      this.buildingForm.get('address').disable();
      this.buildingForm.get('customerId').disable();
      this.buildingForm.get('operationsManagers').disable();
      this.buildingForm.get('supervisors').disable();
      this.buildingForm.get('emergencyPhone').disable();
      this.buildingForm.get('emergencyPhoneExt').disable();
      this.buildingForm.get('emergencyNotes').disable();
      this.buildingForm.get('isActive').disable();
    }
  }

  ngOnDestroy(): void {

    if (this.listCustomersSubscription && !this.listCustomersSubscription.closed) {
      this.listCustomersSubscription.unsubscribe();
    }

    if (this.listOperationsManagersSubscription && !this.listOperationsManagersSubscription.closed) {
      this.listOperationsManagersSubscription.unsubscribe();
    }

    if (this.listInspectorsSubscription && !this.listInspectorsSubscription.closed) {
      this.listInspectorsSubscription.unsubscribe();
    }

    if (this.listSupervisorsSubscription && !this.listSupervisorsSubscription.closed) {
      this.listSupervisorsSubscription.unsubscribe();
    }

    this._onDestroy.next();
    this._onDestroy.complete();
  }

  get buildingCodeCtrl(): AbstractControl {
    return this.buildingForm.get('code');
  }

  createWOCreateForm(): FormGroup {
    return this.formBuilder.group({
      code: ['', [Validators.required, Validators.maxLength(32)]],
      name: [''],
      address: [''],
      isActive: [1],
    });
  }

  // return building inspector
  get inspector(): any {
    return this.building.employees.filter(e => e.type === 8)[0] || null;
  }

  createWOUpdateForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.building.id],
      code: [this.building.code === '' ? this.building.id.toString() : this.building.code, [Validators.required, Validators.maxLength(32)]],
      name: [this.building.name],
      address: [this.building.address],
      customerId: [this.building.customerId],
      customerIdCtrl: [''],
      operationsManagers: [''],
      operationsManagerIdCtrl: [''],
      inspectorId: [this.inspector ? this.inspector.id : null],
      inspectorIdCtrl: [''],
      supervisors: [''],
      supervisorsCtrl: [''],
      isActive: [this.building.isActive],
      emergencyPhone: [this.building.emergencyPhone],
      emergencyPhoneExt: [this.building.emergencyPhoneExt],
      emergencyNotes: [this.building.emergencyNotes]
    });
  }

  /*
  * ADDRESS
  */
  addAddress(): void {
    this.addressDialogRef = this.dialog.open(AddressFormComponent, {
      panelClass: 'address-form-dialog',
      data: {
        action: 'create',
        type: 'building',
      }
    });

    this.addressDialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        this.address = response.value;
        this.buildingForm.get('address').setValue(this.address);
        this.buildingAddress = this.address.addressLine1 + ' ' + this.address.addressLine2 + ' ' + ' ' + this.address.city + ', ' + this.address.zipCode + ' ' + this.address.state;
      });
  }

  editAddress(): void {
    if (this.roleLevelLoggedUser <= 10) {
      // add entityId, type and default values into address
      this.building.address.entityId = this.building.id;
      this.building.address.type = 'building';
      this.building.address.default = 1;

      this.addressDialogRef = this.dialog.open(AddressFormComponent, {
        panelClass: 'address-form-dialog',
        data: {
          action: 'edit',
          type: 'building',
          address: this.building.address,
          entityId: this.building.id
        }
      });

      this.addressDialogRef.afterClosed()
        .subscribe((response: any) => {
          if (!response) {
            return;
          }
          this.address = response[1].value;

          this.buildingForm.get('address').setValue(this.address);
          this.buildingAddress = this.address.addressLine1 + ' '
            + this.address.addressLine2 + ' ' + ' ' + this.address.city
            + ', ' + this.address.zipCode + ' ' + this.address.state;
          this.buildingService.updateElement(this.address, 'updateaddress');
        });
    }
  }
  /*
 * END ADDRESS
 */

  /*
    * CUSTOMERS
    */
  getcustomers(): void {
    // If there is a selected customer, send customerId to consume /customers/readallcbo, else send 'null'
    const idCustomer = this.building.customerId ? this.building.customerId : null;

    if (this.listCustomersSubscription && !this.listCustomersSubscription.closed) {
      this.listCustomersSubscription.unsubscribe();
    }

    this.listCustomersSubscription = this.customerService.getAllAsList('readallcbo', '', 0, 99999, idCustomer, { 'withContacts': '1' })
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.customers = response.payload;
        this.filteredCustomers.next(this.customers);

        if (this.action === 'edit') {
          this.assignedCustomer = this.customers.find(c => c.id === this.building.customerId);
        }
      });
  }

  get customerIdCtrl(): AbstractControl {
    return this.buildingForm.get('customerIdCtrl');
  }

  private filterCustomers(): void {
    if (!this.customers) {
      return;
    }
    // get the search keyword
    let search = this.customerIdCtrl.value;
    if (!search) {
      this.filteredCustomers.next(this.customers.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the customers
    this.filteredCustomers.next(
      this.customers.filter(customer => customer.name.toLowerCase().indexOf(search) > -1)
    );
  }
  /*
  * END CUSTOMERS
  */

  getEmployees(): any {
    const operationsManagers = this.usersService.getAllAsList('readallcbo', '', 0, 99999, null, { 'roleLevel': '30' });

    const inspectors = this.usersService.getAllAsList('readallcbo', '', 0, 99999);

    const subcontractors = this.usersService.getAllAsList('readallcbo', '', 0, 99999, null, { 'roleLevel': '35' });

    const supervisors = this.usersService.getSupervisorsByBuildingId('readAllSupervisorsCbo', '', 0, 9999, null, this.building.id, {});

    return forkJoin([operationsManagers, subcontractors, supervisors, inspectors]).subscribe(results => {

      this.getOperationsManagers(results[0]);
      this.getSubcontractors(results[1]);
      this.getSupervisors(results[2]);
      this.getInspectors(results[3]);

    });
  }

  getSubcontractors(subcontractors: any): void {
    this.subcontractors = subcontractors.payload;
  }

  /*
 * OPERATIONS MANAGER
 */
  getOperationsManagers(operationsManager: any): void {
    this.operationsManagers = operationsManager.payload;
    this.filteredOperationsManagers.next(this.operationsManagers);

    const buildingOperationManagers = this.building.employees.filter(e => e.type === 2);

    buildingOperationManagers.forEach(element => {
      const s = this.operationsManagers.find(i => i.id === element.id);
      if (s) {
        this.selectedOperationsManagers.push(s);
      }
    });
  }

  get operationsManagerIdCtrl(): AbstractControl {
    return this.buildingForm.get('operationsManagerIdCtrl');
  }

  private filterOperationsManagers(): void {
    if (!this.operationsManagers) {
      return;
    }
    // get the search keyword
    let search = this.operationsManagerIdCtrl.value;
    if (!search) {
      this.filteredOperationsManagers.next(this.operationsManagers.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the operations managers
    this.filteredOperationsManagers.next(
      this.operationsManagers.filter(operationsManager => operationsManager.name.toLowerCase().indexOf(search) > -1)
    );
  }
  /*
 * END OPERATIONS MANAGER
 */

  /** INSPECTOR */
  getInspectors(inspectors: any): void {
    this.inspectors = inspectors.payload;
    this.filteredInspectors.next(this.inspectors);
  }
  get inspectorIdCtrl(): AbstractControl {
    return this.buildingForm.get('inspectorIdCtrl');
  }

  private filterInspectors(): void {
    if (!this.inspectors) {
      return;
    }
    // get the search keyword
    let search = this.inspectorIdCtrl.value;
    if (!search) {
      this.filteredInspectors.next(this.inspectors.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the operations managers
    this.filteredInspectors.next(
      this.inspectors.filter(inspector => inspector.name.toLowerCase().indexOf(search) > -1)
    );
  }
  /** END INSPECTOR */

  /*
  * SUPERVISOR
  */
  getSupervisors(supervisors: any): void {

    this.supervisors = supervisors.payload;
    // Add Operations managers into supervisor list that are not selected as building operations manager
    // and are not selected as building supervisor

    const possiblesOperationsManagerToDisplay = this.operationsManagers
      .filter(op =>
        this.building.employees.find(e => e.id === op.id && e.Type === 2) !== null
        && !this.supervisors.find(s => s.id === op.id));

    possiblesOperationsManagerToDisplay.forEach(om => {
      const sup = {
        roleName: om.roleName,
        id: om.id,
        name: om.name,
        type: 2,
      };
      this.supervisors.push(sup);
    });

    // Add Subcontractors to supervisors list that are not selected as supervisors
    const possiblesSubcontratorsToDisplay = this.subcontractors.filter(sub => !this.supervisors.find(s => s.id === sub.id));

    possiblesSubcontratorsToDisplay.forEach(subcontractor => {
      const sup = {
        roleName: subcontractor.roleName,
        id: subcontractor.id,
        name: subcontractor.name,
        type: 2,
      };
      this.supervisors.push(sup);
    });

    this.filteredSupervisors.next(this.supervisors);

    const buildingSupervisors = this.building.employees.filter(e => e.type === 1);
    if (supervisors) {
      let s;
      buildingSupervisors.forEach(element => {
        s = this.supervisors.find(i => i.id === element.id);
        this.selectedSupervisors.push(s);
      });
    }
  }

  get supervisorsCtrl(): AbstractControl {
    return this.buildingForm.get('supervisorsCtrl');
  }

  private filterSupervisors(): void {
    if (!this.supervisors) {
      return;
    }
    // get the search keyword
    let search = this.supervisorsCtrl.value;
    if (!search) {
      this.filteredSupervisors.next(this.supervisors.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the supervisor
    this.filteredSupervisors.next(
      this.supervisors.filter(supervisor => supervisor.name.toLowerCase().indexOf(search) > -1)
    );
  }

  /*
 * END SUPERVISOR
 */

  /*
  * CONTACTS
  */
  searchContact(): void {
    this.searchContactDialogRef = this.dialog.open(SearchContactFormComponent, {
      panelClass: 'search-contact-form-dialog',
      data: {
        action: 'create',
        entityId: this.building.id,
        contactType: 'building',
        title: 'Assign contact to building ' + this.building.name,
        showHistoryData: true
      }
    });

    this.searchContactDialogRef.afterClosed()
      .subscribe((response: any) => {

        if (!response) {
          return;
        }

        const assignContactParameters = new AssignContactParameters(response.getRawValue());

        this.assignContact(assignContactParameters);
        // TODO: Change when the response contains contact type
        // this.contacts.push(response);
      });
  }

  getContacts(): void {
    this.contactService.getAllContactsByEntity(this.building.id, 'building')
      .subscribe((contacts: ContactBaseModel[]) => {
        this.contacts = contacts;
      });
  }

  editContact(buildingId: number, contactId: number): void {
    if (this.roleLevelLoggedUser <= 10) {
      this.contactService.getContactByEntity(buildingId, contactId, 'building')
        .subscribe((contactData: ContactBaseModel) => {
          if (contactData) {
            this.contactDialogRef = this.dialog.open(ContactFormComponent, {
              panelClass: 'contact-form-dialog',
              data: {
                contact: contactData,
                action: 'edit',
              }
            });

            this.contactDialogRef.afterClosed()
              .subscribe(response => {
                if (!response) {
                  return;
                }
                const actionType: string = response[0];
                const formData: FormGroup = response[1];
                const updatedContactObj = formData.getRawValue();
                switch (actionType) {
                  /**
                   * Save
                   */
                  case 'save':

                    this.contactService.update(updatedContactObj)
                      .subscribe(
                        (res) => {
                          this.snackBar.open('Contact updated successfully!!!', 'close', { duration: 1000 });
                          this.getContacts();
                        },
                        (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

                    break;
                  /**
                   * Delete
                   */
                  case 'delete':

                    this.unassingContact(buildingId, updatedContactObj.id);

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
  }

  assignContact(assignContactParameters: AssignContactParameters): void {
    this.contactService.assignContact(assignContactParameters)
      .subscribe((response: any) => {
        if (response.status === 200) {
          this.snackBar.open('Contact added successfully!!!', 'close', { duration: 1000 });
          this.getContacts();
        }
      },
        (error) => {
          if (error.status === 412) {

            this.snackBar.open(error.error, 'close', { duration: 5000 });
          }
          else { this.snackBar.open('Oops, there was an error', 'close', { duration: 2000 }); }
        });
  }

  unassingContact(buildingId: number, contactId: number): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to unassign the contact from this building?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.contactService.unassignContact(buildingId, contactId, 'building')
          .then((r: any) => {
            this.snackBar.open('Contact unassigned successfully!!!', 'close', { duration: 1000 });
            this.getContacts();
          },
            (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

      }
      this.confirmDialogRef = null;
    });
  }
  /*
  * END CONTACTS
  */

  submit(): void {

    if (this.selectedOperationsManagers.length === 0) {
      this.snackBar.open('Oops, select at least one operation manager', 'close', { duration: 2000 });
      return;
    }

    const updatedBuildingObj = new BuildingUpdateEmployeesModel(this.buildingForm.getRawValue());
    this.dialogRef.close(['save', updatedBuildingObj]);
  }

  downloadPDFFile(): void {
    this.buildingService.get(this.building.id, 'GetBuildingPDFBase64').subscribe(
      (response: any) => {
        // this.loading$.next(false);
        const bytes = atob(response);

        const byteNumbers = new Array(bytes.length);
        for (let i = 0; i < bytes.length; i++) {
          byteNumbers[i] = bytes.charCodeAt(i);
        }

        const byteArray = new Uint8Array(byteNumbers);

        const blob = new Blob([byteArray], { type: 'application/pdf' });

        this.fileUrl = this.sanitizer.bypassSecurityTrustResourceUrl(window.URL.createObjectURL(blob));

      }, (error) => {
        // this.loading$.next(false);
        this.snackBar.open('Oops, pdf file not available to download', 'close', { duration: 1000 });
      }
    );
  }
}
