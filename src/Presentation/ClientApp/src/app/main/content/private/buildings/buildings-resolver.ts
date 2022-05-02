import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { BuildingsService } from './buildings.service';
import { BuildingBaseModel } from '@app/core/models/building/building-base.model';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})

export class BuildingsResolver implements Resolve<BuildingBaseModel>
{
    constructor(private buildingService: BuildingsService ) {}

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
      ): Observable<any>|Promise<any>|any {
        this.buildingService.validateModuleAccess(ApplicationModule.Buildings);
        return this.buildingService.onFilterChanged.next({});
      }

}
