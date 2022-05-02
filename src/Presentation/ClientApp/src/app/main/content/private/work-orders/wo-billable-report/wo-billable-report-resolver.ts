import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Injectable, Inject } from '@angular/core';
import { WorkOrderBaseModel } from '@app/core/models/work-order/work-order-base.model';
import { Observable } from 'rxjs';
import { WorkOrderBillableReportGridModel } from '@app/core/models/work-order-billable-report/wo-billable-report-grid.model';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';
import { WoBillableReportService } from '@app/main/content/private/work-orders/wo-billable-report/wo-billable-report.service';
import { PermissionService } from '@app/core/services/permission.service';

@Injectable({
  providedIn: 'root'
})
export class WorkOrderBillableReportResolver implements Resolve<WorkOrderBillableReportGridModel>
{
  constructor(
    private woBillableReportService: WoBillableReportService,
    private _permissionService: PermissionService
    ) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this._permissionService.getUserModulePermissions(ApplicationModule.WorkOrder);
    this.woBillableReportService.validateModuleAccess(ApplicationModule.BillableReport);
    return this.woBillableReportService.getElements('ReadBillingReport', '', '', '', 0, 50, {});
  }

}
