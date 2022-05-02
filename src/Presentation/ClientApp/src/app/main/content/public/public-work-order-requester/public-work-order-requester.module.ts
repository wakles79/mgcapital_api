import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PublicWorkOrderRequesterComponent } from './public-work-order-requester.component';
import { Routes, RouterModule } from '@angular/router';
import { PublicWorkOrderRequesterResolver } from './public-work-order-requester-resolver';
import { FuseSharedModule } from '@fuse/shared.module';
import { LightboxModule } from 'ngx-lightbox';
import { PipesModule } from '@app/core/pipes/pipes.module';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressBarModule } from '@angular/material/progress-bar';

const routes: Routes = [
  {
    path: 'external-work-orders/:guid',
    component: PublicWorkOrderRequesterComponent,
    resolve: {
      workOrder: PublicWorkOrderRequesterResolver
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
    LightboxModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    MatProgressBarModule,

    // Add pipes
    PipesModule

  ],
  declarations: [
    PublicWorkOrderRequesterComponent
  ],

})
export class PublicWorkOrderRequesterModule { }
