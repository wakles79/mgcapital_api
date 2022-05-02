import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MultipleDeleteConfirmDialogComponent } from '@app/core/modules/delete-confirm-dialog/multiple-delete-confirm-dialog/multiple-delete-confirm-dialog.component';
import { ContactsService } from '../contacts.service';

@Component({
  selector: 'app-selected-bar-contacts',
  templateUrl: './selected-bar.component.html',
  styleUrls: ['./selected-bar.component.scss']
})
export class ContactsSelectedBarComponent implements OnInit {

  selectedContacts: string[];
  hasSelectedContacts: boolean;
  isIndeterminate: boolean;
  confirmDialogRef: MatDialogRef<MultipleDeleteConfirmDialogComponent>;

  constructor(
    private contactsService: ContactsService,
    public dialog: MatDialog
  ) {
    this.contactsService.selectedElementsChanged
      .subscribe(selectedContacts => {
        this.selectedContacts = selectedContacts;
        setTimeout(() => {
          this.hasSelectedContacts = selectedContacts.length > 0;
          this.isIndeterminate = (selectedContacts.length !== this.contactsService.allElementsToList.length && selectedContacts.length > 0);
        }, 0);
      });

  }

  ngOnInit(): void {
  }

  selectAll(): void {
    this.contactsService.getSelectedElements();
  }

  deselectAll(): void {
    this.contactsService.deselectElements();
  }

  deleteSelectedContacts(): void {

    const toDelete = [];
    this.selectedContacts.forEach(guid => {
      toDelete.push(this.contactsService.deleteByGuid(guid));
    });

    this.confirmDialogRef = this.dialog.open(MultipleDeleteConfirmDialogComponent, {
      panelClass: 'multiple-delete-dialog',
      disableClose: false,
      data: {
        title: 'Delete multiple users',
        requests: toDelete
      }
    });


    this.confirmDialogRef.afterClosed().subscribe((result) => {
      this.deselectAll();
      this.contactsService.getElements();
    });
  }

}
