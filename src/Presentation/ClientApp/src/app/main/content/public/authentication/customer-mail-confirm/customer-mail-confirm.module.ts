import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { FuseCustomerMailConfirmComponent } from './customer-mail-confirm.component';
import { FuseSharedModule } from '@fuse/shared.module';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

const routes = [
  {
    path: 'auth/customer-mail-confirm',
    component: FuseCustomerMailConfirmComponent
  }
];

@NgModule({
  declarations: [
    FuseCustomerMailConfirmComponent
  ],
  imports: [
    FuseSharedModule,
    // Material Modules
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    RouterModule.forChild(routes)
  ]
})

export class CustomerMailConfirmModule {

}
