import { InspectionBaseModel } from '../inspections/inspection-base.model';

export class InspectionGridModel extends InspectionBaseModel {

  createdDate: number;
  items: number;
  closedItems: number;
  tasks: number;
  completedTasks: number;
  notesCount: number;
  buildingName: string;
  employeeName: string;
  statusName: string;
  epochDueDate: number;
  epochCloseDate: number;

  constructor(inspectionGridModel: InspectionGridModel) {
    super(inspectionGridModel);

    this.createdDate = inspectionGridModel.createdDate || 0;
    this.items = inspectionGridModel.items || 0;
    this.closedItems = inspectionGridModel.closedItems || 0;
    this.buildingName = inspectionGridModel.buildingName || '';
    this.employeeName = inspectionGridModel.employeeName || '';
    this.statusName = inspectionGridModel.statusName || '';
    this.epochDueDate = inspectionGridModel.epochDueDate || 0;
    this.epochCloseDate = inspectionGridModel.epochCloseDate || 0;
    this.tasks = inspectionGridModel.tasks || 0;
    this.completedTasks = inspectionGridModel.completedTasks || 0;
    this.notesCount = inspectionGridModel.notesCount || 0;
  }

}
