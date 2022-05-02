import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketFormDialogComponent } from './ticket-form/ticket-form.component';
import { FuseSharedModule } from '@fuse/shared.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';



@NgModule({

  imports: [

    FuseSharedModule,
    MatIconModule,
    MatButtonModule,
    MatSnackBarModule,
    MatDialogModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatInputModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    MatSelectModule,
    MatOptionModule,
    NgxMatSelectSearchModule


  ],
  declarations: [
    TicketFormDialogComponent

  ],
  exports: [
    TicketFormDialogComponent

  ],
  entryComponents: [

  ],
  providers: [
    FromEpochPipe
  ]
})
export class TicketFormModule { }
