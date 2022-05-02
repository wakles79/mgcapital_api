import { DatePipe } from '@angular/common';
import { Component, HostBinding, Input, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { TicketDestinationType, TicketSource, TicketStatus, TicketStatusColor } from '@app/core/models/ticket/ticket-base.model';
import { TicketGridModel } from '@app/core/models/ticket/ticket-grid.model';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { fuseAnimations } from '@fuse/animations';
import { Subject, Subscription } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { TicketActivityDialogComponent } from '../../full-ticket-detail/ticket-activity-dialog/ticket-activity-dialog.component';
import { TicketsService } from '../../tickets.service';

@Component({
  selector: 'app-ticket-list-item',
  templateUrl: './ticket-list-item.component.html',
  styleUrls: ['./ticket-list-item.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class TicketListItemComponent implements OnInit, OnDestroy {
  @Input() ticket: any;
  labels: any[];
  @HostBinding('class.selected') selected: boolean;

  private _unsubscribeAll: Subject<any>;

  onSelectedTicketsChanged: Subscription;
  onLabelsChanged: Subscription;

  public REF = {
    TicketSource: TicketSource,
    TicketStatus: TicketStatus,
    TicketDestinationType: TicketDestinationType,
    TicketStatusColor: TicketStatusColor
  };

  expiredSnoozeDate: boolean;

  today = new Date();

  dialogRef: any;

  constructor(
    private _ticketService: TicketsService,
    private _epochPipe: FromEpochPipe,
    private _datePipe: DatePipe,
    private _router: Router,
    private _dialog: MatDialog,
    public _snackBar: MatSnackBar,
  ) {
    this._unsubscribeAll = new Subject();
  }

  ngOnInit(): void {
    // Set the initial values
    this.ticket = new TicketGridModel(this.ticket);

    if (this.ticket.epochSnoozeDate > 0) {
      const snoozeDate = new Date(this._epochPipe.transform(this.ticket.epochSnoozeDate));
      if (this.today > snoozeDate) {
        this.expiredSnoozeDate = true;
      }
    }

    // Subscribe to update on selected ticket change
    this.onSelectedTicketsChanged =
      this._ticketService.onSelectedTicketsChanged
        .pipe(takeUntil(this._unsubscribeAll))
        .subscribe(selectedtickets => {
          this.selected = false;

          if (selectedtickets.length > 0) {
            for (const ticket of selectedtickets) {
              if (ticket.id === this.ticket.id) {
                this.selected = true;
                break;
              }
            }
          }
        });
  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  /**
   * Ticket selection changed
   */
  onSelectedChange(): void {
    this._ticketService.toggleSelectedTicket(this.ticket.id);
  }

  /**
   * Toggle star
   * @param event
   */
  toggleStar(event): void {
    event.stopPropagation();

    // this.ticket.toggleStar();

    this._ticketService.updateElement(this.ticket);
  }

  // Show ticket activity log dialog
  showActivity(): void {
    //event.stopPropagation();
    this.dialogRef = this._dialog.open(TicketActivityDialogComponent, {
      panelClass: 'ticket-activity-dialog',
      data: {
        id: this.ticket.id
      }
    });
  }
  /**
   * Toggle Important
   * @param event
   */
  toggleImportant(event): void {
    event.stopPropagation();

    // this.ticket.toggleImportant();

    this._ticketService.updateElement(this.ticket);
  }

  // Return a date if it has a valid value
  // Valid value is a date different of null or default value
  getValidSnoozeDate(ticket: any): any {

    const snoozeDate = ticket.snoozeDate;

    let possibleDate: any = new Date(snoozeDate);
    const dateToCompare = new Date('2000-01-01');

    if (possibleDate < dateToCompare) {
      return '';
    }
    else {
      possibleDate = this._epochPipe.transform(ticket.epochSnoozeDate);
      return this._datePipe.transform(possibleDate, 'MMM dd hh:mm a');
    }
  }

  // Options
  viewTicketSummary(id: number): void {
    this._router.navigateByUrl('/app/inbox/ticket-detail/' + id);
  }
}
