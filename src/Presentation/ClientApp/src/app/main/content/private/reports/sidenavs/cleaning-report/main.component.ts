import { Component, OnInit, OnDestroy } from '@angular/core';
import * as moment from 'moment';
import { CLEANING_REPORT_STATUSES } from '@app/core/models/reports/cleaning-report/cleaning-report-status.model';
import { FormControl, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { CleaningReportService } from '../../cleaning-report/cleaning-report.service';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { fuseAnimations } from '@fuse/animations';

@Component({
  selector: 'app-cleaning-report-sidebar',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit, OnDestroy {

  cleaningReportStatuses = CLEANING_REPORT_STATUSES;
  filterActive: string;
  dateFrom = moment(moment().add(-1, 'd').toDate()).format('YYYY-MM-DD');
  dateTo = moment(moment().add().toDate()).format('YYYY-MM-DD');

  filterBy: { [key: string]: any } = {
    'statusid': 'null',
    'commentDirection': 'null',
    'dateFrom': '',
    'dateTo': '',
  };

  dateFromCtrl = new FormControl('', Validators.required);
  dateToCtrl = new FormControl('', Validators.required);

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  constructor(
    private cleaningReportService: CleaningReportService,
  ) {
  }

  ngOnInit(): void {
    this.filterBy = this.cleaningReportService.filterBy;

    this.filterActive = this.cleaningReportStatuses.find(status => status.id === this.cleaningReportService.filterBy['statusid']).value;

    this.dateFromCtrl.setValue(this.cleaningReportService.dateFrom);
    this.dateToCtrl.setValue(this.cleaningReportService.dateTo);
  }

  changeFilter(filter: any): void {
    this.filterActive = filter;
    const statusId = this.cleaningReportStatuses.find(status => status.value === this.filterActive).id;

    this.filterBy['statusid'] = statusId;
    this.cleaningReportService.onFilterChanged.next(this.filterBy);
  }

  changeCommentsFilter(value: any): void {
    this.filterBy['commentDirection'] = value;
    this.cleaningReportService.onFilterChanged.next(this.filterBy);
  }

  changeDates(event: MatDatepickerInputEvent<Date>): void {
    this.dateFrom = this.dateFromCtrl.value;
    this.dateTo = this.dateToCtrl.value;

    this.filterBy['dateFrom'] = moment(this.dateFrom).format('YYYY-MM-DD');
    this.filterBy['dateTo'] = moment(this.dateTo).format('YYYY-MM-DD');
    this.cleaningReportService.onFilterChanged.next(this.filterBy);
    this.cleaningReportService.onDatesChanges.next({ 'dateFrom': this.dateFrom, 'dateTo': this.dateTo });
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

}
