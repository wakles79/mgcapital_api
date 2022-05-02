import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PublicCleaningReportComponent } from './public-cleaning-report.component';
import { PublicCleaningReportResolver } from './public-cleaning-report.resolver';
import { FuseSharedModule } from '@fuse/shared.module';
import { RouterModule } from '@angular/router';
import { LightboxModule } from 'ngx-lightbox';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatExpansionModule } from '@angular/material/expansion';
import { PipesModule } from '@app/core/pipes/pipes.module';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

const routes = [
  {
    path: 'reports/cleaning-report/:guid',
    component: PublicCleaningReportComponent,
    resolve: {
      report: PublicCleaningReportResolver
    }
  }
];

@NgModule({
  declarations: [
    PublicCleaningReportComponent
  ],
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
    LightboxModule,

    MatIconModule,
    MatButtonModule,
    MatProgressBarModule,
    MatExpansionModule,
    MatTableModule,
    CdkTableModule,
    MatPaginatorModule,
    MatTooltipModule,
    MatInputModule,

    // Pipes module
    PipesModule
  ]
})
export class PublicCleaningReportModule { }
