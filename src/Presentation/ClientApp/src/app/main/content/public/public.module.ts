import { NgModule } from '@angular/core';

import { LoginModule } from './authentication/login/login.module';
import { ForgotPasswordModule } from './authentication/forgot-password/forgot-password.module';
import { ResetPasswordModule } from './authentication/reset-password/reset-password.module';
import { LockModule } from './authentication/lock/lock.module';
import { MailConfirmModule } from './authentication/mail-confirm/mail-confirm.module';
import { CustomerMailConfirmModule } from './authentication/customer-mail-confirm/customer-mail-confirm.module';
import { Error403Module } from './errors/403/error-403.module';
import { Error404Module } from './errors/404/error-404.module';
import { Error500Module } from './errors/500/error-500.module';
import { MaintenanceModule } from './maintenance/maintenence.module';
import { RouterModule } from '@angular/router';
import { SelectCompanyModule } from '@app/main/content/public/authentication/select-company/select-company.module';
import { PublicWorkOrderModule } from '@app/main/content/public/public-work-order/public-work-order.module';
import { PublicDailyReportOperationsManagerModule } from '@app/main/content/public/public-daily-report-operations-manager/public-daily-report-operations-manager.module';
import { PublicCleaningReportModule } from '@app/main/content/public/public-cleaning-report/public-cleaning-report.module';
import { PublicContractReportModule } from './public-contract-report/public-contract-report.module';
import { PublicProposalDetailModule } from './public-proposal-detail/public-proposal-detail.module';
import { PublicInspectionDetailModule } from './public-inspection-detail/public-inspection-detail.module';
import { PublicWorkOrderRequesterModule } from './public-work-order-requester/public-work-order-requester.module';

const routes = [
  {
    path: '',
    redirectTo: '/app/home'
  }
];

@NgModule({
  imports: [

    RouterModule.forChild(routes),

    // Auth
    LoginModule,
    ForgotPasswordModule,
    ResetPasswordModule,
    LockModule,
    MailConfirmModule,
    CustomerMailConfirmModule,
    SelectCompanyModule,


    // Errors
    Error403Module,
    Error404Module,
    Error500Module,


    MaintenanceModule,

    PublicWorkOrderModule,
    PublicDailyReportOperationsManagerModule,
    PublicWorkOrderRequesterModule,
    PublicCleaningReportModule,
    PublicContractReportModule,
    PublicProposalDetailModule,
    PublicInspectionDetailModule
  ],
  declarations: []
})
export class FusePublicModule {

}
