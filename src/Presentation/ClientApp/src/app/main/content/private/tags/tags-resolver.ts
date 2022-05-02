import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { TagBaseModel } from '@app/core/models/tag/tag-base.model';
import { Observable } from 'rxjs';
import { TagsService } from './tags.service';

@Injectable({
  providedIn: 'root'
})
export class TagsResolver implements Resolve<TagBaseModel>{

  constructor(private tagService: TagsService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.tagService.onFilterChanged.next({});
  }
}
