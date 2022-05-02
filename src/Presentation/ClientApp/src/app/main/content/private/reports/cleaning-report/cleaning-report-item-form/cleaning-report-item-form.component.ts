import { Component, OnInit, ViewEncapsulation, OnDestroy, Input, ViewChild, AfterViewInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CleaningReportItemTemplateFormComponent } from '@app/core/modules/cleaning-report-item-template-form/cleaning-report-item-template-form.component';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-cleaning-report-item-form',
  templateUrl: './cleaning-report-item-form.component.html',
  styleUrls: ['./cleaning-report-item-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CleaningReportItemFormComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild(CleaningReportItemTemplateFormComponent) cleaningReportItemTemplate: CleaningReportItemTemplateFormComponent;
  dataCleaningReportItem: any;

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  constructor(
    public dialogRef: MatDialogRef<CleaningReportItemFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
  ) {
    this.dataCleaningReportItem = data;
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.cleaningReportItemTemplate.onCleaningReportItemTemplateSubmitted.subscribe((payload: any) => {
      this.dialogRef.close(payload);
    });
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

}
