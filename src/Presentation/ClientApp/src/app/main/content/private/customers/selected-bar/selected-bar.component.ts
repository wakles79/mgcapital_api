import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MultipleDeleteConfirmDialogComponent } from '@app/core/modules/delete-confirm-dialog/multiple-delete-confirm-dialog/multiple-delete-confirm-dialog.component';
import { CustomersService } from '../customers.service';

@Component({
  selector: 'app-selected-bar-customers',
  templateUrl: './selected-bar.component.html',
  styleUrls: ['./selected-bar.component.scss']
})
export class CustomersSelectedBarComponent implements OnInit {

  selectedCustomers: string[];
  hasSelectedCustomers: boolean;
  isIndeterminate: boolean;
  confirmDialogRef: MatDialogRef<MultipleDeleteConfirmDialogComponent>;

  constructor(
    private customersService: CustomersService,
    public dialog: MatDialog
  ) {
    this.customersService.onSelectedCustomersChanged
      .subscribe(selectedCustomers => {
        this.selectedCustomers = selectedCustomers;
        setTimeout(() => {
          this.hasSelectedCustomers = selectedCustomers.length > 0;
          this.isIndeterminate = (selectedCustomers.length !== this.customersService.customers.length && selectedCustomers.length > 0);
        }, 0);
      });

  }

  ngOnInit(): void {
  }

  selectAll(): void {
    this.customersService.selectCustomers();
  }

  deselectAll(): void {
    this.customersService.deselectCustomers();
  }

  deleteSelectedCustomers(): void {
    const toDelete = [];
    this.selectedCustomers.forEach(guid => {
      toDelete.push(this.customersService.deleteByGuid(guid));
    });

    this.confirmDialogRef = this.dialog.open(MultipleDeleteConfirmDialogComponent, {
      panelClass: 'multiple-delete-dialog',
      disableClose: false,
      data: {
        title: 'Delete multiple users',
        requests: toDelete
      }
    });

    this.confirmDialogRef.afterClosed().subscribe((result) => {
      this.customersService.getCustomers();
    });
  }
}
