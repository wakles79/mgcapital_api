import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { FuseLockComponent } from './lock.component';
import { FuseSharedModule } from '@fuse/shared.module';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

const routes = [
  {
    path: 'auth/lock',
    component: FuseLockComponent
  }
];

@NgModule({
  declarations: [
    FuseLockComponent
  ],
  imports: [
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    FuseSharedModule,
    RouterModule.forChild(routes)
  ]
})

export class LockModule {

}
