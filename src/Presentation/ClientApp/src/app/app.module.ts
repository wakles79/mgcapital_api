import { NgModule, ErrorHandler, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

import { FuseModule } from '@fuse/fuse.module';
import { FuseSharedModule } from '@fuse/shared.module';
import { FuseProgressBarModule, FuseSidebarModule, FuseThemeOptionsModule } from '@fuse/components';

import { fuseConfig } from 'app/fuse-config';

import { AppComponent } from 'app/app.component';
import { LayoutModule } from 'app/layout/layout.module';
import { AuthGuard } from '@app/core/services/auth-guard.service';
import { CompanyInterceptor } from '@app/core/interceptors/company.interceptor';
import { ErrorHandlerInterceptor } from '@app/core/interceptors/error-handler.interceptor';
import { AppErrorHandler } from '@app/core/error-handling/app-error-handler';
import { ConfigLoaderService } from '@app/core/services/config-loader.service';
import { ContactFormModule } from './core/modules/contact-form/contact-form.module';
import { ConfirmDialogModule } from './core/modules/confirm-dialog/confirm-dialog.module';
import { DeleteConfirmDialogModule } from './core/modules/delete-confirm-dialog/delete-confirm-dialog.module';
import { VerifyFreshdeskModule } from './core/modules/verify-freshdesk/verify-freshdesk.module';
import { AgmCoreModule } from '@agm/core';
import { BuildingFormModule } from './core/modules/building-form/building-form.module';
import { CustomerFormModule } from './core/modules/customer-form/customer-form.module';
import { WorkOrderDialogModule } from './core/modules/work-order-dialog/work-order-dialog.module';
import { WorkOrderFormModule } from './core/modules/work-order-form/work-order-form.module';
import { FusePipesModule } from '@fuse/pipes/pipes.module';
import { PipesModule } from './core/pipes/pipes.module';
import { MessageDialogModule } from './core/modules/message-dialog/message-dialog.module';
import { ElementSelectorFilterModule } from './core/modules/element-selector-filter/element-selector-filter.module';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { ShareUrlDialogModule } from './core/modules/share-url-dialog/share-url-dialog.module';
import { CleaningReportItemTemplateFormModule } from './core/modules/cleaning-report-item-template-form/cleaning-report-item-template-form.module';
import { InspectionsFormModule } from './core/modules/inspections-form/inspections-form.module';
import { TicketFormModule } from './core/modules/ticket-form/ticket-form.module';
import { CalendarSequenceSummaryResultModule } from './core/modules/calendar-sequence-summary-result/calendar-sequence-summary-result.module';
import { ExpensesFormModule } from './core/modules/expenses-form/expenses-form.module';
import { EditorModule } from '@progress/kendo-angular-editor';
import { ImportRevenueExpenseCsvFormModule } from './core/modules/import-revenue-expense-csv-form/import-revenue-expense-csv-form.module';
import { OfficeTypeFormModule } from './core/modules/office-type-form/office-type-form.module';
import { ResultImportCsvModule } from './core/modules/result-import-csv/result-import-csv.module';
import { BudgetActivityLogNotesDialogModule } from './core/modules/budget-activity-log-notes-dialog/budget-activity-log-notes-dialog.module';

const appRoutes: Routes = [
  {
    path: 'app',
    loadChildren: () => import('./main/content/private/private.module').then(m => m.FusePrivateModule),
    canActivate: [AuthGuard]
  },
  {
    path: '',
    loadChildren: () => import('./main/content/public/public.module').then(m => m.FusePublicModule),
  },
  {
    path: '**',
    redirectTo: 'errors/error-404'
  }
];

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    RouterModule.forRoot(appRoutes),

    TranslateModule.forRoot(),

    // Fuse modules
    FuseModule.forRoot(fuseConfig),
    FuseProgressBarModule,
    FuseSharedModule,
    FuseSidebarModule,
    FuseThemeOptionsModule,

    // App modules
    LayoutModule,
    EditorModule,

    // App shared modules
    ContactFormModule,
    ConfirmDialogModule,
    DeleteConfirmDialogModule,
    VerifyFreshdeskModule,
    BuildingFormModule,
    CustomerFormModule,
    WorkOrderDialogModule,
    WorkOrderFormModule,
    MessageDialogModule,
    ElementSelectorFilterModule,
    ShareUrlDialogModule,
    CleaningReportItemTemplateFormModule,
    InspectionsFormModule,
    TicketFormModule,
    CalendarSequenceSummaryResultModule,
    ExpensesFormModule,
    ImportRevenueExpenseCsvFormModule,
    OfficeTypeFormModule,
    ResultImportCsvModule,
    BudgetActivityLogNotesDialogModule,

    // App pipes
    FusePipesModule,
    PipesModule,

    AgmCoreModule.forRoot({
      apiKey: 'AIzaSyC9XXEmb6jc3Z4OPtSNIQLZY6qxl9xYOVs',
      libraries: ['places']
    }),

    NgxMatSelectSearchModule

  ],
  bootstrap: [
    AppComponent
  ],
  providers: [
    // Company Interceptor
    {
      provide: HTTP_INTERCEPTORS,
      useClass: CompanyInterceptor,
      multi: true
    },

    // Error Handler Interceptor
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorHandlerInterceptor,
      multi: true
    },

    // Error Handler
    {
      provide: ErrorHandler,
      useClass: AppErrorHandler
    },

    // Config initializer
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializerFactory,
      deps: [ConfigLoaderService],
      multi: true
    }
  ]
})
export class AppModule {
}

export function appInitializerFactory(configsLoaderService: ConfigLoaderService): () => Promise<any> {
  return () => configsLoaderService.loadConfig();
}
