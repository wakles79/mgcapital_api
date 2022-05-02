import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RoleBaseModel } from '@app/core/models/role/role-base.model';
import { fuseAnimations } from '@fuse/animations';
import { RolesService } from '../roles.service';

@Component({
  selector: 'app-role-form',
  templateUrl: './role-form.component.html',
  styleUrls: ['./role-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class RoleFormComponent implements OnInit {

  action: string;
  dialogTitle: string;

  role: RoleBaseModel;
  roleForm: FormGroup;

  roles: { id: number, name: string, level: number }[] = [];

  get readOnly(): any {
    return localStorage.getItem('readOnly') === null ? false : true;
  }

  constructor(
    public dialogRef: MatDialogRef<RoleFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private roleService: RolesService
  ) {
    this.action = data.action;

    if (this.action === 'new') {
      this.dialogTitle = 'New Role';
      this.roleForm = this.createRoleForm();
    } else {
      this.dialogTitle = 'Update Role';
      this.role = data.role;
      this.roleForm = this.updateRoleForm();
    }
  }

  ngOnInit(): void {
    this.loadRoles();
  }

  loadRoles(): void {
    this.roleService.getDefaultRoles()
      .subscribe((response: { id: number, name: string, level: number }[]) => {
        this.roles = response;
      }, () => {

      });
  }

  createRoleForm(): FormGroup {
    return this.formBuilder.group({
      level: ['', [Validators.required]],
      name: ['', [Validators.required]],
      type: [1]
    });
  }

  updateRoleForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.role.id],
      level: [this.role.level, [Validators.required]],
      name: [this.role.name, [Validators.required]],
      type: [this.role.type]
    });
  }

  // Actions
  save(): void {
    this.dialogRef.close({ action: 'save', data: this.roleForm });
  }

  remove(): void {
    this.dialogRef.close({ action: 'remove', data: this.roleForm });
  }
}
