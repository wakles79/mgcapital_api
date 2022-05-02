import { AfterViewInit, Component, Input, OnDestroy, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { UserBaseModel } from '@app/core/models/user/user-base.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { forkJoin, merge, Observable, Subscription } from 'rxjs';
import { ShareBuildingDialogComponent } from '../share-building-dialog/share-building-dialog.component';
import { UserGridModel } from '@app/core/models/user/user-grid.model';
import { UsersService } from '../users.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '@app/core/services/auth.service';
import { DataSource } from '@angular/cdk/table';
import { FormGroup } from '@angular/forms';
import { UserFormComponent } from '../user-form/user-form.component';
import { tap } from 'rxjs/operators';


@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class UserListComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @Input() readOnly: boolean;

  users: UserGridModel[];
  user: UserBaseModel;
  dataSource: UsersDataSource | null;
  displayedColumns = ['checkbox', 'fullName', 'email', 'phone', 'roleName', 'buttons'];
  selectedUsers: any[];
  checkboxes: {};

  allElementsChangedSubscription: Subscription;
  selectedElementsChangedSubscription: Subscription;
  onUserDataChangedSubscription: Subscription;

  dialogRef: any;
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  shareBuildingDialog: MatDialogRef<ShareBuildingDialogComponent>;

  get usersCount(): number { return this.usersService.elementsCount; }

  // Level Role
  roleLevelLoggedUser: number;
  constructor(
    private usersService: UsersService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private authService: AuthService
  ) {
    this.allElementsChangedSubscription =
      this.usersService.allElementsChanged.subscribe(users => {

        this.users = users;

        this.checkboxes = {};
        users.map(user => {
          this.checkboxes[user.guid] = false;
        });
      });

    this.selectedElementsChangedSubscription =
      this.usersService.selectedElementsChanged.subscribe(selectedUsers => {
        for (const guid in this.checkboxes) {
          if (!this.checkboxes.hasOwnProperty(guid)) {
            continue;
          }

          this.checkboxes[guid] = selectedUsers.includes(guid);
        }
        this.selectedUsers = selectedUsers;
      });

    this.onUserDataChangedSubscription =
      this.usersService.onUserDataChanged.subscribe(user => {
        this.user = user;
      });
  }

  ngOnInit(): void {
    this.roleLevelLoggedUser = this.authService.currentUser.roleLevel;
    this.dataSource = new UsersDataSource(this.usersService);
  }
  ngAfterViewInit(): void {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => {
      this.paginator.pageIndex = 0;
      console.log('change paginator');
    }
    );

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.usersService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize))
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.allElementsChangedSubscription.unsubscribe();
    this.selectedElementsChangedSubscription.unsubscribe();
    this.onUserDataChangedSubscription.unsubscribe();
  }


  editUser(user): void {
    this.usersService.get(user.id, 'update')
      .subscribe((userData: any) => {
        if (userData) {
          const userUpdateObj = new UserBaseModel(userData);
          this.dialogRef = this.dialog.open(UserFormComponent, {
            panelClass: 'user-form-dialog',
            data: {
              user: userUpdateObj,
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
              const updatedUserObj = new UserBaseModel(formData.getRawValue());
              switch (actionType) {
                /**
                 * Save
                 */
                case 'save':

                  this.usersService.updateElement(updatedUserObj)
                    .then(
                      () => this.snackBar.open('User updated successfully!!!', 'close', { duration: 1000 }),
                      () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
                    .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));

                  break;
                /**
                 * Delete
                 */
                case 'delete':

                  this.deleteElement(userUpdateObj);

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

  sendCredentials(user): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = `Are you sure you send credentials to ${user.email}?`;

    this.confirmDialogRef.afterClosed()
      .subscribe(result => {
        if (result) {
          this.usersService.sendCredentials(user)
            .subscribe((r) => {
              this.snackBar.open('Credentials sent successfully!!!', 'close', { duration: 1000 });
            },
              (error) => {
                this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
              });
        }
        this.confirmDialogRef = null;
      });
  }

  /**
   * Delete User
   */
  deleteElement(user): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.usersService.loadingSubject.next(true);
        this.usersService.delete(user)
          .then(response => {
            this.usersService.loadingSubject.next(false);
            this.snackBar.open('User successfully deleted', 'close', { duration: 2000 });
          })
          .catch(error => {
            this.usersService.loadingSubject.next(false);
            this.snackBar.open(error, 'close', { duration: 5000 });
          });
      }
      this.confirmDialogRef = null;
    });

  }

  onSelectedChange(userGuid): void {
    this.usersService.toggleSelectedElement(userGuid);
  }

  /** Share Building */
  shareBuilding(userId: number): void {
    this.shareBuildingDialog = this.dialog.open(ShareBuildingDialogComponent, {
      panelClass: 'share-building-dialog',
      data: {
        currentUser: userId
      }
    });

    this.shareBuildingDialog.afterClosed()
      .subscribe((result: any[]) => {
        if (!result) {
          console.log('no result');
          return;
        }

        if (result.length > 0) {
          forkJoin(result).subscribe(
            (response: any[]) => {
              this.snackBar.open('Buildings shared successfully', 'close', { duration: 2000 });
            }, (error) => {
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            }
          );
        }
      });
  }

  shareBuildingByOp(userId: number): void {
    this.shareBuildingDialog = this.dialog.open(ShareBuildingDialogComponent, {
      panelClass: 'share-building-dialog',
      data: {
        currentUser: this.authService.currentUser.employeeId,
        targetUser: userId,
        fromOp: true
      }
    });
  }

}

export class UsersDataSource extends DataSource<any>
{
  constructor(private usersService: UsersService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.usersService.allElementsChanged;
  }

  disconnect(): void {
  }
}
