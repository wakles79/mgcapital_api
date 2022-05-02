import { Component, OnInit, ViewEncapsulation, OnDestroy, Inject } from '@angular/core';
import { FormGroup, AbstractControl, FormBuilder, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { CleaningReportCreateModel } from '@app/core/models/reports/cleaning-report/cleaning.report.create.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { Subject, Subscription } from 'rxjs';
import { UsersService } from '@app/main/content/private/users/users.service';
import { CleaningReportService } from '../cleaning-report.service';
import { ContactsService } from '@app/main/content/private/contacts/contacts.service';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import * as moment from 'moment';

@Component({
  selector: 'app-cleaning-report-form',
  templateUrl: './cleaning-report-form.component.html',
  styleUrls: ['./cleaning-report-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class CleaningReportFormComponent implements OnInit, OnDestroy {

  action: string;
  dialogTitle: string;
  cleaningReportForm: FormGroup;

  report: CleaningReportCreateModel;

  private _onDestroy = new Subject<void>();

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  dialogCustomerRef: any;

  employees: { id: number, name: string }[] = [];
  filteredEmployees$: Subject<ListItemModel[]> = new Subject<ListItemModel[]>();
  listEmployeesSubscription: Subscription;

  contacts: { id: number, name: string }[] = [];
  filteredContacts$: Subject<ListItemModel[]> = new Subject<ListItemModel[]>();
  listContactsSubscription: Subscription;

  get employeesCtrl(): AbstractControl { return this.cleaningReportForm.get('employeesCtrl'); }

  get contactsCtrl(): AbstractControl { return this.cleaningReportForm.get('contactsCtrl'); }

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<CleaningReportFormComponent>,
    public snackBar: MatSnackBar,
    private cleaningReportService: CleaningReportService,
    private employeesService: UsersService,
    private contactsService: ContactsService
  ) {
    this.action = data.action;

    if (this.action === 'new') {
      this.dialogTitle = 'New Cleaning Report';
      this.cleaningReportForm = this.cleaningReportCreateForm();
    }
    else {
      this.dialogTitle = 'Edit Cleaning Report';
      this.report = data.report;
      this.cleaningReportForm = this.cleaningReportUpdateForm();
    }
  }

  ngOnInit(): void {
    this.getEmployees();
    this.getContacts();

    this.contactsCtrl.valueChanges
    .pipe(
      debounceTime(300),
      distinctUntilChanged())
      .subscribe(() => {
        this.filterContacts();
      });

    this.employeesCtrl.valueChanges
    .pipe(
      debounceTime(300),
      distinctUntilChanged())
      .subscribe(() => {
        this.filterEmployees();
      });
  }

  cleaningReportCreateForm(): FormGroup {
    return this.formBuilder.group({
      contactsCtrl: [''],
      employeesCtrl: [''],
      employeeId: ['', Validators.required],
      contactId: ['', Validators.required],
      location: ['', Validators.required],
      dateOfService: [moment().add(-1, 'd').toDate(), Validators.required],
    });
  }

  cleaningReportUpdateForm(): FormGroup {
    return this.formBuilder.group({
      contactsCtrl: [''],
      employeesCtrl: [''],
      id: [this.report.id],
      employeeId: [{ value: this.report.employeeId, disabled: this.readOnly }, Validators.required],
      contactId: [{ value: this.report.contactId, disabled: this.readOnly }, Validators.required],
      location: [{ value: this.report.location, disabled: this.readOnly }, Validators.required],
      dateOfService: [{ value: this.report.dateOfService, disabled: this.readOnly }, Validators.required],
    });
  }

  getEmployees(): void {
    const employeeId = this.action === 'edit' ? this.report.employeeId : null;

    if (this.listEmployeesSubscription && !this.listEmployeesSubscription.closed) {
      this.listEmployeesSubscription.unsubscribe();
    }

    this.listEmployeesSubscription =
      this.employeesService.getAllAsList('readallemployeebyroleandcomparisonvaluecbo', '', 0, 999, employeeId,
        { roleLevel: '20', 'comparisonValue': 'LessThanOrEqualTo' })
        .subscribe((response: { count: number, payload: ListItemModel[] }) => {
          this.employees = response.payload;
          this.filteredEmployees$.next(this.employees);
        });
  }

  private filterEmployees(): void {
    if (!this.employees) {
      return;
    }

    // get the search keyword
    let search = this.employeesCtrl.value;
    if (!search) {
      this.filteredEmployees$.next(this.employees.slice());
      return;
    } else {
      search = search.toLowerCase();
    }

    // filter the customers
    this.filteredEmployees$.next(
      this.employees.filter(customer => customer.name.toLowerCase().indexOf(search) > -1)
    );
  }

  getContacts(): void {
    const contactId = this.action === 'edit' ? this.report.contactId : null;

    if (this.listContactsSubscription && !this.listContactsSubscription.closed) {
      this.listContactsSubscription.unsubscribe();
    }

    this.listContactsSubscription = this.contactsService.getAllAsList('readallbldgcontactcbo', '', 0, 999, contactId, { 'contactType': 'Property Manager' })
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.contacts = response.payload;
        this.filteredContacts$.next(this.contacts);
      });
  }

  private filterContacts(): void {
    if (!this.contacts) {
      return;
    }

    // get the search keyword
    let search = this.contactsCtrl.value;
    if (!search) {
      this.filteredContacts$.next(this.contacts.slice());
      return;
    } else {
      search = search.toLowerCase();
    }

    // filter the customers
    this.filteredContacts$.next(
      this.contacts.filter(contact => contact.name.toLowerCase().indexOf(search) > -1)
    );
  }


  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

}
