import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { Form, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AddressModel } from '@app/core/models/common/address.model';
import { EmailModel } from '@app/core/models/common/email.model';
import { PhoneModel } from '@app/core/models/common/phone.model';
import { ContactBaseModel } from '@app/core/models/contact/contact-base.model';
import { ContactsService } from '@app/main/content/private/contacts/contacts.service';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { CalendarEvent } from 'angular-calendar';
import { BehaviorSubject } from 'rxjs';
import { AddressFormComponent } from '../address-form/address-form.component';
import { EmailFormComponent } from '../email-form/email-form.component';
import { PhoneFormComponent } from '../phone-form/phone-form.component';

@Component({
  selector: 'contact-form-dialog',
  templateUrl: './contact-form.component.html',
  styleUrls: ['./contact-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ContactFormComponent implements OnInit {

  event: CalendarEvent;
  dialogTitle: string;
  contactForm: FormGroup;
  action: string;
  phoneDialogRef: any;
  emailDialogRef: any;
  addressDialogRef: any;
  contact: ContactBaseModel;

  // For pre-populate contact form fields with `WorkOrder.Requester*` fields
  preRequesterSelected: any = {};

  phones$: BehaviorSubject<PhoneModel[]> = new BehaviorSubject<PhoneModel[]>([]);
  emails$: BehaviorSubject<EmailModel[]> = new BehaviorSubject<EmailModel[]>([]);
  addresses$: BehaviorSubject<AddressModel[]> = new BehaviorSubject<AddressModel[]>([]);
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    public dialogRef: MatDialogRef<ContactFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private contactsService: ContactsService,
    public snackBar: MatSnackBar
  ) {
    this.action = data.action;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Contact';
      this.contact = data.contact;
      this.getPhones();
      this.getEmails();
      this.getAddresses();
      this.contactForm = this.createContactUpdateForm();
    }
    else {
      this.dialogTitle = 'New Contact';
      if (data.preRequesterSelected) {
        this.preRequesterSelected = data.preRequesterSelected;
      }
      this.contactForm = this.createContactCreateForm();
    }
  }

  ngOnInit(): void {
  }

  createContactCreateForm(): FormGroup {
    return this.formBuilder.group({
      firstName: [this.preRequesterSelected.firstName, [Validators.required, Validators.maxLength(80)]],
      lastName: [this.preRequesterSelected.lastName, [Validators.required, Validators.maxLength(80)]],
      phone: [this.preRequesterSelected.phone, [Validators.required]],
      ext: [''],
      email: [this.preRequesterSelected.email, [Validators.required]],
      sendNotifications: [true],
    });
  }

  createContactUpdateForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.contact.id],
      guid: [this.contact.guid],
      firstName: [{ value: this.contact.firstName, disabled: this.readOnly }, [Validators.required, Validators.maxLength(80)]],
      middleName: [{ value: this.contact.middleName, disabled: this.readOnly }, [Validators.maxLength(80)]],
      salutation: [{ value: this.contact.salutation, disabled: this.readOnly }, [Validators.maxLength(80)]],
      title: [{ value: this.contact.title, disabled: this.readOnly }, [Validators.maxLength(80)]],
      dob: [{ value: this.contact.dob, disabled: this.readOnly }],
      lastName: [{ value: this.contact.lastName, disabled: this.readOnly }, [Validators.required, Validators.maxLength(80)]],
      sendNotifications: [{ value: this.contact.sendNotifications, disabled: this.readOnly }],
      notes: [{ value: this.contact.notes, disabled: this.readOnly }],
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
    this.contactsService.getAllPhones(this.contact.id)
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
    this.contactsService.getAllEmails(this.contact.id)
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
    this.contactsService.getAllAddresses(this.contact.id)
      .subscribe((addresses: AddressModel[]) => {
        this.addresses$.next(addresses);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching user addresses', 'close', { duration: 1000 }));
  }
}
