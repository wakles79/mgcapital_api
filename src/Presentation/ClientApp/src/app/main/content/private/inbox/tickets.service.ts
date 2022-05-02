import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { TicketGridModel } from '@app/core/models/ticket/ticket-grid.model';
import { TicketBaseModel } from '@app/core/models/ticket/ticket-base.model';
import { BehaviorSubject, Observable, from } from 'rxjs';
import { TicketDetailsModel } from '@app/core/models/ticket/ticket-details.model';
import { UpdateStatusRangeTicketsParameters } from '@app/core/models/ticket/object-parameters/update-status-range-tickets.model';
import { UpdateTicketStatusParameters } from '@app/core/models/ticket/object-parameters/update-ticket-status.model';
import { DeleteRangeTicketsParameters } from '@app/core/models/ticket/object-parameters/delete-range-tickets.model';
import { ConvertTicketParameters } from '@app/core/models/ticket/object-parameters/convert-ticket.model';

@Injectable({
  providedIn: 'root'
})
export class TicketsService extends BaseListService<TicketGridModel> {
  selectedTickets: TicketBaseModel[] = [];
  currentTicket: any;

  onSelectedTicketsChanged: BehaviorSubject<any> = new BehaviorSubject([]);
  onCurrentTicketChanged: BehaviorSubject<any> = new BehaviorSubject([]);
  onTicketUpdated: BehaviorSubject<any> = new BehaviorSubject([]);

  onTicketDestinationChanged: BehaviorSubject<any> = new BehaviorSubject([]);

