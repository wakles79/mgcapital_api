import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PublicDailyReportOperationsManagerComponent } from './public-daily-report-operations-manager.component';
import { PublicDailyReportOperationsManagerResolver } from './public-daily-report-om-resolver';
import { Routes, RouterModule } from '@angular/router';
import { FuseSharedModule } from '@fuse/shared.module';
import { PipesModule } from '@app/core/pipes/pipes.module';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatMenuModule } from '@angular/material/menu';
import { CdkTableModule } from '@angular/cdk/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';

const routes: Routes = [
  {
    path: 'work-orders/daily-report/:loggedEmployeeGuid/:operationsManagerId/:operationsManagerGuid/:dateFrom/:dateTo',
    component: PublicDailyReportOperationsManagerComponent,
    resolve: {
      resolver: PublicDailyReportOperationsManagerResolver
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatSelectModule,
    MatTableModule,
    CdkTableModule,
    MatSortModule,
    MatMenuModule,
    MatPaginatorModule,

    // Add pipes
    PipesModule
  ],
  declarations: [
    PublicDailyReportOperationsManagerComponent
  ],

})
export class PublicDailyReportOperationsManagerModule { }
