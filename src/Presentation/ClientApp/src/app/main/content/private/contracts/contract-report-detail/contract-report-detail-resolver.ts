import { Resolve, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ContractReportDetailsModel } from '@app/core/models/contract/contract-report-detail.model';
import { ContractReportDetailService } from './contract-report-detail.service';

@Injectable({
  providedIn: 'root'
})
export class ContractReportDetailResolver implements Resolve<ContractReportDetailsModel>{

  constructor(private contractService: ContractReportDetailService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    const contractId = route.params.id;
    return this.contractService.getDetails(contractId);
  }
}
