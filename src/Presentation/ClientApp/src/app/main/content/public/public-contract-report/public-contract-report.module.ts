import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PublicContractReportComponent } from './public-contract-report.component';
import { Routes, RouterModule } from '@angular/router';
import { FuseSharedModule } from '@fuse/shared.module';
import { PublicContractReportResolver } from './public-contract-report-resolver';
import { LightboxModule } from 'ngx-lightbox';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';

const routes: Routes = [
  {
    path: 'contracts/contract-report/:guid',
    component: PublicContractReportComponent,
    resolve: {
      report: PublicContractReportResolver
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
    MatListModule,
    MatCardModule,
    MatTableModule,
  ],
  declarations: [
    PublicContractReportComponent
  ],
  providers: [

  ]

})
export class PublicContractReportModule { }
