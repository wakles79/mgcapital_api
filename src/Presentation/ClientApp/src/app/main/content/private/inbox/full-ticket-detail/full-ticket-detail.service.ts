import { Inject, Injectable } from '@angular/core';
import { TicketBaseModel } from '@app/core/models/ticket/ticket-base.model';
import { BaseListService } from '@app/core/services/base-list.service';
import { BehaviorSubject, Observable } from 'rxjs';
import { FdTicketDetailModel } from '@app/core/models/freshdesk/fd-ticket-detail.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { FdTicketReplyModel } from '@app/core/models/freshdesk/fd-ticket-reply.model';
import { UpdateTicketStatusParameters } from '@app/core/models/ticket/object-parameters/update-ticket-status.model';
@Injectable({
  providedIn: 'root'
})
export class FullTicketDetailService extends BaseListService<any> {

  ticket: TicketBaseModel;
  onTicketChanged: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  freshdeskTicket: FdTicketDetailModel;
  onFreshdeskTicketChanged: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  onIdentifierChaged: BehaviorSubject<number> = new BehaviorSubject<number>(null);

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'tickets', http);

    this.onTicketChanged = new BehaviorSubject<any>(null);
  }

  public setId(id: number): void {
    this.onIdentifierChaged.next(id);
  }

  public getTicketDetails(id: number, action = 'GetTicketDetail'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.get(`${this.fullUrl}/${action}/${id}`)
        .subscribe((response: any) => {
          console.log('server resopnse');
          resolve(response);

          this.ticket = new TicketBaseModel(response);
          this.onTicketChanged.next(this.ticket);

        }, (error) => { reject(error); });
    });
  }

  public getTicketsCbo(type, value: string, action: string = 'ReadAllCboToMerge'): Observable<any> {
    const httpParams = new HttpParams()
      .set('paramType', type)
      .set('value', value);

    return this.http.get(`${this.fullUrl}/${action}`, {
      params: httpParams
    });
  }

  public mergeTickets(resource: { ticketId: number, ticketsId: number[] }, action = 'MergeTickets'): Observable<any> {
    return this.create(resource, action);
  }

  public sendTicketReply(replyObject: FdTicketReplyModel, files: File[], ticketId: number, action = 'ReplyTicket'): Observable<any> {

    const resource = new FormData();
    resource.append('ReplyModel', JSON.stringify(replyObject));
    resource.append('TicketId', ticketId.toString());

    for (let i = 0; i < files.length; i++) {
      resource.append('Attachments', files[i], files[i].name);
    }

    return this.http.post(`${this.fullUrl}/${action}`, resource, { observe: 'response' });
  }

  public sendTicketForward(replyObject: FdTicketReplyModel, to: string, ticketId: string, files: File[], action = 'ForwardTicket'): Observable<any> {

    const resource = new FormData();
    resource.append('StrReply', JSON.stringify(replyObject));
    resource.append('To', to);
    resource.append('TicketId', ticketId);

    for (let i = 0; i < files.length; i++) {
      resource.append('Attachments', files[i], files[i].name);
    }

    return this.http.post(`${this.fullUrl}/${action}`, resource, { observe: 'response' });
  }

  public addTicketAttachment(ticketId: number, files: File[], action = 'AddTicketAttachment'): Observable<any> {
    const resource = new FormData();
    resource.append('TicketId', ticketId.toString());
    resource.append('File', files[0], files[0].name);

    return this.http.post(`${this.fullUrl}/${action}`, resource, { observe: 'response' });
  }

  public copyImageToTicket(object, action: string = 'CopyFreshdeskImageToTicket'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, object);
  }

  public downloadImage(url: string, name: string, type: string, action: string = 'DownloadFreshdeskImage'): Observable<any> {
    const httpParams = new HttpParams()
      .set('url', url)
      .set('name', name)
      .set('fileType', type);

    return this.http.get(`${this.fullUrl}/${action}`, { params: httpParams, responseType: 'blob' });
  }

  public deleteTicket(id: number, action = 'delete'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}/?id=${id}`, { observe: 'response' })
        .subscribe((response: any) => {
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
        },
          (error) => {
            reject(error.error);
          });
    });
  }
}
