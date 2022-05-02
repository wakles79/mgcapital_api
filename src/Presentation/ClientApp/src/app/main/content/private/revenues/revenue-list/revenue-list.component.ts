import { DataSource } from '@angular/cdk/table';
import { Component, OnInit, ViewEncapsulation, AfterViewInit, ViewChild, TemplateRef, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { RevenueBaseModel } from '@app/core/models/revenue/revenue-base.model';
import { RevenueGridModel } from '@app/core/models/revenue/revenue-grid.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { merge, Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { RevenueFormComponent } from '../revenue-form/revenue-form.component';
import { RevenuesService } from '../revenues.service';

@Component({
  selector: 'app-revenue-list',
  templateUrl: './revenue-list.component.html',
  styleUrls: ['./revenue-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class RevenueListComponent implements OnInit, AfterViewInit {

  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @Input() readOnly: boolean;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;
  revenueFormDialog: MatDialogRef<RevenueFormComponent>;

  get revenueCount(): number { return this.revenueService.elementsCount; }

  columnsToDisplay = ['checkbox', 'transactionNumber', 'contract', 'building', 'customer', 'description', 'reference', 'date', 'subtotal', 'tax', 'total', 'buttons'];

  loading$ = this.revenueService.loadingSubject.asObservable();
  searchInput: FormControl;
  dataSource: RevenueDataSource | null;

  onRevenuesChangedSubscription: Subscription;
  onSelectedRevenuesChangedSubscription: Subscription;
  onRevenueDataChangedSubscription: Subscription;

  selectedRevenues: any[];
  checkboxes: {};

  revenues: RevenueGridModel[] = [];
  revenue: RevenueBaseModel;

  constructor(
    private revenueService: RevenuesService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {
    this.searchInput = new FormControl(this.revenueService.searchText);

    this.onRevenuesChangedSubscription = this.revenueService.allElementsChanged
      .subscribe(revenues => {
        this.revenues = revenues;

        this.checkboxes = {};

        revenues.map(revenue => {
          this.checkboxes[revenue.id] = false;
        });
      });

    this.onSelectedRevenuesChangedSubscription =
      this.revenueService.selectedElementsChanged.subscribe(selectedRevenues => {
        for (const id in this.checkboxes) {
          if (!this.checkboxes.hasOwnProperty(id)) {
            continue;
          }
          this.checkboxes[id] = selectedRevenues.includes(id);
        }
        this.selectedRevenues = selectedRevenues;
      });

    this.onRevenueDataChangedSubscription = this.revenueService.elementChanged
      .subscribe(revenue => {
        this.revenue = revenue;
      });
  }

  ngOnInit(): void {

    this.searchInput.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.revenueService.searchTextChanged.next(searchText);
      });
    this.dataSource = new RevenueDataSource(this.revenueService);
  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.revenueService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize
        ))
      )
      .subscribe();
  }

  editRevenue(revenue): void {
    this.revenueFormDialog = this.dialog.open(RevenueFormComponent, {
      panelClass: 'revenue-form-dialog',
      data: {
        action: 'edit',
        revenue: revenue
      }
    });


    this.revenueFormDialog.afterClosed()
      .subscribe((revenueForm: FormGroup) => {
        if (!revenueForm) {
          return;
        }

        const RevenueToUpdate = new RevenueBaseModel(revenueForm.getRawValue());
        this.revenueService.loadingSubject.next(true);
        this.revenueService.updateElement(RevenueToUpdate)
          .then(
            () => {
              this.revenueService.loadingSubject.next(false);
              this.snackBar.open('Inspection updated successfully!!!', 'close', { duration: 1000 });
            },
            () => {
              this.revenueService.loadingSubject.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            })
          .catch(() => {
            this.revenueService.loadingSubject.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });

      });
  }

  deleteRevenue(revenue): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });
    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.revenueService.delete(revenue);
      }
      this.confirmDialogRef = null;
    });
  }

  onSelectedChange(revenueID): void {
    this.revenueService.toggleSelectedElement(revenueID);
  }

}

export class RevenueDataSource extends DataSource<RevenueGridModel>{

  constructor(private revenueService: RevenuesService) {
    super();
  }

  connect(): Observable<any[]> {
    return this.revenueService.allElementsChanged;
  }

  disconnect(): void {

  }
}
