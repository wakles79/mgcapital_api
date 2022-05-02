import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-calendar-sequence-summary-result',
  templateUrl: './calendar-sequence-summary-result.component.html',
  styleUrls: ['./calendar-sequence-summary-result.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CalendarSequenceSummaryResultComponent implements OnInit {

  summaries: { woNumber: string, success: boolean, date: Date }[] = [];

  constructor(
    public dialogRef: MatDialogRef<CalendarSequenceSummaryResultComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any
  ) {
    this.summaries = data.summary;
  }

  ngOnInit(): void {
  }

}
