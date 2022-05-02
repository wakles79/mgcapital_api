import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarSequenceSummaryResultComponent } from './calendar-sequence-summary-result/calendar-sequence-summary-result.component';
import { FuseSharedModule } from '@fuse/shared.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';



@NgModule({
  imports: [
    FuseSharedModule,
    MatIconModule,
    MatButtonModule,
    MatToolbarModule,
    MatCardModule
  ],
  declarations: [
    CalendarSequenceSummaryResultComponent
  ],
  exports: [
    CalendarSequenceSummaryResultComponent
  ],
  entryComponents: [

  ],
  providers: [

  ]
})
export class CalendarSequenceSummaryResultModule { }
