import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fuseAnimations } from '@fuse/animations';
import { CleaningReportService } from './cleaning-report.service';
import { CleaningReportFormComponent } from './cleaning-report-form/cleaning-report-form.component';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';

@Component({
  selector: 'app-cleaning-report',
  templateUrl: './cleaning-report.component.html',
  styleUrls: ['./cleaning-report.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None,
})
export class CleaningReportComponent implements OnInit {

  dialogRef: any;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private cleaningReportService: CleaningReportService,
    private _fuseSidebarService: FuseSidebarService
  ) { }

  ngOnInit(): void {
  }

  newCleaningReport(): void {
    this.dialogRef = this.dialog.open(CleaningReportFormComponent, {
      panelClass: 'cleaning-report-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.dialogRef.afterClosed().subscribe((response: FormGroup) => {
      if (!response) {
        return;
      }

      this.cleaningReportService.createElement(response.getRawValue())
        .then(
          () => this.snackBar.open('Cleaning Report created successfully!!!', 'close', { duration: 1000 }),
          () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 })
        ).catch(
          () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 })
        );
    });
  }

    /**
     * Toggle sidebar
     *
     * @param name
     */
    toggleSidebar(name): void
    {
        this._fuseSidebarService.getSidebar(name).toggleOpen();
    }

}
