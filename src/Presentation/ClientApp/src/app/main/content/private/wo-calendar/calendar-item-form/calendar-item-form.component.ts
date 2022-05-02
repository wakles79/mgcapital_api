import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSelectChange } from '@angular/material/select';
import { CalendarItemFrequencyCreateModel } from '@app/core/models/calendar/calendar-item-frequency-create.model';
import { CALENDAR_FREQUENCY, CALENDAR_MONTH } from '@app/core/models/calendar/calendar-periodicity-enum';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { WorkOrderUpdateModel } from '@app/core/models/work-order/work-order-update.model';
import { WorkOrderSharedFormComponent } from '@app/core/modules/work-order-form/work-order-form/work-order-form.component';

@Component({
  selector: 'app-calendar-item-form',
  templateUrl: './calendar-item-form.component.html',
  styleUrls: ['./calendar-item-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CalendarItemFormComponent implements OnInit {

  action: string;
  dialogTitle: string;

  calendarItemForm: FormGroup;

  itemTypes: ListItemModel[] = [{ id: 0, name: 'Work Order' }];
  frequencies: ListItemModel[] = [];
  months: ListItemModel[] = [];
  selectedMonths: number[] = [];
  quarterlyMonths: ListItemModel[] = [];

  currentFrequency = 0;

  today: Date = new Date();

  workOrderFormDialog: MatDialogRef<WorkOrderSharedFormComponent>;

  workOrder: WorkOrderUpdateModel = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    private _formBuilder: FormBuilder,
    public dialogRef: MatDialogRef<CalendarItemFormComponent>,
    private _dialog: MatDialog
  ) {
    this.action = data.action;

    if (this.action === 'new') {
      this.dialogTitle = 'New Item';
      this.calendarItemForm = this.createCalendarItemForm();
    } else if (this.action === 'edit') {
      this.dialogTitle = 'Update Item';
    }
  }

  ngOnInit(): void {
    this.getFrequencies();
    this.getMonths();

    this.data.hasOwnProperty('workOrder') ? this.workOrder = this.data.workOrder : this.workOrder = null;
    if (this.workOrder != null) {
      this.workOrder.sendNotifications = false;
    }
  }

  // Form
  createCalendarItemForm(): FormGroup {
    return this._formBuilder.group({
      type: [0, [Validators.required]],
      quantity: [{ value: 1, disabled: true }, [Validators.required]],
      frequency: [0, [Validators.required]],
      startDate: ['', [Validators.required]]
    });
  }

  // Aux
  getFrequencies(): void {
    // tslint:disable-next-line: forin
    for (const frequency in CALENDAR_FREQUENCY) {
      if (typeof CALENDAR_FREQUENCY[frequency] === 'number') {
        this.frequencies.push({ id: CALENDAR_FREQUENCY[frequency] as any, name: frequency.replace(/([A-Z])/g, ' $1').trim() });
      }
    }
  }
  getMonths(): void {
    // tslint:disable-next-line: forin
    for (const month in CALENDAR_MONTH) {
      if (typeof CALENDAR_MONTH[month] === 'number') {
        this.months.push({ id: CALENDAR_MONTH[month] as any, name: month });
      }
    }
  }

  // Frequency
  frequencyChanged(event: MatSelectChange): void {

    if (CALENDAR_FREQUENCY[CALENDAR_FREQUENCY[this.currentFrequency]] === CALENDAR_FREQUENCY.OneTimeOnly) {
      this.calendarItemForm.controls['quantity'].reset({ value: 1, disabled: false });
    }

    this.currentFrequency = event.value;

    switch (CALENDAR_FREQUENCY[CALENDAR_FREQUENCY[event.value]]) {
      case CALENDAR_FREQUENCY.OneTimeOnly:
        this.calendarItemForm.controls['quantity'].reset({ value: 1, disabled: true });
        break;
    }
  }

  // Month
  quarterlyStartMonthChanged(event: MatSelectChange): void {
    const nextMonths = this.months.filter(m => m.id >= event.value);
    const lastMonths = this.months.filter(m => m.id < event.value);

    this.quarterlyMonths = nextMonths;

    lastMonths.forEach(m => {
      this.quarterlyMonths.push(m);
    });
  }
  quarterlyCustomMonthChecked(event: MatCheckboxChange, month: number): void {
    if (event.checked) {
      this.selectedMonths.push(month);
    } else {
      const index = this.selectedMonths.findIndex(m => m === month);
      if (index >= 0) {
        this.selectedMonths.splice(index, 1);
      }
    }
  }

  // Buttons
  submit(): void {
    const calendarItem = new CalendarItemFrequencyCreateModel(this.calendarItemForm.getRawValue());

    this.workOrderFormDialog = this._dialog.open(WorkOrderSharedFormComponent, {
      panelClass: 'work-order-form-dialog',
      data: {
        action: 'new',
        workOrder: this.workOrder === null ? null : this.workOrder,
        workOrderFromClone: this.workOrder === null ? null : true,
        sendNotifications: false,
        fromCalendar: true
      }
    });

    this.workOrderFormDialog.afterClosed().subscribe(response => {
      if (!response) {
        return;
      }

      if (response[0] === 'close') {
        return;
      }

      calendarItem.workOrder = response.getRawValue();
      calendarItem.months = this.selectedMonths;

      this.dialogRef.close(calendarItem);
    });
  }

}
