import { NgModule } from '@angular/core';
import { InboxComponent } from './inbox.component';
import { InboxService } from './inbox.service';
import { TicketsService } from './tickets.service';
import { TicketsResolver } from './tickets.resolver';
import { FuseSharedModule } from '@fuse/shared.module';
import { RouterModule, Routes } from '@angular/router';
import { FullTicketDetailComponent } from './full-ticket-detail/full-ticket-detail.component';
import { FullTicketDetailResolver } from './full-ticket-detail/full-ticket-detail-resolver';
import { InboxMainComponent } from './sidenavs/inbox-main/inbox-main.component';
import { TicketListComponent } from './ticket-list/ticket-list.component';
import { TicketListItemComponent } from './ticket-list/ticket-list-item/ticket-list-item.component';
import { FuseSidebarModule } from '@fuse/components';
import { DatePipe } from '@angular/common';
import { PipesModule } from '@app/core/pipes/pipes.module';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { MergeTicketDialogComponent } from './full-ticket-detail/merge-ticket-dialog/merge-ticket-dialog.component';
import { TicketReplyComponent } from './full-ticket-detail/ticket-reply/ticket-reply.component';
import { TicketReplyFormComponent } from './full-ticket-detail/ticket-reply-form/ticket-reply-form.component';

// Material
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatMenuModule } from '@angular/material/menu';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRippleModule } from '@angular/material/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatCardModule } from '@angular/material/card';
import { MatDivider, MatDividerModule } from '@angular/material/divider';
import { TicketFormDialogComponent } from '@app/core/modules/ticket-form/ticket-form/ticket-form.component';
import { TicketFormModule } from '@app/core/modules/ticket-form/ticket-form.module';
import { WorkOrderDialogModule } from '@app/core/modules/work-order-dialog/work-order-dialog.module';
import { WorkOrderSequencesDialogComponent } from '@app/core/modules/work-order-dialog/work-order-sequences-dialog/work-order-sequences-dialog.component';
import { WoTaskFormConfirmCloseComponent } from '@app/core/modules/work-order-dialog/wo-task-form-confirm-close/wo-task-form-confirm-close.component';
import { EditorModule } from '@progress/kendo-angular-editor';
import { LightboxModule } from 'ngx-lightbox';
import { TicketDestinationComponent } from './ticket-destination/ticket-destination.component';
import { TicketDestinationWorkOrderComponent } from './ticket-destination-work-order/ticket-destination-work-order.component';
import { WorkOrderFormModule } from '@app/core/modules/work-order-form/work-order-form.module';
import { WorkOrderFormTemplateComponent } from '@app/core/modules/work-order-form/work-order-form-template/work-order-form-template.component';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { WorkOrderTaskFormComponent } from '@app/core/modules/work-order-form/work-order-task-form/work-order-task-form.component';
import { WoTaskBillingFormComponent } from '@app/core/modules/work-order-form/wo-task-billing-form/wo-task-billing-form.component';
import { WoConfirmDialogComponent } from '@app/core/modules/work-order-dialog/wo-confirm-dialog/wo-confirm-dialog.component';
import { WorkOrderSharedFormComponent } from '@app/core/modules/work-order-form/work-order-form/work-order-form.component';
import { MessageDialogComponent } from '@app/core/modules/message-dialog/message-dialog/message-dialog.component';
import { MessageDialogModule } from '@app/core/modules/message-dialog/message-dialog.module';
import { MatToolbarModule } from '@angular/material/toolbar';
import { TicketFdDetailComponent } from './full-ticket-detail/ticket-fd-detail/ticket-fd-detail.component';
import { TicketActivityDialogComponent } from './full-ticket-detail/ticket-activity-dialog/ticket-activity-dialog.component';
import { TicketFdConversationComponent } from './full-ticket-detail/ticket-fd-conversation/ticket-fd-conversation.component';

const routes: Routes = [
  {
    path: ':folderHandle',
    component: InboxComponent,
    resolve: {
      tickets: TicketsResolver
    }
  },
  {
    path: 'ticket-detail/:id',
    component: FullTicketDetailComponent,
    resolve: {
      ticket: FullTicketDetailResolver
    }
  },
  {
    path: '**',
    redirectTo: 'pending'
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
    FuseSidebarModule,
    PipesModule,
    EditorModule,
    LightboxModule,

    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatMenuModule,
    MatSelectModule,
    MatIconModule,
    MatTooltipModule,
    MatCheckboxModule,
    MatRippleModule,
    MatDialogModule,
    MatSnackBarModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    NgxMatSelectSearchModule,
    MatCardModule,
    MatDividerModule,
    MatToolbarModule,

    // App shared modules
    TicketFormModule,
    WorkOrderDialogModule,
    WorkOrderFormModule,
    MessageDialogModule
  ],
  declarations: [
    InboxComponent,
    FullTicketDetailComponent,
    InboxMainComponent,
    TicketListComponent,
    TicketListItemComponent,
    MergeTicketDialogComponent,
    TicketReplyComponent,
    TicketReplyFormComponent,
    TicketDestinationComponent,
    TicketDestinationWorkOrderComponent,
    TicketFdDetailComponent,
    TicketActivityDialogComponent,
    TicketFdConversationComponent,
  ],
  providers: [
    DatePipe,
    FromEpochPipe,
  ],
  entryComponents: [
    TicketFormDialogComponent,
    // InboxMainViewComponent,
    // InboxConverterTicketViewComponent,
    WorkOrderFormTemplateComponent,
    WoTaskBillingFormComponent,
    WoConfirmDialogComponent,
    // TicketDestinationCleaningReportItemComponent,
    TicketDestinationWorkOrderComponent,
    MessageDialogComponent,
    TicketReplyFormComponent,
    MergeTicketDialogComponent,
    WorkOrderTaskFormComponent,
    WorkOrderSequencesDialogComponent,
    WorkOrderSharedFormComponent,
    FuseConfirmDialogComponent,
    WoTaskFormConfirmCloseComponent,
  ]
})
export class InboxModule { }