  onSelectedForlderChanged: BehaviorSubject<any> = new BehaviorSubject([]);

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'tickets', http);
  }


  /**
   * Set current ticket by id
   * @param id
   */
  setCurrentTicket(id): void {
    this.get(id).subscribe((ticket: TicketDetailsModel) => {
      this.currentTicket = ticket;
      this.onCurrentTicketChanged.next(this.currentTicket);
    },
      (error) => {
        this.onCurrentTicketChanged.next([]);
      }
    );
  }

  updateElement(element, action = 'update'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.update(element, action)
        .subscribe((response: TicketDetailsModel) => {
          resolve(response);
          this.getElements();
          this.currentTicket = response;
          this.onCurrentTicketChanged.next(this.currentTicket);
        },
          (error) => {
            reject(error);
          });
    });
  }

  updateStatusRangeTickets(updateStatusRangeTicketsParameters: UpdateStatusRangeTicketsParameters, action = 'updateStatus '): Promise<any> {

    return new Promise((resolve, reject) => {
      this.http.put(`${this.fullUrl}/${action}`, updateStatusRangeTicketsParameters)
        .subscribe((response: any) => {

          this.getElements('readall', '', '', 'DESC', 0, 100, this.filterBy);
          this.deselectTickets();
          this.currentTicket = [];
          this.onCurrentTicketChanged.next(this.currentTicket);

          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  updateTicketStatus(updateTicketStatusParameters: UpdateTicketStatusParameters, action: string = 'updateSingleStatus'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.put(`${this.fullUrl}/${action}`, updateTicketStatusParameters, { observe: 'response' })
        .subscribe((response: any) => {
          resolve(response);
          if (response.status === 200) {
            this.getElements();
            this.onCurrentTicketChanged.next(response.body);
          }
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  deleteTicket(elementToDelete, action = 'delete'): Promise<any> {
    const element = this.allElementsToList.find(e => e.id === elementToDelete.id);
    const elementIndex = this.allElementsToList.indexOf(element);

    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}/?id=${element.id}`, { observe: 'response' })
        .subscribe((response: any) => {
          if (response.status === 200) {
            this.allElementsToList.splice(elementIndex, 1);
            this.allElementsChanged.next(this.allElementsToList);
            this.elementsCount = this.allElementsToList.length;
            this.currentTicket = [];
            this.onCurrentTicketChanged.next([]);
          }
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  deleteRangeTickets(deleteRangeTicketsParameters: DeleteRangeTicketsParameters, action = 'deleteRange '): Promise<any> {

    return new Promise((resolve, reject) => {
      this.http.request('delete', `${this.fullUrl}/${action}`, { body: deleteRangeTicketsParameters })
        .subscribe((response: any) => {

          this.deleteTicketsFromList(deleteRangeTicketsParameters);
          this.deselectTickets();

          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  private deleteTicketsFromList(deleteRangeTicketsParameters: DeleteRangeTicketsParameters): void {

    deleteRangeTicketsParameters.id.forEach(ticketId => {

      const element = this.allElementsToList.find(e => e.id === ticketId);
      const elementIndex = this.allElementsToList.indexOf(element);
      this.allElementsToList.splice(elementIndex, 1);

    });

    this.allElementsChanged.next(this.allElementsToList);
    this.currentTicket = [];
    this.onCurrentTicketChanged.next([]);
  }

  convertTicket(convertTicketParameters: ConvertTicketParameters, action: string = 'convert'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.put(`${this.fullUrl}/${action}`, convertTicketParameters, { observe: 'response' })
        .subscribe((response: any) => {
          resolve(response);
          if (response.status === 200) {
            this.getElements();
            this.onCurrentTicketChanged.next(response.body);
            this.onTicketUpdated.next('converted');
          }
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  getInitFilterTickets(params: any): void {
    // this.filterBy = params;
    this.getElements('readall', '', '', 'DESC', 0, 100, {});
    this.onSelectedForlderChanged.next([]);
    this.deselectTickets();
  }

  filterTickets(params: any): void {

    this.getElements('readall', '', '', 'DESC', 0, 100, params);
  }

  /**
   * Toggle selected ticket by id
   * @param id
   */
  toggleSelectedTicket(id): void {
    // First, check if we already have that ticket as selected...
    if (this.selectedTickets.length > 0) {
      for (const ticket of this.selectedTickets) {
        // ...delete the selected ticket
        if (ticket.id === id) {
          const index = this.selectedTickets.indexOf(ticket);

          if (index !== -1) {
            this.selectedTickets.splice(index, 1);

            this.onSelectedTicketsChanged.next(this.selectedTickets);
            return;
          }
        }
      }
    }

    // If we don't have it, push as selected
    this.selectedTickets.push(
      this.allElementsToList.find(ticket => {
        return ticket.id === id;
      })
    );

    this.onSelectedTicketsChanged.next(this.selectedTickets);
  }

  /**
   * Toggle select all
   */
  toggleSelectAll(): void {
    if (this.selectedTickets.length > 0) {
      this.deselectTickets();
    }
    else {
      this.selectTickets();
    }
  }

  selectTickets(filterParameter?, filterValue?): void {
    this.selectedTickets = [];

    // If there is no filter, select all tickets
    if (filterParameter === undefined || filterValue === undefined) {
      this.selectedTickets = this.allElementsToList;
    }
    else {
      this.selectedTickets.push(...
        this.allElementsToList.filter(ticket => {
          return ticket[filterParameter] === filterValue;
        })
      );
    }

    this.onSelectedTicketsChanged.next(this.selectedTickets);
  }

  deselectTickets(): void {
    this.selectedTickets = [];
    this.onSelectedTicketsChanged.next(this.selectedTickets);
  }

  createTicketFromPreCalendar(elementData): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(elementData, 'AddTicketFromPreCalendar')
        .subscribe(response => {
          this.getElements();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  getActivityLog(
    ticketGuid = null,
    ticketId = null,
    params: { [key: string]: string } = {}): Observable<any> {

    // Default data source filter params
    let queryParams = new HttpParams()
      .set('entityGuid', ticketGuid)
      .set('entityId', ticketId)
      .set('pageNumber', '0')
      .set('pageSize', '9999'); // HACK: As long as it needs

    queryParams = this.extendQueryParams(queryParams, params);
    console.log({ticketGuid});
    return this.http.get(`${this.apiBaseUrl}api/TicketActivityLog/readAll`, {
      params: queryParams
    });
  }
}
