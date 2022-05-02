import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { WoCalendarComponent } from './wo-calendar.component';
import { RouterModule, Routes } from '@angular/router';
import { WoCalendarResolver } from './wo-calendar-resolver';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { LightboxModule } from 'ngx-lightbox';
import { CalendarItemFormComponent } from './calendar-item-form/calendar-item-form.component';
import { CalendarSequenceSummaryResultModule } from '@app/core/modules/calendar-sequence-summary-result/calendar-sequence-summary-result.module';
import { WorkOrderDialogModule } from '@app/core/modules/work-order-dialog/work-order-dialog.module';
import { WorkOrderFormModule } from '@app/core/modules/work-order-form/work-order-form.module';
import { PipesModule } from '@app/core/pipes/pipes.module';
import { FuseSharedModule } from '@fuse/shared.module';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { WorkOrderTaskFormComponent } from '@app/core/modules/work-order-form/work-order-task-form/work-order-task-form.component';
import { WoTaskFormConfirmCloseComponent } from '@app/core/modules/work-order-dialog/wo-task-form-confirm-close/wo-task-form-confirm-close.component';
import { WorkOrderSharedFormComponent } from '@app/core/modules/work-order-form/work-order-form/work-order-form.component';
import { WoActivityLogDialogComponent } from '@app/core/modules/work-order-dialog/wo-activity-log-dialog/wo-activity-log-dialog.component';
import { WorkOrderSequencesDialogComponent } from '@app/core/modules/work-order-dialog/work-order-sequences-dialog/work-order-sequences-dialog.component';
import { WoTaskBillingFormComponent } from '@app/core/modules/work-order-form/wo-task-billing-form/wo-task-billing-form.component';
import { CalendarSequenceSummaryResultComponent } from '@app/core/modules/calendar-sequence-summary-result/calendar-sequence-summary-result/calendar-sequence-summary-result.component';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { WorkOrderFormTemplateComponent } from '@app/core/modules/work-order-form/work-order-form-template/work-order-form-template.component';
import { MainComponent } from './sidenavs/main/main.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';

// Material
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule, MatNativeDateModule } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatSortModule } from '@angular/material/sort';

const routes: Routes = [
  {
    path: '**',
    component: WoCalendarComponent,
    resolve: {
      resover: WoCalendarResolver
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory,
    }),
    LightboxModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSidenavModule,
    MatCardModule,
    MatChipsModule,
    MatTooltipModule,
    MatMenuModule,
    MatTableModule,
    CdkTableModule,
    MatPaginatorModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatSelectModule,
    MatOptionModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatNativeDateModule,
    NgxMatSelectSearchModule,
    MatInputModule,
    MatSortModule,

    // Add pipes
    PipesModule,

    // Add shared modules
    CalendarSequenceSummaryResultModule,
    WorkOrderDialogModule,
    WorkOrderFormModule,
  ],
  declarations: [
    WoCalendarComponent,
    CalendarItemFormComponent,
    MainComponent,
  ],
  providers: [
    DatePipe,
    FromEpochPipe,
  ],
  entryComponents: [
    CalendarItemFormComponent,
    WorkOrderFormTemplateComponent,
    WorkOrderSharedFormComponent,
    FuseConfirmDialogComponent,
    WoActivityLogDialogComponent,
    WorkOrderSequencesDialogComponent,
    WoTaskBillingFormComponent,
    CalendarSequenceSummaryResultComponent,
    WorkOrderTaskFormComponent,
    WoTaskFormConfirmCloseComponent
  ]
})
export class WoCalendarModule { }
