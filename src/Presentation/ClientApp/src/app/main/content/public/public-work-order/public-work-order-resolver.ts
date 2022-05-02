import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { WorkOrdersService } from '@app/main/content/private/work-orders/work-orders.service';
import { WorkOrderDetailsModel } from '@app/core/models/work-order/work-order-details.model';

@Injectable({
  providedIn: 'root'
})
export class PublicWorkOrderResolver implements Resolve<WorkOrderDetailsModel>
{
    constructor(private woService: WorkOrdersService ){}

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
      ): Observable<any>|Promise<any>|any {
        return this.woService.getWorkOrderPublic(route.params.guid);
      }

}
