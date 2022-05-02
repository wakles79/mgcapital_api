import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, AbstractControl, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AssignContactParameters } from '@app/core/models/building/object-parameters/assign-contact.model';
import { AddressModel } from '@app/core/models/common/address.model';
import { PhoneModel } from '@app/core/models/common/phone.model';
import { ContactBaseModel } from '@app/core/models/contact/contact-base.model';
import { CustomerBaseModel } from '@app/core/models/customer/customer-base.model';
import { CustomersBaseService } from '@app/core/services/customers.service';
import { ContactsService } from '@app/main/content/private/contacts/contacts.service';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { CalendarEvent } from 'angular-calendar';
import { BehaviorSubject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AddressFormComponent } from '../contact-form/address-form/address-form.component';
import { ContactFormComponent } from '../contact-form/contact-form/contact-form.component';
import { PhoneFormComponent } from '../contact-form/phone-form/phone-form.component';
import { SearchContactFormComponent } from '../contact-form/search-contact-form/search-contact-form.component';

@Component({
  selector: 'customer-form-dialog',
  templateUrl: './customer-form.component.html',
  styleUrls: ['./customer-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class CustomerFormComponent implements OnInit {

  event: CalendarEvent;
  dialogTitle: string;
  customerForm: FormGroup;
  action: string;
  phoneDialogRef: any;
  addressDialogRef: any;
  contactDialogRef: any;
  customer: CustomerBaseModel;

  phones$: BehaviorSubject<PhoneModel[]> = new BehaviorSubject<PhoneModel[]>([]);
  addresses$: BehaviorSubject<AddressModel[]> = new BehaviorSubject<AddressModel[]>([]);
  contacts$: BehaviorSubject<ContactBaseModel[]> = new BehaviorSubject<ContactBaseModel[]>([]);
  groups$: BehaviorSubject<{ id: number, name: string }[]> = new BehaviorSubject<{ id: number, name: string }[]>([]);
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  invalidCode: boolean = false;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    public dialogRef: MatDialogRef<CustomerFormComponent>,
    public searchContactDialogRef: MatDialogRef<SearchContactFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private customersService: CustomersBaseService,
    private contactsService: ContactsService,
    public snackBar: MatSnackBar
  ) {
    this.action = data.action;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Management Co.';
      this.customer = data.customer;
      this.getPhones();
      this.getAddresses();
      this.getContacts();
      this.getGroups();
      this.getAssignedGroups();
      this.customerForm = this.createCustomerUpdateForm();
    }
    else {
      this.dialogTitle = 'New Management Co.';
      this.customerForm = this.createCustomerCreateForm();
    }

  }

  ngOnInit(): void {

    this.customerCodeCtrl.valueChanges
      .pipe(debounceTime(300),
        distinctUntilChanged())
      .subscribe(value => {
        if (value) {
          this.customersService.validateCustomerCodeAvailability(value, this.customer ? this.customer.id : -1)
            .subscribe((result: string) => {
              if (result === 'Existing') {
                this.invalidCode = true;
                this.customerCodeCtrl.setErrors({ 'incorrect': true });
              } else {
                this.invalidCode = false;
                this.customerCodeCtrl.setErrors(null);
              }
            }, (error) => {
              console.log('Error');
            });
        }
      });

  }

  get code(): string {
    return Math.random().toString(36).substr(2, 32).toUpperCase();
  }

  get customerCodeCtrl(): AbstractControl {
    return this.customerForm.get('code');
  }


  createCustomerCreateForm(): FormGroup {
    return this.formBuilder.group({
      code: ['', [Validators.required, Validators.maxLength(32)]],
      name: ['', [Validators.required, Validators.maxLength(80)]],
      notes: [''],
      minimumProfitMargin: [0],
      creditLimit: [0],
    });
  }

  createCustomerUpdateForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.customer.id],
      guid: [this.customer.guid],
      code: [{ value: this.customer.code, disabled: this.readOnly }, [Validators.required, Validators.maxLength(32)]],
      name: [{ value: this.customer.name, disabled: this.readOnly }, [Validators.required, Validators.maxLength(80)]],
      notes: [{ value: this.customer.notes, disabled: this.readOnly }],
      minimumProfitMargin: [this.customer.minimumProfitMargin],
      creditLimit: [this.customer.creditLimit],
    });
  }

  editPhone(phoneId: number): void {
    if (!this.readOnly) {
      this.customersService.getPhone(phoneId)
        .subscribe((phoneData: PhoneModel) => {
          if (phoneData) {
            this.phoneDialogRef = this.dialog.open(PhoneFormComponent, {
              panelClass: 'phone-form-dialog',
              data: {
                phone: phoneData,
                action: 'edit'
              }
            });

            this.phoneDialogRef.afterClosed()
              .subscribe(response => {
                if (!response) {
                  return;
                }
                const actionType: string = response[0];
                const formData: FormGroup = response[1];
                const updatedPhoneObj = formData.getRawValue();
                switch (actionType) {
                  /**
                   * Save
                   */
                  case 'save':

                    this.customersService.updatePhone(updatedPhoneObj)
                      .subscribe(
                        (res) => {
                          this.snackBar.open('Phone updated successfully!!!', 'close', { duration: 1000 });
                          this.getPhones();
                        },
                        (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

                    break;
                  /**
                   * Delete
                   */
                  case 'delete':

                    this.deletePhone(updatedPhoneObj.id);

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

  newPhone(customerId: number): void {
    this.phoneDialogRef = this.dialog.open(PhoneFormComponent, {
      panelClass: 'phone-form-dialog',
      data: {
        action: 'create',
        entityId: customerId
      }
    });

    this.phoneDialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        this.customersService.createPhone(response.getRawValue())
          .subscribe((r: any) => {
            this.snackBar.open('Phone created successfully!!!', 'close', { duration: 1000 });
            this.getPhones();
          },
            (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });

  }

  deletePhone(phoneId: number): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.customersService.deletePhone(phoneId)
          .then((r: any) => {
            this.snackBar.open('Phone deleted successfully!!!', 'close', { duration: 1000 });
            this.getPhones();
          },
            (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

      }
      this.confirmDialogRef = null;
    });
  }

  getPhones(): void {
    this.customersService.getAllPhones(this.customer.id)
      .subscribe((phones: PhoneModel[]) => {
        this.phones$.next(phones);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching customer phones', 'close', { duration: 1000 }));
  }

  editAddress(customerId: number, addressId: number): void {
    if (!this.readOnly) {
      this.customersService.getAddress(customerId, addressId)
        .subscribe((addressData: AddressModel) => {
          if (addressData) {
            this.addressDialogRef = this.dialog.open(AddressFormComponent, {
              panelClass: 'address-form-dialog',
              data: {
                address: addressData,
                action: 'edit'
              }
            });

            this.addressDialogRef.afterClosed()
              .subscribe(response => {
                if (!response) {
                  return;
                }
                const actionType: string = response[0];
                const formData: FormGroup = response[1];
                const updatedAddressObj = formData.getRawValue();
                switch (actionType) {
                  /**
                   * Save
                   */
                  case 'save':

                    this.customersService.updateAddress(updatedAddressObj)
                      .subscribe(
                        (res) => {
                          this.snackBar.open('Address updated successfully!!!', 'close', { duration: 1000 });
                          this.getAddresses();
                        },
                        (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

                    break;
                  /**
                   * Delete
                   */
                  case 'delete':

                    this.deleteAddress(updatedAddressObj.entityId, updatedAddressObj.addressId);

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

  newAddress(customerId: number): void {
    this.addressDialogRef = this.dialog.open(AddressFormComponent, {
      panelClass: 'address-form-dialog',
      data: {
        action: 'create',
        entityId: customerId
      }
    });

    this.addressDialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        this.customersService.createAddress(response.getRawValue())
          .subscribe((r: any) => {
            this.snackBar.open('Address created successfully!!!', 'close', { duration: 1000 });
            this.getAddresses();
          },
            (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });

  }

  deleteAddress(customerId: number, addressId: number): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.customersService.deleteAddress(customerId, addressId)
          .then((r: any) => {
            this.snackBar.open('Address deleted successfully!!!', 'close', { duration: 1000 });
            this.getAddresses();
          },
            (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

      }
      this.confirmDialogRef = null;
    });
  }

  getAddresses(): void {
    this.customersService.getAllAddresses(this.customer.id)
      .subscribe((addresses: AddressModel[]) => {
        this.addresses$.next(addresses);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching customer addresses', 'close', { duration: 1000 }));
  }

  // Contacts
  searchContact(): void {
    this.searchContactDialogRef = this.dialog.open(SearchContactFormComponent, {
      panelClass: 'search-contact-form-dialog',
      data: {
        action: 'create',
        entityId: this.customer.id,
        contactType: 'customer',
        title: 'Assign contact to Management Co. '
      }
    });

    this.searchContactDialogRef.afterClosed()
      .subscribe((response: any) => {
        if (!response) {
          return;
        }
        console.log('response', response);
        // TODO: Change when the response contains contact type
        // this.contacts.push(response);
        const assignContactParameters = new AssignContactParameters(response.getRawValue());

        this.assignContact(assignContactParameters);
        // TODO: Change when the response contains contact type
        // this.contacts.push(response);
      });
  }

  editContact(customerId: number, contactId: number): void {
    if (!this.readOnly) {
      this.contactsService.getContactByEntity(customerId, contactId, 'customer')
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

                    this.contactsService.update(updatedContactObj)
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

                    this.unassignContact(customerId, updatedContactObj.id);

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
    this.contactsService.assignContact(assignContactParameters)
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

  unassignContact(customerId: number, contactId: number): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to unassign the contact from this Management Co.?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.contactsService.unassignContact(customerId, contactId, 'customer')
          .then((r: any) => {
            this.snackBar.open('Contact unassign successfully!!!', 'close', { duration: 1000 });
            this.getContacts();
          },
            (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

      }
      this.confirmDialogRef = null;
    });
  }

  getContacts(): void {
    this.contactsService.getAllContactsByEntity(this.customer.id, 'customer')
      .subscribe((contacts: ContactBaseModel[]) => {
        this.contacts$.next(contacts);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching Management Co. contacts', 'close', { duration: 1000 }));
  }
  // End of Contacts
  // Groups
  getGroups(): void {
    this.customersService.getAllGroups()
      .subscribe((groups: { id: number, name: string }[]) => {
        this.groups$.next(groups);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching groups', 'close', { duration: 1000 }));
  }

  getAssignedGroups(): void {
    this.customersService.getAllAssignedGroups(this.customer.id)
      .subscribe((groups: { id: number, name: string }[]) => {
        this.customer.groupIds = groups.map(g => g.id);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching customer groups', 'close', { duration: 1000 }));
  }
  // End of Groups
}
