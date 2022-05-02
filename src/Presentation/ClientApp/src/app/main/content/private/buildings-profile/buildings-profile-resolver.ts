import { Resolve, RouterStateSnapshot, ActivatedRouteSnapshot, Route } from '@angular/router';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BuildingBaseModel } from '@app/core/models/building/building-base.model';
import { BuildingsService } from '../buildings/buildings.service';

@Injectable({
  providedIn: 'root'
})
export class BuildingsProfileResolver implements Resolve<BuildingBaseModel> {

  constructor(private buildingService: BuildingsService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    const id = route.params.id;
    return this.buildingService.getElements('ReadAllBuildingProfile');
  }

}
