export class AssignContactParameters {
  entityId: number;
  contactType: string;
  contactId: number;
  type: string;
  showHistoryFrom: number;


  constructor(assignContactParameters: any) {
    this.entityId = assignContactParameters.entityId;
    this.contactType = assignContactParameters.contactType;
    this.contactId = assignContactParameters.contactId;
    this.type = assignContactParameters.type;
    this.showHistoryFrom = assignContactParameters.showHistoryFrom;
  }
}
