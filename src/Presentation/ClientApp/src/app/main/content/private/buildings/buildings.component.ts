import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '@app/core/services/auth.service';
import { fuseAnimations } from '@fuse/animations';
import { BuildingsService } from './buildings.service';
import { BuildingFormComponent } from '@app/core/modules/building-form/building-form/building-form.component';
import * as moment from 'moment';
import { FormGroup } from '@angular/forms';
import { FuseSidebarService } from '../../../../../@fuse/components/sidebar/sidebar.service';

@Component({
  selector: 'app-buildings',
  templateUrl: './buildings.component.html',
  styleUrls: ['./buildings.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None

})
export class BuildingsComponent implements OnInit {

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  dialogRef: any;
  roleLevelLoggedUser: any;

  constructor(
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private buildingService: BuildingsService,
    private authService: AuthService,
    private _fuseSidebarService: FuseSidebarService) { }

  ngOnInit(): void {
    this.roleLevelLoggedUser = this.authService.currentUser.roleLevel;
  }

  newBuilding(): void {
    this.dialogRef = this.dialog.open(BuildingFormComponent, {
      panelClass: 'building-form-dialog',
      data: {
        action: 'new'
      }
    });
    this.dialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        // Create building
        this.buildingService.createElement(response.getRawValue())
          .then(
            () => this.snackBar.open('Building created successfully!!!', 'close', { duration: 1000 }),
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });
  }

  exportBuildingReportPDF(): void {
    this.buildingService.loadingSubject.next(true);
    this.buildingService.getDocumentUrl().subscribe((response: string) => {
      this.buildingService.loadingSubject.next(false);
      window.open(response, '_blank');
    },
      (error) => {
        this.buildingService.loadingSubject.next(false);
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

  exportCsvReport(): void {
    this.buildingService.exportCsv()
      .then(
        (csvFile) => {
          this.downloadFile(csvFile);
        },
        (error) => {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      );
  }

  downloadFile(data: any): void {
    const csvData = data;
    const a = document.createElement('a');
    a.setAttribute('style', 'display:none;');
    document.body.appendChild(a);
    const blob = new Blob([csvData.body], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    a.href = url;
    a.download = 'BuildingReport_' + this.dateReportString + '.csv';
    a.click();
  }

  get dateReportString(): string {
    return moment(new Date()).format('YYYY-MM-DD');
  }

/**
 * Toggle sidebar
 *
 * @param name
 */
  toggleSidebar(name): void {
    this._fuseSidebarService.getSidebar(name).toggleOpen();
  }
}
