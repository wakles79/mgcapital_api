import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
import { TicketGridModel } from '@app/core/models/ticket/ticket-grid.model';
import { fuseAnimations } from '@fuse/animations';
import { Subject, Subscription } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { TicketsService } from '../tickets.service';

@Component({
  selector: 'app-ticket-list',
  templateUrl: './ticket-list.component.html',
  styleUrls: ['./ticket-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: fuseAnimations
})
export class TicketListComponent implements OnInit, OnDestroy {
  private _unsubscribeAll: Subject<any>;

  // MatPaginator Output
  pageEvent: PageEvent;
  pageSize = 100;
  pageSizeOptions: number[] = [100, 200, 300, 400];

  tickets: TicketGridModel[];
  currentTicket: TicketGridModel;

  onTicketsChanged: Subscription;
  onCurrentTicketChanged: Subscription;

  get ticketsCount(): number { return this._ticketService.elementsCount; }

  routeParams: any;

  constructor(
    private _ticketService: TicketsService,
    private router: Router,
    private route: ActivatedRoute

  ) {
    this._unsubscribeAll = new Subject();
  }

  ngOnInit(): void {
    // Subscribe to update tickets on changes
    this.onTicketsChanged =
      this._ticketService.allElementsChanged
        .pipe(
          takeUntil(this._unsubscribeAll)
        )
        .subscribe((tickets: any[]) => {
          this.tickets = tickets;
        });

    // Subscribe to update current ticket on changes
    this.onCurrentTicketChanged =
      this._ticketService.onCurrentTicketChanged
        .pipe(
          takeUntil(this._unsubscribeAll)
        )
        .subscribe(currentTicket => {
          if (currentTicket) {
            this.currentTicket = currentTicket;
          }
        });

    this.routeParams = this.route.params;

  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  // Options
  viewTicketSummary(id: number): void {
    this.router.navigateByUrl('/app/inbox/ticket-detail/' + id);
    // if (this.routeParams.value.folderHandle !== 'delete') {
    //   this.router.navigateByUrl('/app/inbox/ticket-detail/' + id);
    // }
  }

  /**
   * Get Tickets
   * @param event
   */
  getTickets(event?: PageEvent): void {
    if (!event) {
      return;
    }

    // Sroll top
    // TODO: Improve this and add it to all scrolling events
    const container = document.getElementsByTagName('ticket-list');
    if (container && container.length > 0) {
      container[0].scrollTop = 0;
    }
    this.pageEvent = event;

    // if (this.onTicketsChanged && !this.onTicketsChanged.closed) {
    //   this.onTicketsChanged.unsubscribe();
    // }

    this._ticketService.getElements(
      'readall', '',
      '',
      '',
      this.pageEvent.pageIndex,
      this.pageEvent.pageSize, {});
  }
}
