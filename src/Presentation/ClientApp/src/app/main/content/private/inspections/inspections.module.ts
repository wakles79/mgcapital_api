import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { InspectionsResolver } from './inspections-resolver';
import { InspectionsComponent } from './inspections.component';
import { FuseSharedModule } from '../../../../../@fuse/shared.module';
import { FuseSidebarModule } from '../../../../../@fuse/components/sidebar/sidebar.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { InspectionsFormModule } from '@app/core/modules/inspections-form/inspections-form.module';
import { MainComponent } from './sidenavs/main/main.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule, MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';
import { InspectionListComponent } from './inspection-list/inspection-list.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatListModule } from '@angular/material/list';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { CdkTableModule } from '@angular/cdk/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { PipesModule } from '../../../../core/pipes/pipes.module';
import { MatSortModule } from '@angular/material/sort';
import { InspectionsFormComponent } from '@app/core/modules/inspections-form/inspections-form/inspections-form.component';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { InspectionDetailComponent } from './inspection-detail/inspection-detail.component';
import { InspectionDetailResolver } from './inspection-detail/inspection-detail-resolver';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatExpansionModule } from '@angular/material/expansion';
import { TicketFormModule } from '../../../../core/modules/ticket-form/ticket-form.module';
import { ISendEmailConfirmDialogComponent } from './i-send-email-confirm-dialog/i-send-email-confirm-dialog.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { InspectionItemComponent } from './inspection-detail/inspection-item/inspection-item.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { ICloseInspectionDialogComponent } from './i-close-inspection-dialog/i-close-inspection-dialog.component';
import { LightboxModule } from 'ngx-lightbox';
import { TicketFormDialogComponent } from '@app/core/modules/ticket-form/ticket-form/ticket-form.component';
import { ShareUrlDialogComponent } from '@app/core/modules/share-url-dialog/share-url-dialog/share-url-dialog.component';

const routes: Routes = [
  {
    path: '',
    component: InspectionsComponent,
    resolve: {
      resolver: InspectionsResolver
    }
  }, {
    path: 'detail/:id',
    component: InspectionDetailComponent,
    resolve: {
      resolver: InspectionDetailResolver
    }
  }
];

@NgModule({
  imports: [

    FuseSharedModule,
    FuseSidebarModule,
    RouterModule.forChild(routes),
    LightboxModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    // MatSnackBarModule,
    MatDialogModule,
    MatToolbarModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    MatExpansionModule,
    MatListModule,
    MatTooltipModule,
    MatSelectModule,
    MatOptionModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatRippleModule,
    MatTableModule,
    CdkTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatSlideToggleModule,
    MatCardModule,
    MatProgressBarModule,
    MatCheckboxModule,
    NgxMatSelectSearchModule,

    // Add Modules
    InspectionsFormModule,
    TicketFormModule,

    // Add Pipes
    PipesModule

  ],
  declarations: [
    InspectionsComponent,
    MainComponent,
    InspectionListComponent,
    InspectionDetailComponent,
    ISendEmailConfirmDialogComponent,
    InspectionItemComponent,
    ICloseInspectionDialogComponent
  ],
  providers: [
    DatePipe,
    FromEpochPipe
  ],
  entryComponents: [
    InspectionsFormComponent,
    InspectionItemComponent,
    ICloseInspectionDialogComponent,
    TicketFormDialogComponent,
    ShareUrlDialogComponent,
    ISendEmailConfirmDialogComponent
  ]
})
export class InspectionsModule { }
