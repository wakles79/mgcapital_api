import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { OfficeTypesService } from './office-types.service';
import { Observable } from 'rxjs';
import { OfficeTypeBaseModel } from '@app/core/models/office-type/office-type-base.model';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class OfficeTypesResolver implements Resolve<OfficeTypeBaseModel>
{
  constructor(private officeTypeService: OfficeTypesService) { }

  resolve(
    router: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.officeTypeService.validateModuleAccess(ApplicationModule.ContractServicesCatalog);
    return this.officeTypeService.getElements();
  }
}
