import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PhoneModel } from '@app/core/models/common/phone.model';
import { EmailModel } from '@app/core/models/common/email.model';
import { AddressModel } from '@app/core/models/common/address.model';
import { UserBaseModel } from '@app/core/models/user/user-base.model';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { CalendarEvent } from 'angular-calendar';
import { BehaviorSubject } from 'rxjs';
import { UsersService } from '../users.service';
import { DepartmentsService } from '@app/main/content/private/departments/departments.service';
import { ContactsService } from '@app/main/content/private/contacts/contacts.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '@app/core/services/auth.service';
import { MatSelectChange } from '@angular/material/select';
import { PhoneFormComponent } from '@app/core/modules/contact-form/phone-form/phone-form.component';
import { EmailFormComponent } from '@app/core/modules/contact-form/email-form/email-form.component';
import { AddressFormComponent } from '@app/core/modules/contact-form/address-form/address-form.component';
import { fuseAnimations } from '@fuse/animations';
import { ListItemModel } from '@app/core/models/common/list-item.model';

@Component({
  selector: 'app-user-form',
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class UserFormComponent implements OnInit {

  event: CalendarEvent;
  dialogTitle: string;
  userForm: FormGroup;
  action: string;
  phoneDialogRef: any;
  emailDialogRef: any;
  addressDialogRef: any;
  user: UserBaseModel;
  roles: any[];

  departments$: BehaviorSubject<ListItemModel[]> = new BehaviorSubject<ListItemModel[]>([]);
  phones$: BehaviorSubject<PhoneModel[]> = new BehaviorSubject<PhoneModel[]>([]);
  emails$: BehaviorSubject<EmailModel[]> = new BehaviorSubject<EmailModel[]>([]);
  addresses$: BehaviorSubject<AddressModel[]> = new BehaviorSubject<AddressModel[]>([]);
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  public freshdeskKey: string;
  public freshdeskAgentId: string;

  constructor(
    public dialogRef: MatDialogRef<UserFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private usersService: UsersService,
    private departmentsService: DepartmentsService,
    private contactsService: ContactsService,
    public snackBar: MatSnackBar,
    private authService: AuthService
  ) {
    this.action = data.action;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit User';
      this.user = data.user;
      this.freshdeskAgentId = this.user.freshdeskAgentId;
      this.freshdeskKey = this.user.freshdeskApiKey;
      this.getPhones();
      this.getEmails();
      this.getAddresses();
      this.userForm = this.createUserUpdateForm();
      this.departmentsService
        .getAll()
        .subscribe((response: { count: number, payload: any[] }) => {
          if (response.payload) {
            const mappedDepartments = response.payload.map(
              d => {
                return { id: d.id, name: d.name };
              }
            );
            this.departments$.next(mappedDepartments);
          }
        });
    }
    else {
      this.dialogTitle = 'New User';
      this.userForm = this.createUserCreateForm();
    }

  }

  ngOnInit(): void {
    this.getRoles();

    this.userForm.get('freshdeskApiKey').valueChanges
      .subscribe(value => {
        this.freshdeskKey = value;
      });

    this.userForm.get('freshdeskAgentId').valueChanges
      .subscribe(value => {
        this.freshdeskAgentId = value;
      });
  }

  getRoles(): void {
    this.usersService.getAllAsList('readallcboroles', '', 0, 20, null)
      .subscribe((response: { count: number, payload: any[] }) => {
        this.roles = response.payload;
      },
        (error) => this.snackBar.open('Oops, there was an error fetching roles', 'close', { duration: 1000 })
      );
  }

  createUserCreateForm(): FormGroup {
    return this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]]
    });
  }

  editPhone(phoneId: number): void {
    if (!this.readOnly) {
      this.contactsService.getPhone(phoneId)
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

                    this.contactsService.updatePhone(updatedPhoneObj)
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

  newPhone(contactId: number): void {
    this.phoneDialogRef = this.dialog.open(PhoneFormComponent, {
      panelClass: 'phone-form-dialog',
      data: {
        action: 'create',
        entityId: contactId
      }
    });

    this.phoneDialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        this.contactsService.createPhone(response.getRawValue())
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
        this.contactsService.deletePhone(phoneId)
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
    this.contactsService.getAllPhones(this.user.contactId)
      .subscribe((phones: PhoneModel[]) => {
        this.phones$.next(phones);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching user phones', 'close', { duration: 1000 }));
  }

  editEmail(emailId: number): void {
    if (!this.readOnly) {
      this.contactsService.getEmail(emailId)
        .subscribe((emailData: EmailModel) => {
          if (emailData) {
            this.emailDialogRef = this.dialog.open(EmailFormComponent, {
              panelClass: 'email-form-dialog',
              data: {
                email: emailData,
                action: 'edit'
              }
            });

            this.emailDialogRef.afterClosed()
              .subscribe(response => {
                if (!response) {
                  return;
                }
                const actionType: string = response[0];
                const formData: FormGroup = response[1];
                const updatedEmailObj = formData.getRawValue();
                switch (actionType) {
                  /**
                   * Save
                   */
                  case 'save':

                    this.contactsService.updateEmail(updatedEmailObj)
                      .subscribe(
                        (res) => {
                          this.snackBar.open('Email updated successfully!!!', 'close', { duration: 1000 });
                          this.getEmails();
                        },
                        (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

                    break;
                  /**
                   * Delete
                   */
                  case 'delete':

                    this.deleteEmail(updatedEmailObj.id);

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

  newEmail(contactId: number): void {
    this.emailDialogRef = this.dialog.open(EmailFormComponent, {
      panelClass: 'email-form-dialog',
      data: {
        action: 'create',
        entityId: contactId
      }
    });

    this.emailDialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        this.contactsService.createEmail(response.getRawValue())
          .subscribe((r: any) => {
            this.snackBar.open('Email created successfully!!!', 'close', { duration: 1000 });
            this.getEmails();
          },
            (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });

  }

  deleteEmail(emailId: number): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.contactsService.deleteEmail(emailId)
          .then((r: any) => {
            this.snackBar.open('Email deleted successfully!!!', 'close', { duration: 1000 });
            this.getEmails();
          },
            (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

      }
      this.confirmDialogRef = null;
    });
  }

  getEmails(): void {
    this.contactsService.getAllEmails(this.user.contactId)
      .subscribe((emails: EmailModel[]) => {
        this.emails$.next(emails);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching user emails', 'close', { duration: 1000 }));
  }

  editAddress(contactId: number, addressId: number): void {
    if (!this.readOnly) {
      this.contactsService.getAddress(contactId, addressId)
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

                    this.contactsService.updateAddress(updatedAddressObj)
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

  newAddress(contactId: number): void {
    this.addressDialogRef = this.dialog.open(AddressFormComponent, {
      panelClass: 'address-form-dialog',
      data: {
        action: 'create',
        entityId: contactId
      }
    });

    this.addressDialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        this.contactsService.createAddress(response.getRawValue())
          .subscribe((r: any) => {
            this.snackBar.open('Address created successfully!!!', 'close', { duration: 1000 });
            this.getAddresses();
          },
            (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });

  }

  deleteAddress(contactId: number, addressId: number): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.contactsService.deleteAddress(contactId, addressId)
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
    this.contactsService.getAllAddresses(this.user.contactId)
      .subscribe((addresses: AddressModel[]) => {
        this.addresses$.next(addresses);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching user addresses', 'close', { duration: 1000 }));
  }

  createUserUpdateForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.user.id],
      guid: [this.user.guid],
      contactId: [this.user.contactId],
      firstName: [{ value: this.user.firstName, disabled: this.readOnly }, [Validators.required]],
      middleName: [{ value: this.user.middleName, disabled: this.readOnly }],
      salutation: [{ value: this.user.salutation, disabled: this.readOnly }],
      departmentId: [{ value: this.user.departmentId, disabled: this.readOnly }],
      roleId: [{ value: this.user.roleId, disabled: this.readOnly }],
      roleLevel: [{ value: this.user.roleLevel, disabled: this.readOnly }],
      dob: [{ value: this.user.dob, disabled: this.readOnly }],
      lastName: [{ value: this.user.lastName, disabled: this.readOnly }, [Validators.required]],
      email: [{ value: this.user.email, disabled: this.readOnly }, [Validators.required, Validators.email]],
      notes: [{ value: this.user.notes, disabled: this.readOnly }],
      sendNotifications: [{ value: this.user.sendNotifications, disabled: this.readOnly }],
      hasFreshdeskAccount: [{ value: this.user.hasFreshdeskAccount, disabled: this.readOnly }],
      freshdeskApiKey: [{ value: this.user.freshdeskApiKey, disabled: this.readOnly }],
      freshdeskAgentId: [{ value: this.user.freshdeskAgentId, disabled: this.readOnly }],
      emailSignature: [{ value: this.user.emailSignature, disabled: this.readOnly }],
    });
  }

  // Form
  /**
   * to prevent
   * @param event
   */
  roleChanged(event: MatSelectChange): void {
    const role = this.roles.find(r => r.id === event.value);
    if (role) {
      this.userForm.patchValue({ roleLevel: role.level });
    }
  }

}
