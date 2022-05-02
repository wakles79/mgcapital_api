import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProposalsComponent } from './proposals.component';
import { Routes, RouterModule } from '@angular/router';
import { ProposalsResolver } from './proposals-resolver';
import { ProposalListComponent } from './proposal-list/proposal-list.component';
import { ProposalFormComponent } from './proposal-form/proposal-form.component';
import { FuseSharedModule } from '@fuse/shared.module';
import { ProposalDetailComponent } from './proposal-detail/proposal-detail.component';
import { FuseSidebarModule } from '../../../../../@fuse/components/sidebar/sidebar.module';
import { ProposalServiceFormComponent } from './proposal-detail/proposal-service-form/proposal-service-form.component';
import { ProposalDetailResolver } from './proposal-detail/proposal-detail-resolver';
import { PSendEmailConfirmDialogComponent } from './p-send-email-confirm-dialog/p-send-email-confirm-dialog.component';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { ShareUrlDialogComponent } from '@app/core/modules/share-url-dialog/share-url-dialog/share-url-dialog.component';
import { OfficeTypeFormComponent } from '@app/core/modules/office-type-form/office-type-form.component';
import { ShareUrlDialogModule } from '@app/core/modules/share-url-dialog/share-url-dialog.module';
import { OfficeTypeFormModule } from '@app/core/modules/office-type-form/office-type-form.module';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSelectModule } from '@angular/material/select';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';


const routes: Routes = [
  {
    path: 'proposals',
    component: ProposalsComponent,
    resolve: {
      resolver: ProposalsResolver
    }
  }, {
    path: 'proposal-detail/:id',
    component: ProposalDetailComponent,
    resolve: {
      resolver: ProposalDetailResolver
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
    MatTableModule,
    CdkTableModule,
    MatMenuModule,
    MatPaginatorModule,
    MatSortModule,
    MatToolbarModule,
    MatTooltipModule,
    MatSelectModule,
    NgxMatSelectSearchModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatRippleModule,

    // App modules import
    ShareUrlDialogModule,
    OfficeTypeFormModule,
  ],
  declarations: [
    ProposalsComponent,
    ProposalListComponent,
    ProposalFormComponent,
    ProposalDetailComponent,
    ProposalServiceFormComponent,
    PSendEmailConfirmDialogComponent
  ],
  providers: [

  ],
  entryComponents: [
    ProposalFormComponent,
    FuseConfirmDialogComponent,
    ShareUrlDialogComponent,
    ProposalServiceFormComponent,
    OfficeTypeFormComponent,
    PSendEmailConfirmDialogComponent
  ]

})
export class ProposalsModule { }
