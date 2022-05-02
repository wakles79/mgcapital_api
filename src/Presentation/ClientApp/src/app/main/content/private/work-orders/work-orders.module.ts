import { NgModule, Component } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { WorkOrdersComponent } from './work-orders.component';
import { FuseSharedModule } from '../../../../../@fuse/shared.module';
import { Resolve, RouterModule, Routes } from '@angular/router';
import { WorkOrderResolver } from './work-orders-resolver';

import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatMenuModule } from '@angular/material/menu';
import { WorkOrderDialogModule } from '@app/core/modules/work-order-dialog/work-order-dialog.module';
import { MainComponent } from './sidenavs/main/main.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { TimeAgoPipe } from 'time-ago-pipe';
import { WorkOrderFormModule } from '@app/core/modules/work-order-form/work-order-form.module';
import { WorkOrderListComponent } from './work-order-list/work-order-list.component';
import { PipesModule } from '@app/core/pipes/pipes.module';
import { FuseSidebarModule } from '../../../../../@fuse/components/sidebar/sidebar.module';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { AuthService } from '@app/core/services/auth.service';
import { CustomersService } from '../customers/customers.service';
import { ScheduleSettingsCategoryService } from '../schedule-settings-category/schedule-settings-category.service';
import { WoCalendarService } from '../wo-calendar/wo-calendar.service';
import { WorkOrderServicesService } from '../work-order-services/work-order-services.service';
import { WorkOrderFormComponent } from './work-order-form/work-order-form.component';
import { WorkOrderDetailComponent } from './work-order-detail/work-order-detail.component';
import { WoBillableReportComponent } from './wo-billable-report/wo-billable-report.component';
import { WoDailyReportOmDocumentComponent } from './wo-daily-report-om-document/wo-daily-report-om-document.component';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatRadioModule } from '@angular/material/radio';
import { WoTaskFormConfirmCloseComponent } from '../../../../core/modules/work-order-dialog/wo-task-form-confirm-close/wo-task-form-confirm-close.component';
import { WorkOrderTaskFormComponent } from '@app/core/modules/work-order-form/work-order-task-form/work-order-task-form.component';
import { WorkOrderSequencesDialogComponent } from '@app/core/modules/work-order-dialog/work-order-sequences-dialog/work-order-sequences-dialog.component';
import { WoActivityLogDialogComponent } from '@app/core/modules/work-order-dialog/wo-activity-log-dialog/wo-activity-log-dialog.component';
import { WorkOrderFormTemplateComponent } from '@app/core/modules/work-order-form/work-order-form-template/work-order-form-template.component';
import { MessageDialogComponent } from '@app/core/modules/message-dialog/message-dialog/message-dialog.component';
import { WoTaskBillingFormComponent } from '@app/core/modules/work-order-form/wo-task-billing-form/wo-task-billing-form.component';
import { LightboxModule } from 'ngx-lightbox';
import { ContactFormModule } from '@app/core/modules/contact-form/contact-form.module';
import { ContactFormComponent } from '@app/core/modules/contact-form/contact-form/contact-form.component';
import { WorkOrderDetailResolver } from './work-order-detail/work-order-detail-resolver';
import { DailyWoReportByOperationsManagerResolver } from './wo-daily-report-operations-manager-resolver';
import { WorkOrderBillableReportResolver } from './wo-billable-report/wo-billable-report-resolver';
import { SearchContactFormComponent } from '@app/core/modules/contact-form/search-contact-form/search-contact-form.component';
import { WoConfirmDialogComponent } from '../../../../core/modules/work-order-dialog/wo-confirm-dialog/wo-confirm-dialog.component';
import { MessageDialogModule } from '@app/core/modules/message-dialog/message-dialog.module';
import { WoDailyReportSidenavComponent } from './sidenavs/wo-daily-report/wo-daily-report.component';
import { WoBillableReportSidenavComponent } from './sidenavs/wo-billable-report/wo-billable-report.component';
import { ElementSelectorFilterModule } from '../../../../core/modules/element-selector-filter/element-selector-filter.module';
import { ReactiveFormsModule } from '@angular/forms';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ShareUrlDialogComponent } from '@app/core/modules/share-url-dialog/share-url-dialog/share-url-dialog.component';
import { WorkOrderSharedFormComponent } from '../../../../core/modules/work-order-form/work-order-form/work-order-form.component';
import { FeatureFlagModule } from '@app/core/modules/feature-flag.module';
import { ElementSelectorFilterComponent } from '@app/core/modules/element-selector-filter/element-selector-filter/element-selector-filter.component';

const routes: Routes = [
  {
    path: '',
    component: WorkOrdersComponent,
    resolve: {
      resolver: WorkOrderResolver
    }
  },
  {
    path: 'detail',
    component: WorkOrderDetailComponent,
    resolve: {
      workorder: WorkOrderDetailResolver
    }
  },
  {
    path: 'daily-report-operations-manager',
    component: WoDailyReportOmDocumentComponent,
    resolve: {
      report: DailyWoReportByOperationsManagerResolver
    },
  },
  {
    path: 'billable-report',
    component: WoBillableReportComponent,
    resolve: {
      report: WorkOrderBillableReportResolver
    },
  },
];

@NgModule({
  imports: [

    FuseSharedModule,
    FuseSidebarModule,
    RouterModule.forChild(routes),
    MatIconModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatSnackBarModule,
    MatDialogModule,
    MatToolbarModule,
    MatTableModule,
    CdkTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSidenavModule,
    MatCheckboxModule,
    MatCardModule,
    MatProgressBarModule,
    MatDividerModule,
    MatSlideToggleModule,
    MatRadioModule,
    MatRippleModule,

    MatExpansionModule,
    MatListModule,

    MatTooltipModule,

    MatDatepickerModule,
    MatNativeDateModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,

    FeatureFlagModule,

    NgxMatSelectSearchModule,
    LightboxModule,

    // App modules import
    WorkOrderFormModule,
    WorkOrderDialogModule,
    ContactFormModule,
    MessageDialogModule,
    ElementSelectorFilterModule,

    // App pipes
    PipesModule,

  ],
  declarations: [
    WorkOrdersComponent,
    MainComponent,
    WorkOrderListComponent,
    WorkOrderFormComponent,
    WorkOrderDetailComponent,
    WoBillableReportComponent,
    WoDailyReportOmDocumentComponent,
    WoBillableReportSidenavComponent,
    WoDailyReportSidenavComponent
  ],
  providers: [
    DatePipe,
    FromEpochPipe,
    ScheduleSettingsCategoryService,
    WorkOrderServicesService,
  ],
  entryComponents: [
    WorkOrderFormComponent,
    ContactFormComponent,
    WoTaskBillingFormComponent,
    SearchContactFormComponent,
    WoConfirmDialogComponent,
    ShareUrlDialogComponent,
    MessageDialogComponent,
    WorkOrderFormTemplateComponent,
    WoActivityLogDialogComponent,
    WorkOrderSharedFormComponent,
    WorkOrderSequencesDialogComponent,
    WorkOrderTaskFormComponent,
    WoTaskFormConfirmCloseComponent,
    ElementSelectorFilterComponent
  ]

})
export class WorkOrdersModule { }
