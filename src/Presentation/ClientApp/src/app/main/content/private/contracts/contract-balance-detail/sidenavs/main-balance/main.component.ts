import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import * as moment from 'moment';
import { ContractsService } from '../../../contracts.service';
import { ContractBalanceDetailService } from '../../contract-balance-detail.service';

@Component({
  selector: 'app-main-balance-sidenav',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainBalanceComponent implements OnInit {

  filterActive: string;
  filterBy: { [key: string]: any } = {};

  // filter
  dateFrom = moment(moment().add(-1, 'd').toDate()).format('YYYY-MM-DD');
  dateTo = moment(moment().add().toDate()).format('Y  YYY-MM-DD');
  dateFromCtrl = new FormControl('', Validators.required);
  dateToCtrl = new FormControl('', Validators.required);

  constructor(
    private contractService: ContractsService,
    private contractReportBalanceService: ContractBalanceDetailService,
  ) {
    this.filterActive = 'all';
    this.filterBy['status'] = 0;
  }

  ngOnInit(): void {
    this.dateFromCtrl.setValue(this.contractReportBalanceService.dateFrom);
    this.dateToCtrl.setValue(this.contractReportBalanceService.dateTo);
  }

  changeDates(event: MatDatepickerInputEvent<Date>): void {
    this.dateFrom = this.dateFromCtrl.value;
    this.dateTo = this.dateToCtrl.value;

    this.filterBy['dateFrom'] = moment(this.dateFrom).format('YYYY-MM-DD');
    this.filterBy['dateTo'] = moment(this.dateTo).format('YYYY-MM-DD');
    this.contractReportBalanceService.onDatesChangesFilter.next(this.filterBy); this.contractReportBalanceService.onDatesChangesFilter.next({
      'dateFrom': moment(this.dateFrom).format('YYYY-MM-DD'),
      'dateTo': moment(this.dateTo).format('YYYY-MM-DD')
    });
  }

}
