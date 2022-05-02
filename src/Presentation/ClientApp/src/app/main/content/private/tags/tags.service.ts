import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { TagGridModel } from '@app/core/models/tag/tag-grid.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TagsService extends BaseListService<TagGridModel>{

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient
  ) {
    super(apiBaseUrl, 'tags', http);
  }

  getTagsAsList(action = 'ReadAllCbo'): Observable<any> {
    return this.http.get(`${this.fullUrl}/${action}`);
  }

  deleteTag(id, action = 'Delete'): Observable<any> {
    const params = new HttpParams()
      .set('id', id);
    return this.http.delete(`${this.fullUrl}/${action}`, { params: params });
  }

  readAllTicketTags(ticketId, action = 'ReadAllTicketTags'): Observable<any> {
    const params = new HttpParams()
      .set('ticketId', ticketId);

    return this.http.get(`${this.fullUrl}/${action}`, { params: params });
  }

  removeTicketTag(ticketTagId, action = 'RemoveTicketTag'): Observable<any> {
    const params = new HttpParams()
      .set('TicketTagId', ticketTagId);

    return this.http.delete(`${this.fullUrl}/${action}`, { params: params });
  }

}
