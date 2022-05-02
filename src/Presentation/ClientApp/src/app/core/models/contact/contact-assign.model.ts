export class ContactAssignModel {
  contactId: number;
  entityId: number;
  contactType: string;
  type: string;
  default: boolean;

  constructor(contactAssign: ContactAssignModel){
    this.contactId = contactAssign.contactId || null;
    this.entityId = contactAssign.entityId || null;
    this.contactType = contactAssign.contactType || '';
    this.type = contactAssign.type || '';
    this.default = contactAssign.default || false;
  }
}
