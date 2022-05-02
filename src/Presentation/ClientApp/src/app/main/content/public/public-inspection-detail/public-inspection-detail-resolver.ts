import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { InspectionDetailService } from '@app/main/content/private/inspections/inspection-detail/inspection-detail.service';
import { InspectionDetailModel } from '@app/core/models/inspections/inspection-detail.model';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PublicInspectionDetailResolver implements Resolve<InspectionDetailModel>{

  constructor(private inspectionDetailService: InspectionDetailService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    const inspectionGuid = route.params.guid;
    return this.inspectionDetailService.getPublicDetails(inspectionGuid);
  }

}
