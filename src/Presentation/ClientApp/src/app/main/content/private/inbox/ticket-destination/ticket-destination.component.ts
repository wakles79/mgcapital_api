import { Component, OnInit, OnDestroy, ViewChild, ViewContainerRef, Inject } from '@angular/core';
import { TicketBaseModel, TicketDestinationType, TicketSource, TicketStatus } from '@app/core/models/ticket/ticket-base.model';
import { fuseAnimations } from '@fuse/animations';
import { Subscription } from 'rxjs';
import { DynamicTicketComponent, TicketDestination } from '@app/core/models/ticket/ticketDestination';
import { InboxService } from '../inbox.service';
import { TicketsService } from '../tickets.service';
import { MatDialog } from '@angular/material/dialog';
import { TicketDestinationLoaderService } from '../ticket-destination-loader.service';
import { AuthService } from '@app/core/services/auth.service';
import { WorkOrderBaseModel, WorkOrderSourceCode } from '@app/core/models/work-order/work-order-base.model';
import { WoAttachmentCreateFromTicketModel } from '@app/core/models/work-order/wo-attachment-create-from-ticket.model';
import { WorkOrderTaskModel } from '@app/core/models/work-order/work-order-task.model';
import { CLEANING_REPORT_ITEM_TYPES } from '@app/core/models/reports/cleaning-report/item-type.model';
import { CleaningReportItemBaseModel } from '@app/core/models/reports/cleaning-report/cleaning.report.item.model';
import { CleaningReportItemAttachmentCreateFromTicketModel } from '@app/core/models/reports/cleaning-report/cr-attachment-create-from-ticket.model';

@Component({
  selector: 'app-ticket-destination',
  templateUrl: './ticket-destination.component.html',
  styleUrls: ['./ticket-destination.component.scss'],
  animations: fuseAnimations
})
export class TicketDestinationComponent implements OnInit, OnDestroy {

  ticket: TicketBaseModel;
  ticketDestination: TicketDestination = new TicketDestination();

  onCurrentTicketChanged: Subscription;
  onTicketDestinationChanged: Subscription;

  dynamicTicketComponents = DynamicTicketComponent;

  public REF = {
    TicketSource: TicketSource,
    TicketStatus: TicketStatus,
    TicketDestinationType: TicketDestinationType
  };

  dialogRef: any;

  @ViewChild('dynamicDestination', {
    read: ViewContainerRef,
    static: true
  }) viewContainerRef: ViewContainerRef;

  constructor(
    private inboxService: InboxService,
    private ticketService: TicketsService,
    private dialog: MatDialog,
    private ticketDestinationLoaderService: TicketDestinationLoaderService,
    private authService: AuthService,
    @Inject(ViewContainerRef) viewContainerRef,
  ) {
  }

  ngOnInit(): void {
    // Subscribe to update the current ticket
    this.onCurrentTicketChanged =
      this.ticketService.onCurrentTicketChanged
        .subscribe(currentTicket => {
          this.ticket = currentTicket;
          // console.log({currentTicket});
        });

    this.onTicketDestinationChanged =
      this.ticketService.onTicketDestinationChanged.subscribe((destination: TicketDestinationType) => {

        this.ticketDestination.destinationType = destination;
        this.resolveDynamicComponentToLoad();

      });
  }

  resolveDynamicComponentToLoad(): void {

    const dynamicComponent = this.dynamicTicketComponents[this.ticketDestination.destinationType.toString()];
    this.ticketDestination.componentToLoad = dynamicComponent;

    if (this.ticketDestination.destinationType.toString() === this.REF.TicketDestinationType[this.REF.TicketDestinationType.workOrder]) {
      this.addDynamicComponent(this.getWorkOrderDestinationData());
    }

    else if (this.ticketDestination.destinationType.toString() === this.REF.TicketDestinationType[this.REF.TicketDestinationType.cleaningItem] ||
      this.ticketDestination.destinationType.toString() === this.REF.TicketDestinationType[this.REF.TicketDestinationType.findingItem]) {
      this.addDynamicComponent(this.getCleaningReportDestinationData());

    }
  }

