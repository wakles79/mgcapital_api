import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { FuseMaintenanceComponent } from './maintenance.component';
import { FuseSharedModule } from '@fuse/shared.module';
import { MatButtonModule } from '@angular/material/button';

const routes = [
  {
    path: 'maintenance',
    component: FuseMaintenanceComponent
  }
];

@NgModule({
  declarations: [
    FuseMaintenanceComponent
  ],
  imports: [
    MatButtonModule,
    FuseSharedModule,
    RouterModule.forChild(routes)
  ]
})

export class MaintenanceModule {

}
