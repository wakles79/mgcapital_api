import { Component, OnInit, ViewEncapsulation, OnDestroy, Inject } from '@angular/core';
import { FormGroup, AbstractControl, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { CustomerBaseModel } from '@app/core/models/customer/customer-base.model';
import { ProposalBaseModel } from '@app/core/models/proposal/proposal-base.model';
import { fuseAnimations } from '@fuse/animations';
import { Subject, Subscription } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ContactsService } from '../../contacts/contacts.service';
import { CustomersService } from '../../customers/customers.service';

@Component({
  selector: 'app-proposal-form',
  templateUrl: './proposal-form.component.html',
  styleUrls: ['./proposal-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ProposalFormComponent implements OnInit, OnDestroy {

  proposalForm: FormGroup;
  proposal: ProposalBaseModel;

  action: string;
  dialogTitle: string;
  private _onDestroy = new Subject<void>();

  get customerFilter(): AbstractControl { return this.proposalForm.get('customerFilter'); }
  customers: ListItemModel[] = [];
  filteredCustomers: Subject<any[]> = new Subject<any[]>();
  listCustomersSubscription: Subscription;
  currentCustomerSelected: -1;

  get contactFilter(): AbstractControl { return this.proposalForm.get('contactFilter'); }
  contacts: ListItemModel[] = [];
  filteredContacts: Subject<any[]> = new Subject<any[]>();
  listContactsSubscription: Subscription;
  currentContactSelected: -1;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    public dialogRef: MatDialogRef<ProposalFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private customerService: CustomersService,
    private contactService: ContactsService,
    public snackBar: MatSnackBar
  ) {
    this.action = data.action;

    if (this.action === 'new') {
      this.dialogTitle = 'New Proposal';
      this.proposalForm = this.createProposalForm();
    } else {
      this.dialogTitle = 'Edit Proposal';
      this.proposal = data.proposal;
      this.proposalForm = this.updateProposalForm();
    }
  }

  ngOnInit(): void {
    this.getCustomers();

    this.getContacts();

    this.customerFilter.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterCustomers();
      });

    this.contactFilter.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterContacts();
      });
  }

  ngOnDestroy(): void {
    if (this.listCustomersSubscription && !this.listCustomersSubscription.closed) {
      this.listCustomersSubscription.unsubscribe();
    }

    if (this.listContactsSubscription && !this.listContactsSubscription.closed) {
      this.listContactsSubscription.unsubscribe();
    }
  }

  /** FORM */
  createProposalForm(): FormGroup {
    return this.formBuilder.group({
      customerFilter: [''],
      contactFilter: [''],
      customerId: [''],
      customerName: ['', [Validators.required]],
      customerEmail: ['', [Validators.required, Validators.email]],
      contactId: ['', [Validators.required]],
      location: ['', [Validators.required]],
      status: [0, [Validators.required]]
    });
  }

  updateProposalForm(): FormGroup {
    return this.formBuilder.group({
      customerFilter: [''],
      contactFilter: [''],
      id: [this.proposal.id],
      customerId: [{ value: this.proposal.customerId, disabled: this.readOnly }, [Validators.required]],
      customerName: [{ value: this.proposal.customerName, disabled: this.readOnly }, [Validators.required]],
      customerEmail: [{ value: this.proposal.customerEmail, disabled: this.readOnly }, [Validators.required, Validators.email]],
      contactId: [{ value: this.proposal.contactId, disabled: this.readOnly }, [Validators.required]],
      location: [{ value: this.proposal.location, disabled: this.readOnly }, [Validators.required]],
      status: [{ value: this.proposal.status, disabled: this.readOnly }, [Validators.required]]
    });
  }

  /** CUSTOMERS */
  getCustomers(): void {
    if (this.listCustomersSubscription && !this.listCustomersSubscription.closed) {
      this.listCustomersSubscription.unsubscribe();
    }

    this.listCustomersSubscription = this.customerService.getAllAsList('readallcbo', '', 0, 99999, null)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.customers = response.payload;
        this.filteredCustomers.next(this.customers);
      });
  }

  private filterCustomers(): void {
    if (!this.customers) {
      return;
    }
    // get the search keyword
    let search = this.customerFilter.value;
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

  customersChanged(id: any): void {
    if (!id) {
      return;
    }

    this.customerService.get(id, 'get')
      .subscribe((customerData: any) => {
        if (!customerData) {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          return;
        }

        const customer = new CustomerBaseModel(customerData);
        this.proposalForm.patchValue({
          customerName: customer.name,
          customerEmail: ''
        });
      });
  }

  /** CONTACTS */
  getContacts(): void {
    if (this.listContactsSubscription && !this.listContactsSubscription.closed) {
      this.listContactsSubscription.unsubscribe();
    }

    this.listContactsSubscription = this.contactService.getAllAsList('readallcbo', '', 0, 99999, null)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.contacts = response.payload;
        this.filteredContacts.next(this.contacts);
      });
  }

  private filterContacts(): void {
    if (!this.contacts) {
      return;
    }
    // get the search keyword
    let search = this.contactFilter.value;
    if (!search) {
      this.filteredContacts.next(this.contacts.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the customers
    this.filteredContacts.next(
      this.contacts.filter(contact => contact.name.toLowerCase().indexOf(search) > -1)
    );
  }

}
