import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PublicWorkOrderComponent } from './public-work-order.component';
import { PublicWorkOrderResolver } from './public-work-order-resolver';
import { RouterModule } from '@angular/router';
import { LightboxModule } from 'ngx-lightbox';
import { PipesModule } from '../../../../core/pipes/pipes.module';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatExpansionModule } from '@angular/material/expansion';
import { FuseSharedModule } from '@fuse/shared.module';

const routes = [
  {
    path: 'work-orders/:guid',
    component: PublicWorkOrderComponent,
    resolve: {
      workOrder: PublicWorkOrderResolver
    }
  }
];

@NgModule({
  declarations: [
    PublicWorkOrderComponent
  ],
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
    LightboxModule,

    MatIconModule,
    MatProgressBarModule,
    MatCheckboxModule,
    MatExpansionModule,

    // Pipes module
    PipesModule
  ]
})
export class PublicWorkOrderModule { }
