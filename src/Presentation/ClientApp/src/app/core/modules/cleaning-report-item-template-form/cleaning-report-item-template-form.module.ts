import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CleaningReportItemTemplateFormComponent } from './cleaning-report-item-template-form.component';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatToolbarModule } from '@angular/material/toolbar';
import { FuseSharedModule } from '../../../../@fuse/shared.module';
import { MatSelectModule } from '@angular/material/select';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatInputModule } from '@angular/material/input';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';



@NgModule({

  imports: [
    FuseSharedModule,
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
    MatExpansionModule,
    MatListModule,
    NgxMatSelectSearchModule,
    MatSlideToggleModule,
    MatTooltipModule,
    NgxMatSelectSearchModule
    // MatCheckboxModule,

  ],
  declarations: [
    CleaningReportItemTemplateFormComponent
  ],
  exports: [
    CleaningReportItemTemplateFormComponent
  ],
  entryComponents: [

  ],
  providers: [

  ]
})
export class CleaningReportItemTemplateFormModule { }
