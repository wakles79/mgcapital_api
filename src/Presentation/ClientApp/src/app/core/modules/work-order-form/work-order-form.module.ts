import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';

import { FuseSharedModule } from '../../../../@fuse/shared.module';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatRadioModule } from '@angular/material/radio';

import { WorkOrderSharedFormComponent } from './work-order-form/work-order-form.component';
import { WorkOrderFormTemplateComponent } from './work-order-form-template/work-order-form-template.component';
import { WorkOrderTaskFormComponent } from './work-order-task-form/work-order-task-form.component';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { PipesModule } from '@app/core/pipes/pipes.module';
import { TimeAgoPipe } from 'time-ago-pipe';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { WoTaskBillingFormComponent } from './wo-task-billing-form/wo-task-billing-form.component';
import { WorkOrderDialogModule } from '../work-order-dialog/work-order-dialog.module';
import { MatCardModule } from '@angular/material/card';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FuseDirectivesModule } from '@fuse/directives/directives';
import { FlexLayoutModule } from '@angular/flex-layout';
import { LightboxModule } from 'ngx-lightbox';
import { FeatureFlagModule } from '@app/core/modules/feature-flag.module';

@NgModule({
  imports: [
    FuseSharedModule,
    MatCheckboxModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatSnackBarModule,
    MatDialogModule,
    MatToolbarModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    NgxMatSelectSearchModule,
    MatSlideToggleModule,
    MatTooltipModule,
    MatProgressBarModule,
    MatRadioModule,
    MatCardModule,
    ReactiveFormsModule,
    FormsModule,
    MatDatepickerModule,
    MatNativeDateModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    LightboxModule,
    FeatureFlagModule,
    // App pipes
    PipesModule,
    // App modules import
    WorkOrderDialogModule
  ],
  declarations: [
    WorkOrderSharedFormComponent,
    WorkOrderFormTemplateComponent,
    WorkOrderTaskFormComponent,
    WoTaskBillingFormComponent,
  ],
  exports: [
    WorkOrderSharedFormComponent,
    WorkOrderFormTemplateComponent,
    WorkOrderTaskFormComponent,
    WoTaskBillingFormComponent,
  ],
  entryComponents: [

  ],
  providers: [
    DatePipe,
  ]
})
export class WorkOrderFormModule { }
