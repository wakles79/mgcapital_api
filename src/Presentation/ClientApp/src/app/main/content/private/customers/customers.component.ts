import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fuseAnimations } from '@fuse/animations';
import { Subscription } from 'rxjs';
import { CustomersService } from './customers.service';
import * as moment from 'moment';
import { CustomerFormComponent } from '@app/core/modules/customer-form/customer-form.component';

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html',
  styleUrls: ['./customers.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class CustomersComponent implements OnInit, OnDestroy {

  hasSelectedCustomers: boolean;
  searchInput: FormControl;
  dialogRef: any;
  onSelectedCustomersChangedSubscription: Subscription;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    private customersService: CustomersService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {
    this.searchInput = new FormControl(this.customersService.searchText);
  }

  newCustomer(): void {
    this.dialogRef = this.dialog.open(CustomerFormComponent, {
      panelClass: 'customer-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.dialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        this.customersService.createCustomer(response.getRawValue())
          .then(
            () => this.snackBar.open('Customer created successfully!!!', 'close', { duration: 1000 }),
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });

  }

  ngOnInit(): void {
    this.onSelectedCustomersChangedSubscription =
      this.customersService.onSelectedCustomersChanged
        .subscribe(selectedCustomers => {
          this.hasSelectedCustomers = selectedCustomers.length > 0;
        });
  }

  ngOnDestroy(): void {
    this.onSelectedCustomersChangedSubscription.unsubscribe();
  }

  exportCsvReport(): void {
    this.customersService.exportCsv()
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
    a.download = 'CustomerReport_' + this.dateReportString + '.csv';
    a.click();
  }

  get dateReportString(): string {
    return moment(new Date()).format('YYYY-MM-DD');
  }
}
