import { Component, OnInit, ViewEncapsulation, OnDestroy, ViewChild, Inject } from '@angular/core';
import { FormGroup, AbstractControl, FormBuilder, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSelect } from '@angular/material/select';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { ReplaySubject, Subject, Subscription } from 'rxjs';
import { ListContactModel } from '@app/core/models/contact/list-contact.model';
import { ContactFormComponent } from '../contact-form/contact-form.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ContactsService } from '@app/main/content/private/contacts/contacts.service';
import { BUILDING_CONTACT_TYPES } from '@core/models/contact/building-contact-types';
import { takeUntil } from 'rxjs/operators';
import { ListItemModel } from '@app/core/models/common/list-item.model';

@Component({
  selector: 'search-contact-form-dialog',
  templateUrl: './search-contact-form.component.html',
  styleUrls: ['./search-contact-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SearchContactFormComponent implements OnInit, OnDestroy {

  dialogTitle: string;
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;
  searchContactForm: FormGroup;

  selectedContactId: number;

  selectedContact: any;
  entityId: number;
  contactType: any;
  contactTypes: any[] = [];
  contacts: ListContactModel[];
  filteredContacts$: ReplaySubject<ListContactModel[]> = new ReplaySubject<ListContactModel[]>();
  @ViewChild('contactsSelect') contactsSelect: MatSelect;

  // If contactTypeRequester is true, "Type" is an input readonly with value 'Requester'
  contactTypeRequester: boolean;

  showHistoryData: boolean;
  showHistoryFrom: any;

  private readonly RELAD_MIN_SCROLL_POSITION = 4;
  private readonly RELOAD_TOP_SCROLL_POSITION = 100;
  private currentContactsSelectPage = 0;
  private contactsListSubscription: Subscription;

  // For pre-populate contact form fields with `WorkOrder.Requester*` fields
  preRequesterSelected: boolean;
  preRequesterSelectedData: any = {};

  get contactCtrl(): AbstractControl {
    return this.searchContactForm.get('contactCtrl');
  }

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<SearchContactFormComponent>,
    public contactDialogRef: MatDialogRef<ContactFormComponent>,
    public snackBar: MatSnackBar,
    private formBuilder: FormBuilder,
    private contactsService: ContactsService,
  ) {
    this.dialogTitle = data.title;
    this.entityId = data.entityId;
    this.contactType = data.contactType;

    this.contactTypeRequester = false;
    this.preRequesterSelected = false;
    // this.selectedContactId = 0;

    if (this.contactType === 'building') {
      this.contactTypes = BUILDING_CONTACT_TYPES;
    }

    if (data.preRequesterSelected) {
      if (Object.keys(data.preRequesterSelected).length > 0) {
        this.preRequesterSelected = true;
        this.preRequesterSelectedData = data.preRequesterSelected;
      }
    }
    if (data.contactTypeRequester) {
      this.contactTypeRequester = true;
    }
    if (data.showHistoryData) {
      this.showHistoryData = true;
      this.showHistoryFrom = new Date();
    }

    this.searchContactForm = this.createSearchContactCreateForm();
  }

  ngOnInit(): void {
    this.getContacts();
    this.contactCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterContacts();
      });

    this.contactsSelect.openedChange.subscribe(() => {
      this.registerContactsPanelScrollEvent();
    });
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  createSearchContactCreateForm(): FormGroup {
    let type = '';
    if (this.contactTypeRequester) {
      type = 'Requester';
    }
    return this.formBuilder.group({
      entityId: [this.entityId],
      contactType: [this.contactType],
      contactId: ['', Validators.required],
      contactCtrl: [''],
      type: [type, Validators.required],
      showHistoryFrom: [this.showHistoryFrom],
    });
  }

  contactsOnChange($event, value): void {
    this.selectedContactId = Number(value);
    if (this.selectedContactId !== null) {
      this.selectedContact = this.contacts.find(c => c.id === this.selectedContactId);
    }
    else {
      this.selectedContactId = 0;
    }
  }

  registerContactsPanelScrollEvent(): void {
    const panel = this.contactsSelect.panel.nativeElement;
    panel.addEventListener('scroll', event => this.filterContactsOnScroll(event));
  }

  filterContactsOnScroll(event): void {
    const currentTop = event.target.scrollTop;
    if (currentTop >= this.RELOAD_TOP_SCROLL_POSITION) {
      // this.currentContactsSelectPage++;
      // this.filterContacts();
    }
    if (currentTop <= this.RELAD_MIN_SCROLL_POSITION) {
      // this.currentContactsSelectPage = Math.max(0, this.currentContactsSelectPage - 1);
      // this.filterContacts();
    }
  }

  private filterContacts(): void {
    this.filterElements(this.contacts, this.contactCtrl, this.filteredContacts$, this.contactsSelect, this.selectedContactId, this.getContacts);
  }

  private filterElements(
    els: any[],
    ctrl: AbstractControl,
    subj$: ReplaySubject<ListItemModel[]>,
    itemsSelect: MatSelect,
    selectedElement: number,
    filterFunc): void {
    // get the search keyword
    const search = (ctrl.value || '').toLowerCase();
    if (search === '' && selectedElement) {
      return;
    }
    // make another request
    filterFunc(search);
  }

  // Read this https://github.com/Microsoft/TypeScript/wiki/FAQ#why-does-this-get-orphaned-in-my-instance-methods
  getContacts = (filter = '') => {

    if (this.contactsListSubscription && !this.contactsListSubscription.closed) {
      this.contactsListSubscription.unsubscribe();
    }
    this.contactsListSubscription = this.contactsService.getAllAsList('readallcbo', filter, this.currentContactsSelectPage, 50, this.selectedContactId)
      .subscribe((response: { count: number, payload: ListContactModel[] }) => {
        this.contacts = response.payload.slice();
        this.filteredContacts$.next(this.contacts);
        // Reseting selected contact Id
        this.selectedContactId = 0;
      },
        (error) => this.snackBar.open('Oops, there was an error fetching contacts', 'close', { duration: 1000 }));
  }

  newContact(): void {
    this.contactDialogRef = this.dialog.open(ContactFormComponent, {
      panelClass: 'contact-form-dialog',
      data: {
        action: 'create',
        preRequesterSelected: this.preRequesterSelectedData
      }
    });

    this.contactDialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        this.contactsService.create(response.getRawValue())
          .subscribe((contact: any) => {
            if (contact.status === 200) {
              console.log('contact', contact);
              // TODO FIX
              this.contacts.push({
                'id': contact.body.id, 'name': contact.body.fullName, 'fullName': contact.body.fullName, 'email': contact.body.email, 'phone': contact.body.phone,
                'fullAddress': contact.body.fulladdress
              });
              this.filteredContacts$.next(this.contacts);
              this.selectedContactId = contact.body.id;
              this.selectedContact = contact.body;
              this.searchContactForm.get('contactId').setValue(contact.body.id);
              this.snackBar.open('Contact created successfully!!!', 'close', { duration: 1000 });
            }
          },
            (error) => {
              if (error.status === 412) {
                this.snackBar.open(error.error, 'close', { duration: 5000 });
              }
              else { this.snackBar.open('Oops, there was an error', 'close', { duration: 2000 }); }
            });
      });
  }
}
