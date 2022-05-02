import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fuseAnimations } from '@fuse/animations';
import { BehaviorSubject } from 'rxjs';
import { RoleFormComponent } from './role-form/role-form.component';
import { RolesService } from './roles.service';
import { RoleBaseModel } from '@app/core/models/role/role-base.model';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class RolesComponent implements OnInit {

  selectedRoleId: number = 0;
  roles: { id: number, name: string, level: number, isSelected: boolean, type: number }[] = [];

  selectedModuleId: number = 0;
  modules: { module: number, moduleName: string, permissions: any[] }[] = [];

  modulePermissions: { id: number, name: string, fullName: string, module: number, type: number, isAssigned: boolean }[] = [];

  loading$ = new BehaviorSubject<boolean>(false);

  get readOnly(): any {
    return localStorage.getItem('readOnly') === null ? false : true;
  }

  roleFormDialog: MatDialogRef<RoleFormComponent>;

  constructor(
    private rolesService: RolesService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private _fuseSidebarService: FuseSidebarService
  ) {

  }

  ngOnInit(): void {
    this.getRoles();
  }

  getModuleFeatureFlag(moduleName: string): string {
    if (!moduleName) {
      return '';
    }
    return `permission-${moduleName.toLowerCase().split(' ').join('-')}`;
  }

  // Roles
  getRoles(): void {
    this.loading$.next(true);
    this.rolesService.getRoles()
      .subscribe((result: { id: number, name: string, level: number, isSelected: boolean, type: number }[]) => {
        this.roles = result;
        this.loading$.next(false);
      });
  }

  newRole(): void {
    this.roleFormDialog = this.dialog.open(RoleFormComponent, {
      panelClass: 'role-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.roleFormDialog.afterClosed()
      .subscribe((result: { action: string, data: FormGroup }) => {
        if (!result) {
          return;
        }


        if (result.action === 'save') {
          this.rolesService.create(result.data.getRawValue(), 'AddRole').subscribe(() => {
            this.snackBar.open('Role added Successfully', 'close', { duration: 1000 });
            this.getRoles();
          }, (error) => {
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            this.getRoles();
          });
        }
      });
  }

  editRole(id: number): any {
    event.stopPropagation();
    this.rolesService.get(id, 'GetRole')
      .subscribe((role: RoleBaseModel) => {

        if (!role) {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          return;
        }

        this.roleFormDialog = this.dialog.open(RoleFormComponent, {
          panelClass: 'role-form-dialog',
          data: {
            action: 'edit',
            role: role
          }
        });

        this.roleFormDialog.afterClosed()
          .subscribe((result: { action: string, data: FormGroup }) => {
            if (!result) {
              return;
            }

            if (result.action === 'save') {
              this.rolesService.update(result.data.getRawValue(), 'UpdateRole')
                .subscribe(() => {
                  this.snackBar.open('Role updated Successfully', 'close', { duration: 1000 });
                  this.getRoles();
                }, (error) => {
                  this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                  this.getRoles();
                });
            } else if (result.action === 'remove') {

              this.rolesService.removeRole(role.id)
                .subscribe(() => {
                  this.snackBar.open('Role removed Successfully', 'close', { duration: 1000 });
                  this.getRoles();

                  if (this.selectedRoleId === role.id) {
                    this.selectedRoleId = 0;
                    this.selectedModuleId = 0;
                    this.modules = [];
                    this.modulePermissions = [];
                  }
                }, (error) => {
                  this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                  this.getRoles();
                });

            }
          });

      }, (error) => {
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

  selectedRoleChanged(roleId: number): void {
    this.selectedRoleId = roleId;
    this.selectedModuleId = 0;
    this.getRoleModuleAccess(roleId);
  }

  get selectedRoleName(): any {
    if (this.selectedRoleId > 0) {
      return this.roles.filter(r => r.id === this.selectedRoleId)[0].name;
    } else {
      return 'Select a role to display';
    }
  }

  // Permissions
  getRoleModuleAccess(roleId: number): void {
    this.modules = [];
    this.loading$.next(true);
    this.rolesService.getModuleAccessByRole(roleId)
      .subscribe((result: { module: number, moduleName: string, permissions: any[] }[]) => {
        this.modules = result;
        this.moduleChanged(this.selectedModuleId);
        this.loading$.next(false);
      });
  }

  updateModuleAccess(accessType: number): void {

    if (this.readOnly) {
      this.snackBar.open('Ops! doesnt have the permissions to run this function!!!', 'close', { duration: 1000 });
      return;
    }

    if (accessType > 2) {
      return;
    }

    if (this.selectedRoleId === 0) {
      this.snackBar.open('Select a role', 'close', { duration: 2000 });
      return;
    }

    this.loading$.next(true);
    this.rolesService.updateRoleModuleAccess({ roleId: this.selectedRoleId, module: this.selectedModuleId, type: accessType })
      .subscribe(() => {
        this.snackBar.open('Access updated successfully!', 'close', { duration: 1000 });
        this.getRoleModuleAccess(this.selectedRoleId);
      }, (error) => {
        this.loading$.next(false);
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });

  }

  moduleChanged(appModule: number): void {
    this.selectedModuleId = appModule;
    this.modulePermissions = this.modules.filter(m => m.module === appModule)[0].permissions;
  }

  updateModulePermission(event: MatCheckboxChange, permissionId: number): void {

    if (this.readOnly) {
      this.snackBar.open('Ops! doesnt have the permissions to run this function!!!', 'close', { duration: 1000 });
      return;
    }

    this.rolesService
      .updateRoleModulePermission({ permissionId: permissionId, roleId: this.selectedRoleId, isAssigned: event.checked }).subscribe(
        () => {
          this.snackBar.open('Access updated successfully!', 'close', { duration: 1000 });
          this.getRoleModuleAccess(this.selectedRoleId);
        }, (error) => {
          this.loading$.next(false);
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
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
