import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fuseAnimations } from '@fuse/animations';
import { Subscription } from 'rxjs';
import { UsersService } from './users.service';
import { FuseSidebarService } from '../../../../../@fuse/components/sidebar/sidebar.service';
import { UserFormComponent } from './user-form/user-form.component';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class UsersComponent implements OnInit, OnDestroy {

  hasSelectedUsers: boolean;
  searchInput: FormControl;
  dialogRef: any;
  selectedElementsChangedSubscription: Subscription;

  get readOnly(): any {
    return localStorage.getItem('readOnly') === null ? false : true;
  }

  constructor(
    private usersService: UsersService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private _fuseSidebarService: FuseSidebarService
  ) {
    this.searchInput = new FormControl(this.usersService.searchText);
  }

  ngOnInit(): void {
    this.selectedElementsChangedSubscription =
      this.usersService.selectedElementsChanged
        .subscribe(selectedUsers => {
          this.hasSelectedUsers = selectedUsers.length > 0;
        });

    this.searchInput.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged())
      .subscribe(searchText => {
        this.usersService.searchTextChanged.next(searchText);
      });
  }

  ngOnDestroy(): void {
    this.selectedElementsChangedSubscription.unsubscribe();

  }

  newUser(): void {
    this.dialogRef = this.dialog.open(UserFormComponent, {
      panelClass: 'user-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.dialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        this.usersService.createUser(response.getRawValue())
          .then(
            () => this.snackBar.open('User created successfully!!!', 'close', { duration: 1000 }),
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });

  }

  /**
   * Toggle sidebar
   *
   * @param name
   */
  toggleSidebar(name): void {
    this._fuseSidebarService.getSidebar(name).toggleOpen();
  }
}
