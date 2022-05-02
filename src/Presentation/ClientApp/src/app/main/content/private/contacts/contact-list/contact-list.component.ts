import { DataSource } from '@angular/cdk/table';
import { DatePipe } from '@angular/common';
import { AfterViewInit, Component, OnInit, ViewEncapsulation, OnDestroy, ViewChild, TemplateRef, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { ContactBaseModel } from '@app/core/models/contact/contact-base.model';
import { ContactFormComponent } from '@app/core/modules/contact-form/contact-form/contact-form.component';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { merge, Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { ContactsService } from '../contacts.service';

@Component({
  selector: 'app-contact-list',
  templateUrl: './contact-list.component.html',
  styleUrls: ['./contact-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ContactListComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  get contactsCount(): any { return this.contactsService.elementsCount; }
  contacts: ContactBaseModel[];
  contact: ContactBaseModel;
  dataSource: ContactsDataSource | null;
  displayedColumns = ['checkbox', 'fullName', 'email', 'phone', 'isCustomer', 'isEmployee', 'buttons'];
  selectedContacts: any[];
  checkboxes: {};

  onContactsChangedSubscription: Subscription;
  onSelectedContactsChangedSubscription: Subscription;
  onContactDataChangedSubscription: Subscription;

  loading$ = this.contactsService.loadingSubject.asObservable();

  dialogRef: any;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  @Input()
  searchInput: FormControl;

  @Input() readOnly: boolean;

  constructor(
    private contactsService: ContactsService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private datePipe: DatePipe
  ) {
    this.onContactsChangedSubscription =
      this.contactsService.allElementsChanged.subscribe(contacts => {

        this.contacts = contacts;
        this.checkboxes = {};

        contacts.map(contact => {
          this.checkboxes[contact.guid] = false;
        });
      });

    this.onSelectedContactsChangedSubscription =
      this.contactsService.selectedElementsChanged.subscribe(selectedContacts => {
        for (const guid in this.checkboxes) {
          if (!this.checkboxes.hasOwnProperty(guid)) {
            continue;
          }

          this.checkboxes[guid] = selectedContacts.includes(guid);
        }
        this.selectedContacts = selectedContacts;
      });

    this.onContactDataChangedSubscription =
      this.contactsService.elementChanged.subscribe(contact => {
        this.contact = contact;
      });

  }

  ngOnInit(): void {
    this.searchInput.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      ).subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.contactsService.searchTextChanged.next(searchText);
      });
    this.dataSource = new ContactsDataSource(this.contactsService);
  }

  ngAfterViewInit(): void {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.contactsService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize))
      )
      .subscribe();

  }

  ngOnDestroy(): void {
    this.onContactsChangedSubscription.unsubscribe();
    this.onSelectedContactsChangedSubscription.unsubscribe();
    this.onContactDataChangedSubscription.unsubscribe();
  }

  editContact(contact): void {
    this.contactsService.get(contact.guid, 'update')
      .subscribe((contactData: any) => {
        if (contactData) {
          const contactUpdateObj = new ContactBaseModel(contactData);
          this.dialogRef = this.dialog.open(ContactFormComponent, {
            panelClass: 'contact-form-dialog',
            data: {
              contact: contactUpdateObj,
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
              const updatedContactObj = new ContactBaseModel(formData.getRawValue());
              switch (actionType) {
                /**
                 * Save
                 */
                case 'save':

                  this.contactsService.updateElement(updatedContactObj)
                    .then(
                      () => this.snackBar.open('Contact updated successfully!!!', 'close', { duration: 1000 }),
                      () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
                    .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

                  break;
                /**
                 * Delete
                 */
                case 'delete':

                  this.deleteContact(contactUpdateObj);

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
   * Delete Contact
   */
  deleteContact(contact): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.contactsService.deleteContact(contact)
          .subscribe((response: any) => {
            this.snackBar.open('Proposal delete successfully!!!', 'close', { duration: 1000 });
            this.contactsService.getElements();
          }, (error) => {
            this.snackBar.open('Oops, This conntact is not allowed to remove', 'close', { duration: 3000 });
          });
      }
      this.confirmDialogRef = null;
    });

  }

  onSelectedChange(contactGuid): void {
    this.contactsService.toggleSelectedElement(contactGuid);
  }

  isEmployeeStatus(contact): string {
    if (contact.isEmployee === 1) {
      return 'Yes';
    }
    return 'No';
  }

  isEmployeeWebStatus(contact): string {
    if (contact.isEmployee === 1) {
      if (contact.webApp) {
        return ' Web Used: ' + this.datePipe.transform(contact.webApp, 'MM/dd/yyyy hh:mm');
      }
      return '';
    }
    return '';
  }

  isEmployeeAppStatus(contact): string {
    if (contact.isEmployee === 1) {
      if (contact.managerApp) {
        return 'App Used: ' + this.datePipe.transform(contact.managerApp, 'MM/dd/yyyy hh:mm');
      }
      return '';
    }
    return '';
  }

  isCustomerStatus(contact): string {
    if (contact.isCustomer === 1) {
      return 'Yes';
    }
    return 'No';
  }
  isCustomerAppStatus(contact): string {
    if (contact.isCustomer === 1) {
      if (contact.clientApp) {
        return 'App Used: ' + this.datePipe.transform(contact.clientApp, 'MM/dd/yyyy hh:mm');
      }
      return '';
    }
    return '';
  }

  sendCredentials(contact): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = `Send Invitation Email to ${contact.email}?`;

    this.confirmDialogRef.afterClosed()
      .subscribe(result => {
        if (result) {
          this.contactsService.sendInvitationEmailContact(contact)
            .subscribe((r) => {
              this.snackBar.open('Invitation sent successfully!!!', 'close', { duration: 1000 });
            },
              (error) => {
                this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
              });

        }
        this.confirmDialogRef = null;
      });
  }

}

export class ContactsDataSource extends DataSource<any>
{
  constructor(private contactsService: ContactsService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.contactsService.allElementsChanged;
  }

  disconnect(): void {
  }
}

