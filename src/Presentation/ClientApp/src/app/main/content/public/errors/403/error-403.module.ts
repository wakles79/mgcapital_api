import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { FuseError403Component } from './error-403.component';
import { FuseSharedModule } from '@fuse/shared.module';

const routes = [
  {
    path: 'errors/error-403',
    component: FuseError403Component
  }
];

@NgModule({
  declarations: [
    FuseError403Component
  ],
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes)
  ]
})

export class Error403Module {

}
