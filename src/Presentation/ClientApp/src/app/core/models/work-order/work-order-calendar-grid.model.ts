import { EntityModel } from '../common/entity.model';

export class WorkOrderCalendarGridModel extends EntityModel {

  originWorkOrderId: string;
  clonePath: string;
  number: number;
  description: string;
  buildingName: string;
  location: string;
  requesterFullName: string;
  dateSubmitted: Date;
  epochDateSubmitted: number;
  dueDate: Date;
  epochDueDate: number;
  isExpired: number;
  sendRequesterNotifications: boolean;
  sendPropertyManagersNotifications: boolean;
  notesCount: number;
  tasksCount: number;
  tasksDoneCount: number;
  tasksBillableCount: number;
  attachmentsCount: number;
  type: number;
  scheduleDate: Date;
  epochScheduleDate: number;
  sequencePosition: number;

  guid: string;
  calendarItemFrequencyId: number;

  categoryName: string;
  subcategoryName: string;

  colorName: string;
  unscheduled: boolean;
  workOrderScheduleSettingId: number;
  elementsInSequence: number;

  constructor(woCalendar: WorkOrderCalendarGridModel) {
    super(woCalendar);

    this.originWorkOrderId = woCalendar.originWorkOrderId || '';
    this.clonePath = woCalendar.clonePath || '';
    this.number = woCalendar.number || 0;
    this.sequencePosition = woCalendar.sequencePosition || 0;
    this.description = woCalendar.description || '';
    this.buildingName = woCalendar.buildingName || '';
    this.location = woCalendar.location || '';
    this.requesterFullName = woCalendar.requesterFullName || '';
    this.dateSubmitted = woCalendar.dateSubmitted || new Date();
    this.epochDateSubmitted = woCalendar.epochDateSubmitted || 0;
    this.dueDate = woCalendar.dueDate || null;
    this.epochDueDate = woCalendar.epochDueDate || 0;
    this.isExpired = woCalendar.isExpired || 0;

    this.sendRequesterNotifications = woCalendar.sendRequesterNotifications || false;
    this.sendPropertyManagersNotifications = woCalendar.sendPropertyManagersNotifications || false;
    this.notesCount = woCalendar.notesCount || 0;
    this.tasksCount = woCalendar.tasksCount || 0;
    this.tasksDoneCount = woCalendar.tasksDoneCount || 0;
    this.tasksBillableCount = woCalendar.tasksBillableCount || 0;
    this.attachmentsCount = woCalendar.attachmentsCount || 0;

    this.type = woCalendar.type || 0;

    this.guid = woCalendar.guid;

    this.calendarItemFrequencyId = woCalendar.calendarItemFrequencyId || 0;

    this.scheduleDate = woCalendar.scheduleDate || null;
    this.epochScheduleDate = woCalendar.epochScheduleDate || 0;

    this.categoryName = woCalendar.categoryName || '';
    this.subcategoryName = woCalendar.subcategoryName || '';

    this.colorName = woCalendar.colorName || 'black';
    this.unscheduled = woCalendar.unscheduled || false;
    this.workOrderScheduleSettingId = woCalendar.workOrderScheduleSettingId || 0;
    this.elementsInSequence = woCalendar.elementsInSequence || 0;
  }

}