  addDynamicComponent(destinationData: any = null): void {
    this.viewContainerRef.clear();
    this.ticketDestinationLoaderService.setRootViewContainerRef(this.viewContainerRef);
    this.ticketDestinationLoaderService.addDynamicComponent(this.ticketDestination.componentToLoad, destinationData);
  }

  getWorkOrderDestinationData(): {} {
    let data = {};
    const workOrder = new WorkOrderBaseModel();
    workOrder.type = this.ticket.type;
    workOrder.priority = this.ticket.priority;
    workOrder.description = this.ticket.description;
    workOrder.requesterFullName = this.ticket.requesterFullName;
    workOrder.requesterEmail = this.ticket.requesterEmail;
    workOrder.buildingId = this.ticket.buildingId;
    workOrder.location = this.ticket.location;
    workOrder.workOrderSourceCode = this.getWoSourceCodeFromTicketSource(this.ticket.source);
    // set status to Pending
    workOrder.statusId = 1;

    this.ticket.attachments.forEach(attachment => {
      const woAttachement = new WoAttachmentCreateFromTicketModel();
      woAttachement.blobName = attachment.blobName;
      woAttachement.fullUrl = attachment.fullUrl;
      woAttachement.description = attachment.description;
      woAttachement.employeeId = this.authService.currentUser.employeeId;

      workOrder.attachments.push(woAttachement);
    });

    try {
      this.ticket.tasks.forEach(task => {
        const taskBase: any = {
          description: task.description
        };
        const woTask = new WorkOrderTaskModel(taskBase);
        workOrder.tasks.push(woTask);
      });
    } catch (error) {
      console.log({ error });
    }

    data = { action: 'newFromTicket', workOrder, source: WorkOrderSourceCode.Ticket, showCloseButton: false };
    return data;
  }

  getWoSourceCodeFromTicketSource(ticketSource: TicketSource): any {

    /*
        TICKET SOURCE   | WO Source
      ---------------------------------------------
      Manager App     | Internal Mobile Application
      Client App      | External Mobile Application
      Work Order Form | Landing Page
    */
    const ticketSourceWoSourceCodeMap = [
      { ticketSource: TicketSource['undefined'], woSourceCode: 0 },
      { ticketSource: TicketSource['Manager App'], woSourceCode: 10 },
      { ticketSource: TicketSource['Client App'], woSourceCode: 11 },
      { ticketSource: TicketSource['Work Order Form'], woSourceCode: 12 }
    ];

    const ticketWoSource = ticketSourceWoSourceCodeMap.find(item => item.ticketSource === ticketSource);

    return ticketWoSource ? ticketWoSource.woSourceCode : 0;
  }

  getCleaningReportDestinationData(): any {
    let data = {};
    const itemType = CLEANING_REPORT_ITEM_TYPES.find(item => item.value === this.ticketDestination.destinationType.toString());

    const cleaningReportItem = new CleaningReportItemBaseModel();
    cleaningReportItem.observances = this.ticket.description;
    cleaningReportItem.type = itemType.id;
    cleaningReportItem.location = this.ticket.location;

    this.ticket.attachments.forEach(attachment => {
      const crAttachement = new CleaningReportItemAttachmentCreateFromTicketModel();
      crAttachement.blobName = attachment.blobName;
      crAttachement.fullUrl = attachment.fullUrl;
      crAttachement.title = attachment.description;

      cleaningReportItem.attachments.push(crAttachement);
    });

    data = {
      action: 'newFromTicket',
      cleaningReportItem: cleaningReportItem,
      itemType: itemType,
      customerContactId: null,
      showCloseButton: false
    };
    return data;
  }

  ngOnDestroy(): void {
    this.onCurrentTicketChanged.unsubscribe();
    this.onTicketDestinationChanged.unsubscribe();
  }

}
