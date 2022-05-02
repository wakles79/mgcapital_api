import { Resolve, RouterStateSnapshot, ActivatedRouteSnapshot, Route } from '@angular/router';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BuildingBaseModel } from '@app/core/models/building/building-base.model';
import { BuildingsProfileService } from '../buildings-profile.service';

@Injectable({
  providedIn: 'root'
})
export class BuildingsDetailResolver implements Resolve<BuildingBaseModel> {

  constructor(private buildingDetailService: BuildingsProfileService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    const id = route.params.id;
    return this.buildingDetailService.getBuildDetailDate(id);
  }

}
