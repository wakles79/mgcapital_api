import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { FuseRegisterComponent } from './register.component';

import { FuseSharedModule } from '@fuse/shared.module';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';

const routes = [
  {
    path: 'auth/register',
    component: FuseRegisterComponent
  }
];

@NgModule({
  declarations: [
    FuseRegisterComponent
  ],
  imports: [
    // Material Modules
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
    FuseSharedModule,
    RouterModule.forChild(routes)
  ]
})

export class RegisterModule {

}
