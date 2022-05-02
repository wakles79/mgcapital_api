import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';
import { Observable } from 'rxjs';
import { InboxService } from './inbox.service';
import { TicketsService } from './tickets.service';

@Injectable({
  providedIn: 'root'
})
export class TicketsResolver implements Resolve<any> {

  routeParams: any;

  constructor(
    private ticketsService: TicketsService,
    private inboxService: InboxService) {
  }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {

    this.ticketsService.validateModuleAccess(ApplicationModule.Inbox);

    this.ticketsService.viewName = 'inbox-list';
    this.ticketsService.loadSessionFilter();

    this.routeParams = route.params;


    if (this.routeParams.folderHandle) {
      const folderHandle = this.routeParams.folderHandle;

      // this.inboxService.onComponentChanged.next(InboxMainViewComponent);
      // this.ticketsService.onCurrentTicketChanged.next([]);

      if (folderHandle === 'pending') {
        this.ticketsService.filterBy['status'] = '1';
        this.ticketsService.filterBy['showSnoozed'] = 'false';
        this.ticketsService.filterBy['IsDeleted'] = 'false';
        return this.ticketsService.getInitFilterTickets({});
      }
      else if (folderHandle === 'resolved') {
        this.ticketsService.filterBy['status'] = '6';
        this.ticketsService.filterBy['showSnoozed'] = 'false';
        this.ticketsService.filterBy['IsDeleted'] = 'false';
        return this.ticketsService.getInitFilterTickets({});
      }
      else if (folderHandle === 'snoozed') {
        this.ticketsService.filterBy['status'] = '1';
        this.ticketsService.filterBy['showSnoozed'] = 'true';
        this.ticketsService.filterBy['IsDeleted'] = 'false';
        return this.ticketsService.getInitFilterTickets({});
      }
      else if (folderHandle === 'delete') {
        this.ticketsService.filterBy['status'] = '1';
        this.ticketsService.filterBy['showSnoozed'] = 'false';
        this.ticketsService.filterBy['IsDeleted'] = 'true';
        return this.ticketsService.getInitFilterTickets({});
      }
    }

  }

}
