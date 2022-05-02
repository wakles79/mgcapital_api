import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fuseAnimations } from '@fuse/animations';
import { Subscription } from 'rxjs';
import { ContactsService } from './contacts.service';
import { ContactFormComponent } from '@app/core/modules/contact-form/contact-form/contact-form.component';

@Component({
  selector: 'app-contacts',
  templateUrl: './contacts.component.html',
  styleUrls: ['./contacts.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ContactsComponent implements OnInit, OnDestroy {

  hasSelectedContacts: boolean;
  searchInput: FormControl;
  dialogRef: any;
  onSelectedContactsChangedSubscription: Subscription;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    private contactsService: ContactsService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {
    this.searchInput = new FormControl(this.contactsService.searchText);
  }

  newContact(): void {
    this.dialogRef = this.dialog.open(ContactFormComponent, {
      panelClass: 'contact-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.dialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        this.contactsService.createElement(response.getRawValue())
          .then(
            () => this.snackBar.open('Contact created successfully!!!', 'close', { duration: 1000 }),
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });

  }

  ngOnInit(): void {
    this.onSelectedContactsChangedSubscription =
      this.contactsService.selectedElementsChanged
        .subscribe(selectedContacts => {
          this.hasSelectedContacts = selectedContacts.length > 0;
        });
  }

  ngOnDestroy(): void {
    this.onSelectedContactsChangedSubscription.unsubscribe();
  }

}
