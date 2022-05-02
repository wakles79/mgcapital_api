import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InspectionDetailComponent } from './inspection-detail.component';
import { RouterModule, Routes } from '@angular/router';
import { PublicInspectionDetailResolver } from './public-inspection-detail-resolver';
import { FuseSharedModule } from '@fuse/shared.module';
import { LightboxModule } from 'ngx-lightbox';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatExpansionModule } from '@angular/material/expansion';
import { PipesModule } from '@app/core/pipes/pipes.module';
import { MatCardModule } from '@angular/material/card';

const routes: Routes = [
  {
    path: 'inspections/inspection-detail/:guid',
    component: InspectionDetailComponent,
    resolve: {
      resolver: PublicInspectionDetailResolver
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
    LightboxModule,

    MatIconModule,
    MatProgressBarModule,
    MatCheckboxModule,
    MatExpansionModule,
    MatCardModule,

    // Pipes module
    PipesModule
  ],
  declarations: [
    InspectionDetailComponent
  ], providers: [
    // InspectionsDetailService,
    // PublicInspectionDetailResolver
  ]
})
export class PublicInspectionDetailModule { }
