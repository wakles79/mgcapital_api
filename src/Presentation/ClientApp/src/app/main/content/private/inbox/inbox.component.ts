import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { TicketStatus } from '@app/core/models/ticket/ticket-base.model';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';
import { BehaviorSubject, interval, Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { TicketsService } from './tickets.service';

@Component({
  selector: 'app-inbox',
  templateUrl: './inbox.component.html',
  styleUrls: ['./inbox.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class InboxComponent implements OnInit, OnDestroy {

  private _unsubscribeAll: Subject<any>;

  searchInput: FormControl;

  areResolvedTickets = false;
  areDeleteTickets = false;

  hasSelectedTickets: boolean;
  isIndeterminate: boolean;
  onSelectedTicketsChanged: Subscription;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;
  loadingList$ = new BehaviorSubject<boolean>(false);

  public REF = {
    TicketStatus: TicketStatus
  };

  routeParams: any;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') === null ? false : true;
  }

  public sortDirection = 'DESC';
  public sortControl: FormControl;
  public sortOptions: { title: string, key: string }[] =
    [
      { title: 'Date Received', key: 'CreatedDate' },
      { title: 'Last Modified', key: 'UpdatedDate' }
    ];

  isLoading = false;
  loading$ = this._ticketService.loadingSubject.asObservable();
  refreshSubscription: Subscription;

  get selectedTicketsIds(): any[] {
    const selectedTicketsIds = [];
    this._ticketService.selectedTickets.forEach(ticket => {
      selectedTicketsIds.push(ticket.id);
    });
    return selectedTicketsIds;
  }

  constructor(
    private _ticketService: TicketsService,
    private _fuseSidebarService: FuseSidebarService,
    private dialog: MatDialog,
    public snackBar: MatSnackBar,
    private route: ActivatedRoute
  ) {
    this._unsubscribeAll = new Subject();

    this.searchInput = new FormControl('');
    this.sortControl = new FormControl('UpdatedDate');
  }

  ngOnInit(): void {

    this.searchInput.valueChanges
      .pipe(
        takeUntil(this._unsubscribeAll),
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(searchText => {
        this._ticketService.searchTextChanged.next(searchText);
      });

    this.onSelectedTicketsChanged =
      this._ticketService.onSelectedTicketsChanged
        .pipe(
          takeUntil(this._unsubscribeAll)
        )
        .subscribe(selectedTickets => {

          setTimeout(() => {
            this.hasSelectedTickets = selectedTickets.length > 0;
            this.isIndeterminate = (selectedTickets.length !== this._ticketService.allElementsToList.length && selectedTickets.length > 0);
          }, 0);
        });

    this.routeParams = this.route.params;
    if (this.routeParams.value.folderHandle === 'resolved') {
      this.areResolvedTickets = true;
    } else if (this.routeParams.value.folderHandle === 'delete') {
      this.areDeleteTickets = true;
    }

    this.sortControl.valueChanges
      .pipe(
        takeUntil(this._unsubscribeAll)
      )
      .subscribe(value => {
        this._ticketService.getElements('readall', '', value === 'none' ? '' : value, this.sortDirection);
      });

    this.loading$.subscribe((status) => {
      this.isLoading = status;
    });

    const timer = interval(30000);
    this.refreshSubscription = timer
      .pipe(
        takeUntil(this._unsubscribeAll)
      ).subscribe(() => {
        this.refresh();
      });

  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  /**
   * Toggle the sidebar
   *
   * @param name
   */
  toggleSidebar(name): void {
    this._fuseSidebarService.getSidebar(name).toggleOpen();
  }

  /**
   * Select all tickets
   */
  toggleSelectAll(): void {
    this._ticketService.toggleSelectAll();
  }

  /**
   *
   */
  selectTickets(filterParameter?, filterValue?): void {
    this._ticketService.selectTickets(filterParameter, filterValue);
  }

  /**
   *
   */
  deselectTickets(): void {
    this._ticketService.deselectTickets();
  }

  /**
   * Delete a Ticket range
   */
  deleteSelectedTickets(): void {

    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadingList$.next(true);
        this._ticketService.deleteRangeTickets({ id: this.selectedTicketsIds })
          .then(
            () => {
              this.loadingList$.next(false);
              this.snackBar.open('Tickets deleted successfully!!!', 'close', { duration: 1000 });
            },
            (error) => {
              this.loadingList$.next(false);
              this.snackBar.open(error, 'close', { duration: 1000 });
            }
          ).catch(
            (error) => {
              this.loadingList$.next(false);
              this.snackBar.open(error, 'close', { duration: 1000 });
            }
          );
      }
      this.confirmDialogRef = null;
    });
  }

  /**
   * Update the status of selected tickets
   */
  updateStatusSelectedTickets(ticketStatus: TicketStatus): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to update the status to ' + this.REF.TicketStatus[ticketStatus] + ' of selected tickets?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadingList$.next(true);
        this._ticketService.updateStatusRangeTickets({ id: this.selectedTicketsIds, status: ticketStatus })
          .then(
            () => {
              this.loadingList$.next(false);
              this.snackBar.open('Tickets updated successfully!!!', 'close', { duration: 1000 });
            },
            (error) => {
              this.loadingList$.next(false);
              this.snackBar.open(error, 'close', { duration: 1000 });
            }
          ).catch(
            (error) => {
              this.loadingList$.next(false);
              this.snackBar.open(error, 'close', { duration: 1000 });
            }
          );
      }
      this.confirmDialogRef = null;
    });
  }

  /**
   * Sort changed
   */
  updateSortDirection(): void {
    if (this.sortDirection === 'ASC') {
      this.sortDirection = 'DESC';
    } else {
      this.sortDirection = 'ASC';
    }

    this._ticketService.getElements('readall', '', this.sortControl.value === 'none' ? '' : this.sortControl.value, this.sortDirection);
  }

  /**
   * Refresh inbox items
   */
  refresh(): void {
    if (!this.isLoading) {
      const value = this.sortControl.value;
      this._ticketService.getElements('readall', '', value === 'none' ? '' : value, this.sortDirection);
    } else {
      console.log('is updating wait...');
    }
  }
}
