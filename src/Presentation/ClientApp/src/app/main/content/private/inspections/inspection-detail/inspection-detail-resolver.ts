import { Injectable } from '@angular/core';
import { InspectionDetailService } from './inspection-detail.service';
import { Observable } from 'rxjs';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Resolve } from '@angular/router';
import { InspectionDetailModel } from '@app/core/models/inspections/inspection-detail.model';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class InspectionDetailResolver implements Resolve<InspectionDetailModel> {

  constructor(private inspectionDetailService: InspectionDetailService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.inspectionDetailService.validateModuleAccess(ApplicationModule.Inspections);
    const id = route.params.id;
    return this.inspectionDetailService.getDetails(id);
  }

}
