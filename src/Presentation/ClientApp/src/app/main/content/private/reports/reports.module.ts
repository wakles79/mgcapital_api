import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CleaningReportComponent } from './cleaning-report/cleaning-report.component';
import { MainComponent } from './sidenavs/cleaning-report/main.component';
import { RouterModule, Routes } from '@angular/router';
import { FuseSharedModule } from '../../../../../@fuse/shared.module';
import { LightboxModule } from 'ngx-lightbox';
import { FuseSidebarModule } from '../../../../../@fuse/components/sidebar/sidebar.module';
import { CleaningReportResolver } from './cleaning-report/cleaning-report.resolver';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CleaningReportFormComponent } from './cleaning-report/cleaning-report-form/cleaning-report-form.component';
import { CleaningReportListComponent } from './cleaning-report/cleaning-report-list/cleaning-report-list.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { PipesModule } from '../../../../core/pipes/pipes.module';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { MatSortModule } from '@angular/material/sort';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInput, MatInputModule } from '@angular/material/input';
import { MatDividerModule } from '@angular/material/divider';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatToolbarModule } from '@angular/material/toolbar';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { CleaningReportDetailsComponent } from './cleaning-report/cleaning-report-details/cleaning-report-details.component';
import { CleaningReportDetailsResolver } from './cleaning-report/cleaning-report-details/cleaning-report-details.resolver';
import { MatExpansionModule } from '@angular/material/expansion';
import { CleaningReportItemFormComponent } from './cleaning-report/cleaning-report-item-form/cleaning-report-item-form.component';
import { CrSendEmailConfirmDialogComponent } from './cleaning-report/cr-send-email-confirm-dialog/cr-send-email-confirm-dialog.component';
import { CleaningReportItemTemplateFormModule } from '@app/core/modules/cleaning-report-item-template-form/cleaning-report-item-template-form.module';
import { CleaningReportItemTemplateFormComponent } from '@app/core/modules/cleaning-report-item-template-form/cleaning-report-item-template-form.component';
import { ShareUrlDialogComponent } from '@app/core/modules/share-url-dialog/share-url-dialog/share-url-dialog.component';
import { ShareUrlDialogModule } from '../../../../core/modules/share-url-dialog/share-url-dialog.module';
import { MatTooltipModule } from '@angular/material/tooltip';


const routes: Routes = [
  {
    path: 'cleaning-report',
    component: CleaningReportComponent,
    resolve: {
      services: CleaningReportResolver
    }
  },
  {
    path: 'cleaning-report/:id',
    component: CleaningReportDetailsComponent,
    resolve: {
      services: CleaningReportDetailsResolver
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    FuseSidebarModule,
    RouterModule.forChild(routes),
    LightboxModule,

    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatTableModule,
    CdkTableModule,
    MatMenuModule,
    MatPaginatorModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatDialogModule,
    MatInputModule,
    MatListModule,
    MatSortModule,
    MatDividerModule,
    MatSlideToggleModule,
    NgxMatSelectSearchModule,
    MatToolbarModule,
    MatProgressSpinnerModule,
    MatSidenavModule,
    MatExpansionModule,
    MatTooltipModule,

    // App pipes
    PipesModule,

    CleaningReportItemTemplateFormModule,
    ShareUrlDialogModule
  ],
  declarations: [
    CleaningReportComponent,
    MainComponent,
    CleaningReportFormComponent,
    CleaningReportListComponent,
    CleaningReportDetailsComponent,
    CleaningReportItemFormComponent,
    CrSendEmailConfirmDialogComponent
  ],
  providers: [

  ],
  entryComponents: [
    CleaningReportFormComponent,
    CleaningReportItemFormComponent,
    CleaningReportItemTemplateFormComponent,
    ShareUrlDialogComponent,
    CrSendEmailConfirmDialogComponent
  ]
})
export class ReportsModule { }
