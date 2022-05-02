import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { ExpensesService } from '../expenses.service';

@Component({
  selector: 'app-selected-bar',
  templateUrl: './selected-bar.component.html',
  styleUrls: ['./selected-bar.component.scss']
})
export class SelectedBarComponent implements OnInit {

  selectedExpenses: string[];
  hasSelectedExpenses: boolean;
  isIndeterminate: boolean;
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  constructor(
    private expenseService: ExpensesService,
    public dialog: MatDialog
  ) {
    this.expenseService.selectedElementsChanged
      .subscribe(selectedExpenses => {
        this.selectedExpenses = selectedExpenses;
        setTimeout(() => {
          this.hasSelectedExpenses = selectedExpenses.length > 0;
          this.isIndeterminate = (selectedExpenses.length !== this.expenseService.allElementsToList.length && selectedExpenses.length > 0);
        }, 0);
      });
  }

  ngOnInit(): void {
  }

  selectAll(): void {
    this.expenseService.getSelectedElements();
  }

  deselectAll(): void {
    this.expenseService.deselectElements();
  }

  deleteSelectedExpenses(): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete all selected expenses?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.expenseService.deleteSelectedElements();
      }
      this.confirmDialogRef = null;
    });
  }

}
