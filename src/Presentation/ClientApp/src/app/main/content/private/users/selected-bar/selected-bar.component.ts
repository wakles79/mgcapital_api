import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MultipleDeleteConfirmDialogComponent } from '@app/core/modules/delete-confirm-dialog/multiple-delete-confirm-dialog/multiple-delete-confirm-dialog.component';
import { fuseAnimations } from '@fuse/animations';
import { UsersService } from '../users.service';

@Component({
  selector: 'app-selected-bar',
  templateUrl: './selected-bar.component.html',
  styleUrls: ['./selected-bar.component.scss'],
  animations: fuseAnimations
})
export class SelectedBarComponent implements OnInit {

  selectedUsers: string[];
  hasSelectedUsers: boolean;
  isIndeterminate: boolean;
  confirmDialogRef: MatDialogRef<MultipleDeleteConfirmDialogComponent>;

  constructor(
    private usersService: UsersService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {
    this.usersService.selectedElementsChanged
      .subscribe(selectedUsers => {
        this.selectedUsers = selectedUsers;
        setTimeout(() => {
          this.hasSelectedUsers = selectedUsers.length > 0;
          this.isIndeterminate = (selectedUsers.length !== this.usersService.allElementsToList.length && selectedUsers.length > 0);
        }, 0);
      });
  }

  ngOnInit(): void {
  }

  selectAll(): void {
    this.usersService.getSelectedElements();
  }

  deselectAll(): void {
    this.usersService.deselectElements();
  }

  deleteSelectedElements(): void {

    const toDelete = [];
    this.selectedUsers.forEach(guid => {
      toDelete.push(this.usersService.deleteUserByGuid(guid));
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
      if (!result) {
        return;
      }

      this.deselectAll();
      this.usersService.getElements();
    });
  }

}
