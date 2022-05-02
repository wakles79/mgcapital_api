import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InspectionsFormComponent } from './inspections-form/inspections-form.component';
import { FuseSharedModule } from '@fuse/shared.module';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { OwlNativeDateTimeModule, OwlDateTimeModule } from 'ng-pick-datetime';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatTooltipModule } from '@angular/material/tooltip';



@NgModule({
  imports: [

    FuseSharedModule,
    MatIconModule,
    // MatProgressSpinnerModule,
    MatButtonModule,
    // MatSnackBarModule,
    MatDialogModule,
    MatToolbarModule,
    // MatMenuModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    // MatExpansionModule,
    // MatListModule,
    MatTooltipModule,
    MatCheckboxModule,
    NgxMatSelectSearchModule,
    OwlNativeDateTimeModule,
    OwlDateTimeModule,
    MatDatepickerModule,
    MatNativeDateModule,

  ],
  declarations: [
    InspectionsFormComponent
  ],
  exports: [
    InspectionsFormComponent
  ],
  entryComponents: [

  ],
  providers: [

  ]
})
export class InspectionsFormModule { }
