import { ContractReportDetailsModel } from '@app/core/models/contract/contract-report-detail.model';
import { ContractReportDetailService } from '../../private/contracts/contract-report-detail/contract-report-detail.service';
import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PublicContractReportResolver implements Resolve<ContractReportDetailsModel>{
  constructor(private contractService: ContractReportDetailService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    const contractGuid = route.params.guid;
    return this.contractService.getPublicDetails(contractGuid);
  }
}
