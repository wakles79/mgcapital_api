import { CompanyEntity } from '@core/models/common/company-entity.model';
import { TicketAttachmentBaseModel } from '@app/core/models/ticket/ticket-attachment-base.model';
import { TicketAdditionalData } from '@app/core/models/ticket/ticket-additional-data.model';

export class TicketBaseModel extends CompanyEntity {
  number: number;
  source: TicketSource;
  status: TicketStatus;
  destinationType: TicketDestinationType;
  destinationEntityId: number;
  description: string;
  fullAddress: string;
  buildingId: number;

  requesterFullName: string;
  requesterEmail: string;
  requesterPhone: string;
  snoozeDate: Date;
  attachments: TicketAttachmentBaseModel[];
  attachmentsCount: number;

  entityNumber: number;
  buildingName: string;
  epochSnoozeDate: number;
  epochCreatedDate: Date;
  epochUpdatedDate: Date;

  data: TicketAdditionalData;
  location: string;

  tasks: any[];
  type: number;
  priority: number;
  freshdeskTicketId: number;
  assignedEmployeeId: number;
  assignedEmployeeName: string;
  pendingReview: boolean;
  newRequesterResponse: boolean;
  isDeleted?: boolean; // hce_013
  messageId: string; // GMail Message Id
  emailSignature?: string;

  constructor(ticket: TicketBaseModel) {
    super(ticket);
    this.number = ticket.number || 0;
    this.source = ticket.source || TicketSource.undefined;
    this.status = ticket.status || TicketStatus.undefined;
    this.destinationType = ticket.destinationType || TicketDestinationType.undefined;
    this.destinationEntityId = ticket.destinationEntityId || null;
    this.description = ticket.description || '';
    this.fullAddress = ticket.fullAddress || '';
    this.buildingId = ticket.buildingId || null;
    this.requesterFullName = ticket.requesterFullName || '';
    this.requesterEmail = ticket.requesterEmail || '';
    this.requesterPhone = ticket.requesterPhone || '';
    this.snoozeDate = ticket.snoozeDate || null;
    this.attachments = ticket.attachments || [];
    this.attachmentsCount = ticket.attachmentsCount || 0;

    this.entityNumber = ticket.entityNumber || null;
    this.buildingName = ticket.buildingName || '';
    this.epochSnoozeDate = ticket.epochSnoozeDate || 0;
    this.epochCreatedDate = ticket.epochCreatedDate || null;
    this.epochUpdatedDate = ticket.epochUpdatedDate || null;

    this.data = ticket.data;
    this.location = ticket.location;

    this.tasks = ticket.tasks || [];
    this.type = ticket.type || null;
    this.priority = ticket.priority || null;
    this.freshdeskTicketId = ticket.freshdeskTicketId || null;
    this.assignedEmployeeId = ticket.assignedEmployeeId || null;
    this.assignedEmployeeName = ticket.assignedEmployeeName || null;
    this.pendingReview = ticket.pendingReview || false;
    this.newRequesterResponse = ticket.newRequesterResponse || false;
    this.messageId = ticket.messageId || null;
    this.isDeleted = ticket.isDeleted || false; // hce_013
    this.emailSignature = ticket.emailSignature;
  }
}

export enum TicketSource {
  'undefined' = 0,
  'Client App' = 1,
  'Work Order Form' = 2,
  'Manager App' = 4,
  'Internal Ticket' = 8,
  'Inspection' = 16,
  'Calendar' = 32,
  'Email' = 64
}

export enum TicketStatus {
  undefined = 0,
  pending = 1,
  converted = 2,
  closed = 4
}

export enum TicketDestinationType {
  undefined = 0,
  workOrder = 1,
  cleaningItem = 2,
  findingItem = 4
}

export enum TicketStatusColor {
  undefined = 'grey',
  pending = 'red',
  converted = 'green',
  closed = 'green'
}
