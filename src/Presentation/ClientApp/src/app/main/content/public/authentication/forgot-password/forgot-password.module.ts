import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { FuseForgotPasswordComponent } from './forgot-password.component';
import { FuseSharedModule } from '@fuse/shared.module';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

const routes = [
  {
    path: 'auth/forgot-password',
    component: FuseForgotPasswordComponent
  }
];

@NgModule({
  declarations: [
    FuseForgotPasswordComponent
  ],
  imports: [
    // Material Modules
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    FuseSharedModule,
    RouterModule.forChild(routes)
  ]
})

export class ForgotPasswordModule {

}
