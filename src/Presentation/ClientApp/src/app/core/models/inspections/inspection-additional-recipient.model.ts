export class InspectionAdditionalRecipientModel{
  fullName: string;
  email: string;

  constructor(inspectionAdditionalRecipient: InspectionAdditionalRecipientModel){
    this.fullName = inspectionAdditionalRecipient.fullName;
    this.email = inspectionAdditionalRecipient.email;
  }
}
